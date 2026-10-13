using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe
{
    public class UiManager : MonoBehaviour
    {
        public GameObject homePage;
        public GameObject startPage;
        public GameObject winnerPage;
        public TextMeshProUGUI symbol;
        public GameObject winnerImage;
        public GameObject drawImage;
        public TicTacToeGameController gameCont;

        public static AiMode aiMode;
        public static bool isHuman;
        public static bool isBack = false;

        private void Awake()
        {
            if (isHuman)
                gameCont.isHuman = isHuman;
            else
                AiModes(aiMode);
        }
        void Start()
        {
            if (isBack)
            {
                homePage.SetActive(false);
                startPage.SetActive(true);
                isBack = false;
            }
        }
        public void Play()
        {
            TicTacToeGameController.isPlaying = true;
            Time.timeScale = 1.0f;
        }

        public void Pause()
        {
            TicTacToeGameController.isPlaying = false;
            Time.timeScale = 0.0f;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Play();
        }
        public void LoadTargetScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        public void Back()
        {
            isBack = true;
            LoadTargetScene(0);
        }
        public void OnExit()
        {
#if UNITY_EDITOR
            // Unity Editor में सिर्फ Play Mode बंद होगा
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            // Windows/Mac/Linux standalone build
            Application.Quit();
#elif UNITY_ANDROID
            // Android app बंद करने के लिए
            Application.Quit();
#elif UNITY_IOS
            // iOS app बंद करने के लिए
            Application.Quit();
#else
            // Other platforms (Linux, WebGL आदि)
            Application.Quit();
#endif
        }

        public void ShowResultPage(string text, bool isWinner, bool isDraw)
        {
            symbol.text = text;
            winnerImage.SetActive(isWinner);
            drawImage.SetActive(isDraw);
            StartCoroutine(ShowWinnerPageAfterDelay());
        }
        private IEnumerator ShowWinnerPageAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            winnerPage.SetActive(true);
        }


        public void PToP()
        {
            isHuman = true;
        }
        public void SmartAi()
        {
            isHuman = false;
            aiMode = AiMode.SmartAi;
        }

        public void MinimaxAi()
        {
            isHuman = false;
            aiMode = AiMode.Minimax;
        }

        public void EasyAi()
        {
            isHuman = false;
            aiMode = AiMode.Easy;
        }

        public void AiModes(AiMode mode)
        {
            gameCont.isHuman = false;
            gameCont.aiMode = mode;
        }
    }
    //Maybe Ui all bug fix it
}