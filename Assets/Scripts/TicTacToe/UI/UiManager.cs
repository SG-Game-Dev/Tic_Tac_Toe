using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

        static public AiMode aiMode;
        //static bool isRestart = false;

        private void Awake()
        {
            // हर scene load पर timeScale सही करें
            Time.timeScale = 1.0f;
            print($"ai mode :{aiMode}");            
        }

        private void Start()
        {
            //if (isRestart || homePage != null)
            //{
            //    homePage.SetActive(false);
            //    startPage.SetActive(true);
            //    isRestart = false;
            //    Play();
            //}            
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
            print($"ai mode:{aiMode} bu restart");
            AiModes(aiMode);
        }
        public void LoadTargetScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
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
            gameCont.isHuman = true;
        }
        public void SmartAi()
        {
            AiModes(AiMode.SmartAi);
            aiMode = AiMode.SmartAi;
        }

        public void MinimaxAi()
        {
            AiModes(AiMode.Minimax);
            aiMode = AiMode.Minimax;
        }

        public void EasyAi()
        {
            AiModes(AiMode.Easy);
            aiMode = AiMode.Easy;
        }

        public void AiModes(AiMode mode)
        {
            gameCont.isHuman = false;
            gameCont.aiMode = mode;
        }
    }
}