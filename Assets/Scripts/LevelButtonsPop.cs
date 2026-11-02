using System.Collections;
using UnityEngine;

public class LevelButtonsPop : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip popAudioClip;
    [SerializeField] private AnimationCurve item_EnlargeCurve;
    [SerializeField] private AnimationCurve item_SettlingCurve;
    [SerializeField] private float item_MaxEnlarge;
    [SerializeField] private float item_EnlargeDuration;
    [SerializeField] private float item_RotateDuration;
    [SerializeField] private float item_SettlingDuration;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private float delayForButtonPop;
    [SerializeField] private float popSoundDelay;
    [SerializeField] private bool rotateItems;
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(FadeIn());
        if (rotateItems)
        {
            StartCoroutine(MaskRotation());
        }

    }

    IEnumerator MaskRotation()
    {
        float elapsedTime = 0;
        while (elapsedTime < item_RotateDuration)
        {
            elapsedTime += Time.deltaTime;
            float clampVlue = Mathf.Clamp01(elapsedTime / item_RotateDuration);
            float lerpValue = Mathf.Lerp(0, 360, clampVlue);
            rectTransform.rotation = Quaternion.Euler(0, 0, lerpValue);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        StartCoroutine(AudioPlayer());
        if (nextButton != null)
        {
            StartCoroutine(NextButtonSpawn());
        }

        float elapsedTime = 0;
        while (elapsedTime < item_EnlargeDuration)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / item_EnlargeDuration);
            float curveValue = item_EnlargeCurve.Evaluate(clampValue);
            float lerpValue = Mathf.Lerp(0, item_MaxEnlarge, curveValue);
            rectTransform.localScale = new Vector3(lerpValue, lerpValue, lerpValue);
            yield return null;
        }
        rectTransform.localScale = new Vector3(item_MaxEnlarge, item_MaxEnlarge, item_MaxEnlarge);
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < item_SettlingDuration)
        {
            elapsedTime += Time.deltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / item_SettlingDuration);
            float curveValue = item_SettlingCurve.Evaluate(clampValue);
            float lerpValue = Mathf.Lerp(item_MaxEnlarge, 1, curveValue);
            rectTransform.localScale = new Vector3(lerpValue, lerpValue, lerpValue);
            yield return null;
        }
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    public void LevelLoadOnButtonClick(string sceneName)
    {
        GameManager.instance.LoadSceneOnButtonClick(sceneName);
    }

    IEnumerator AudioPlayer()
    {
        yield return new WaitForSeconds(popSoundDelay);
        audioSource.PlayOneShot(popAudioClip);
    }
    IEnumerator NextButtonSpawn()
    {
        yield return new WaitForSeconds(delayForButtonPop);
        nextButton.SetActive(true);
    }
}
