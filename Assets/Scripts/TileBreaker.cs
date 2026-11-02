using System.Collections;
using UnityEngine;

public class TileBreaker : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2;
    private void OnEnable()
    {

        transform.localScale = Vector3.zero;
        StartCoroutine(SuicideTimer());

    }

    IEnumerator SuicideTimer()
    {
        float elapsedTime = 0;

        while (elapsedTime < lifeTime)
        {
            if (LevelManager.levelStarted)
            {
                elapsedTime += Time.deltaTime;
                float value = Mathf.Clamp01(elapsedTime / lifeTime);
                float lerpValue = Mathf.Lerp(0, 1, value);
                transform.localScale = new Vector3(lerpValue, 1, 1);
            }
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
        Destroy(this.gameObject);
    }
}
