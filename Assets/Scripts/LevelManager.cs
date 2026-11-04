using System;
using System.Collections;
using TMPro;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public partial class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Serializable]
    public class LevelUI
    {
        [Header("Key")]
        public GameObject keyParent;
        public float keyTimer;
        public Image key;
        [Header("Tiles")]
        public bool limitedTiles = false;
        public int amountOfTiles;
        public TextMeshProUGUI tilesAmountUI;
    }

    [SerializeField] private GameObject tapPanel;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private GameObject[] cameras;
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Door thisLevelDoor;
    [SerializeField] private Transform keyTileTransform;
    [SerializeField] private GameObject retryUICanvas;
    [AutoStaticsCleanup] public static bool levelStarted = false;

    [SerializeField] private LevelUI levelUI;
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1.0f;
    }
    IEnumerator Start()
    {
        levelStarted = false;
        levelUI.tilesAmountUI.text = levelUI.amountOfTiles.ToString("#,#");
        foreach (GameObject cam in cameras)
        {
            if (cam.activeSelf)
            {
                cam.SetActive(false);
            }
            cam.SetActive(true);
            yield return new WaitForSeconds(2f);
            cam.SetActive(false);
        }
        tapPanel.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        StartCoroutine(TapToPlayDilation());
    }

    IEnumerator TapToPlayDilation()
    {
        float elapsedTime = 0;

        while(elapsedTime < 0.25f)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / 0.25f);
            float lerpValue = Mathf.Lerp(0, -1, clampValue);
            startText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, lerpValue);
            yield return null;
        }
        levelStarted = true;
        StartCoroutine(KeyTileDestructionTimer());
        tapPanel.SetActive(false);
    }

    IEnumerator KeyTileDestructionTimer()
    {
        float elapsedTime = 0;

        while (elapsedTime < levelUI.keyTimer)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / levelUI.keyTimer);
            float lerpValue = Mathf.Lerp(1, 0, clampValue);
            levelUI.key.fillAmount = lerpValue;
            if(lerpValue < 0.35f && levelUI.key.color != Color.red)
            {
                StartCoroutine(KeyWarningVisual());
            }
            yield return null;
        }
        levelUI.key.fillAmount = 0;
        StartCoroutine(KeyTileDestroy());
    }

    IEnumerator KeyWarningVisual()
    {
        float elapsedTime = 0;

        while(elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / 1);
            Color lerpValue = Color.Lerp(levelUI.key.color, Color.red, clampValue);
            levelUI.key.color = lerpValue;
            yield return null;
        }
    }

    IEnumerator KeyTileDestroy()
    {
        yield return null;
        Vector3Int cellPos = tileMap.WorldToCell(keyTileTransform.position);
        tileMap.SetTile(cellPos, null);
    }

    public void PlayerCollectedKey()
    {
        StopCoroutine(KeyTileDestructionTimer());
        levelUI.keyParent.SetActive(false);
        thisLevelDoor.UnlockDoor();
    }

    public void TilesAvailabilityUpdate()
    {
        float currentAvailableTiles = levelUI.amountOfTiles -= 1;
        levelUI.tilesAmountUI.text = currentAvailableTiles.ToString("#,#");
        if (levelUI.tilesAmountUI.color != Color.red && currentAvailableTiles <= 5)
        {
            StartCoroutine(TextColorChange());
        }
    }
    IEnumerator TextColorChange()
    {
        float elapsedTime = 0;

        while (elapsedTime < 2)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / 2);
            Color dangerAmount = Color.Lerp(levelUI.tilesAmountUI.color, Color.red, clampValue);
            levelUI.tilesAmountUI.color = dangerAmount;
            yield return null;
        }
    }
    public int AvailableTiles()
    {
        return levelUI.amountOfTiles;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Key"))
        {
            Time.timeScale = 0;
            retryUICanvas.SetActive(true);
            levelStarted = false;
        }
    }

    public void RetryButton()
    {
        Time.timeScale = 1;
        retryUICanvas.SetActive(false);
        string currentScene = SceneManager.GetActiveScene().name;
        GameManager.instance.LoadSceneOnButtonClick(currentScene);
    }
}
