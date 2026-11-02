using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [System.Serializable]
    public class ImageFadeProperties
    {
        public RectTransform maskingImage_RectTransform;

        [Header("Fade In")]
        public float startScale_FadeIn;
        public float endScale_FadeIn;

        [Header("Fade Out")]
        public float startScale_FadeOut;
        public float endScale_FadeOut;
    }
    [System.Serializable]
    public class TransitionProperties
    {
        public GameObject tranisitionCanvas;
        public ImageFadeProperties imageFadeProperties;
        public RectTransform maskingImage_RectTransform;
        public RawImage maskingImage;

        public float timeBetweenFades;
        public AnimationCurve rotationTransitionCurve;
        [Header("Rotaing Square")]
        public bool transitionSquare_Rotation = false;
        public float rotationDuration;

        [Header("Fade In")]
        public float fadeInTransitionDuration;
        public float startScale_FadeIn;
        public float endScale_FadeIn;
        public AnimationCurve fadeInTransitionCurve;

        [Header("Fade Out")]
        public float fadeOutTransitionDuration;
        public float startScale_FadeOut;
        public float endScale_FadeOut;
        public AnimationCurve fadeOutTransitionCurve;
    }

    [Header("Game Audio")]
    [SerializeField] private AudioSource bgm_AudioSource;
    [SerializeField] private AudioClip bgm_AudioClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private bool fadeOut;
    [SerializeField] public TransitionProperties fadeTransition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        bgm_AudioSource.clip = bgm_AudioClip;
        bgm_AudioSource.Play();
    }

    #region Button Config
    public void LoadSceneOnButtonClick(string sceneName)
    {
        audioSource.PlayOneShot(buttonClip);
        fadeTransition.tranisitionCanvas.SetActive(true);

        StartCoroutine(FadeIn(sceneName, fadeTransition.imageFadeProperties));
        if (fadeTransition.transitionSquare_Rotation)
        {
            StartCoroutine(MaskRotation());
        }
    }

    public void OnAnyButtonClick(RectTransform button_RectTransform)
    {
        StartCoroutine(ButtonPopDown(button_RectTransform));
    }

    IEnumerator ButtonPopDown(RectTransform button_RectTransform)
    {
        float elapsedTime = 0;
        float duration = 0.1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / duration);
            float lerpValue = Mathf.Lerp(1, 0.75f, clampValue);
            button_RectTransform.localScale = Vector3.one * lerpValue;
            yield return null;
        }
        StartCoroutine(ButtonPopUp(button_RectTransform));
    }
    IEnumerator ButtonPopUp(RectTransform button_RectTransform)
    {
        float elapsedTime = 0;
        float duration = 0.1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / duration);
            float lerpValue = Mathf.Lerp(0.75f, 1, clampValue);
            button_RectTransform.localScale = Vector3.one * lerpValue;
            yield return null;
        }
    }
    #endregion

    IEnumerator MaskRotation()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeTransition.rotationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / fadeTransition.rotationDuration);
            float curveValue = fadeTransition.rotationTransitionCurve.Evaluate(clampValue);
            float lerpValue = Mathf.Lerp(0, 90, curveValue);
            fadeTransition.maskingImage_RectTransform.rotation = Quaternion.Euler(0, 0, lerpValue);
            yield return null;
        }
    }

    private IEnumerator FadeIn(string sceneName, ImageFadeProperties imageFadeProperties)
    {

        Time.timeScale = 0;

        float elapsedTime = 0;
        while (elapsedTime < fadeTransition.fadeInTransitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / fadeTransition.fadeInTransitionDuration);
            float curveValue = fadeTransition.fadeInTransitionCurve.Evaluate(clampValue);
            float lerpValue = Mathf.Lerp(imageFadeProperties.startScale_FadeIn, imageFadeProperties.endScale_FadeIn, curveValue);
            imageFadeProperties.maskingImage_RectTransform.localScale = Vector3.one * lerpValue;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);


        imageFadeProperties.maskingImage_RectTransform.localScale = Vector3.one * imageFadeProperties.endScale_FadeIn;
        yield return new WaitForSecondsRealtime(fadeTransition.timeBetweenFades);

        if (fadeOut)
        {
            StartCoroutine(FadeOut(imageFadeProperties));
            StartCoroutine(MaskRotation());
        }
        else
        {
            fadeTransition.tranisitionCanvas.SetActive(false);
            Time.timeScale = 1;
        }
    }
    private IEnumerator FadeOut(ImageFadeProperties imageFadeProperties)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeTransition.fadeOutTransitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float clampValue = Mathf.Clamp01(elapsedTime / fadeTransition.fadeOutTransitionDuration);
            float curveValue = fadeTransition.fadeOutTransitionCurve.Evaluate(clampValue);
            float lerpValue = Mathf.Lerp(imageFadeProperties.startScale_FadeOut, imageFadeProperties.endScale_FadeOut, curveValue);

            imageFadeProperties.maskingImage_RectTransform.localScale = Vector3.one * lerpValue;
            yield return null;
        }
        imageFadeProperties.maskingImage_RectTransform.localScale = Vector3.one * imageFadeProperties.endScale_FadeOut;
        fadeTransition.tranisitionCanvas.SetActive(false);

        Time.timeScale = 1;

    }
}
