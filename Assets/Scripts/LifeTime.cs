using System;
using System.Collections;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [Tooltip("the life time of the object")]
    [SerializeField] private float lifeTime = 5;

    private void Start()
    {
        if (LevelManager.levelStarted)
        {
            StartCoroutine(TimerStart());
        }
    }

    IEnumerator TimerStart()
    {
        float timer = 0;
        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            Mathf.Clamp01(timer / lifeTime);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
