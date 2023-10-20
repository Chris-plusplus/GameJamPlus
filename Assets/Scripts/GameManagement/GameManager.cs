using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using DG.Tweening;

namespace GameManagment
{
    public class GameManager : MonoBehaviour
    {
        public static event Action OnPauseGame;
        public static event Action OnResumeGame;

        [SerializeField, Required] private GameObject loadingScreen;
        [SerializeField, Required] private Slider loadingBar;

        public static bool IsGamePaused { get; private set; } = false;

        private const string mainSceneName = "";

        private IEnumerator loader;


        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Application.targetFrameRate = 60;
            DOTween.SetTweensCapacity(200, 1100);
        }

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            loadingScreen.SetActive(false);
            PauseGame();
            LoadGame();
        }
        
        public void LoadGame()
        {
            if (loader != null)
                return;

            loader = LoadGame(mainSceneName);
            StartCoroutine(loader);
        }

        private IEnumerator LoadGame(string sceneName)
        {
            yield return null;

            loadingBar.value = 0;
            loadingScreen.SetActive(true);

            /*
            Debug.Log($"Loading {sceneName}");
            AsyncOperation loadingDimension = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!loadingDimension.isDone)
            {
                loadingBar.value = loadingDimension.progress / 0.9f * 0.5f;
                yield return null;
            }

            Debug.Log($"Loaded {sceneName}");
            */

            loadingScreen.SetActive(false);
            loader = null;
            ResumeGame();
        }

        public static void PauseGame()
        {
            IsGamePaused = true;
            SetCursorState(false);
            OnPauseGame.Invoke();
        }
        public static void ResumeGame()
        {
            IsGamePaused = false;
            SetCursorState(true);
            OnResumeGame?.Invoke();
        }
        public static void SetCursorState(bool isLock)
        {
            if (isLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.SetCursor(default, Vector2.zero, CursorMode.ForceSoftware);
                Cursor.visible = false;
            }
            else
            {
                Cursor.SetCursor(default, Vector2.zero, CursorMode.ForceSoftware);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}