using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    Transform transform1 = null;
    private void OnEnable()
    {
        transform1 = GetComponent<Transform>();
        StartCoroutine(FloatingAnimationIncrement());
    }
    IEnumerator FloatingAnimationIncrement()
    {
        float elapsedTime = 0;
        float startPos = transform1.position.y;
            float endPos = transform1.position.y + 0.5f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float clampedValue = Mathf.Clamp01(elapsedTime / 0.5f);
            float lerpValue = Mathf.Lerp(startPos, endPos, clampedValue);
            transform.position = new Vector3(transform.position.x, lerpValue, transform.position.z);
            yield return null;
        }
        StartCoroutine(FloatingAnimationDecrement());
    }
    IEnumerator FloatingAnimationDecrement()
    {
        float elapsedTime = 0;
        float startPos = transform1.position.y;
        float endPos = transform1.position.y - 0.5f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float clampedValue = Mathf.Clamp01(elapsedTime / 0.5f);
            float lerpValue = Mathf.Lerp(startPos, endPos, clampedValue);
            transform.position = new Vector3(transform.position.x, lerpValue, transform.position.z);
            yield return null;
        }
        StartCoroutine(FloatingAnimationIncrement());
    }
}
