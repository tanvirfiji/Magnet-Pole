using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
 * Scene Loader With Black Effect 
 * 
 * Author: way2tushar
 * Web: https://way2tushar.com/
 * 
 */

namespace way2tushar.Utility
{
    // SceneLoader script requires the GameObject to have a CanvasGroup component
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneLoader : MonoBehaviour
    {
        [Header("Loading UI Reference")]
        [Tooltip("The object representing the loading screen.")]
        [SerializeField] private GameObject loadingObj;

        public static SceneLoader Instance { get; private set; }

        private CanvasGroup canvasGroup;
        private const float FadeDelay = 0.03f;
        private const float AlphaStep = 0.1f;

        private void Awake()
        {
            HandleSingleton();

            canvasGroup = GetComponent<CanvasGroup>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Ensures this object follows a singleton pattern.
        /// </summary>
        private void HandleSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called when a new scene is loaded.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                StartCoroutine(CanvasFade(FadeType.FADE_OUT));
            }
        }

        /// <summary>
        /// Initiates scene loading with a fade-in effect.
        /// </summary>
        public IEnumerator LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) yield break;

            gameObject.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                yield return StartCoroutine(CanvasFade(FadeType.FADE_IN));
            }

            if (loadingObj != null)
                loadingObj.SetActive(true);

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Controls the canvas fade-in and fade-out effect.
        /// </summary>
        private IEnumerator CanvasFade(FadeType fadeType)
        {
            canvasGroup.blocksRaycasts = true;

            float alphaChange = fadeType == FadeType.FADE_IN ? AlphaStep : -AlphaStep;

            while (fadeType == FadeType.FADE_IN ? canvasGroup.alpha < 1f : canvasGroup.alpha > 0f)
            {
                yield return new WaitForSecondsRealtime(FadeDelay);
                canvasGroup.alpha += alphaChange;
            }

            canvasGroup.blocksRaycasts = false;

            if (fadeType == FadeType.FADE_OUT)
            {
                gameObject.SetActive(false);
                if (loadingObj != null)
                    loadingObj.SetActive(false);
            }
        }

        public enum FadeType
        {
            FADE_IN,
            FADE_OUT
        }
    }
}
