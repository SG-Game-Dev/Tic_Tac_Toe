using System.Collections;
using TMPro;
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
        public GameObject disconnectPage;   // NEW (optional): "दोस्त चला गया" वाला panel
        public TicTacToeGameController gameCont;

        public static AiMode aiMode;
        public static bool isHuman;
        public static bool isOnline;        // NEW
        public static bool isBack = false;

        private void Awake()
        {
            if (isHuman)
                gameCont.isHuman = isHuman;
            else
                AiModes(aiMode);
        }

        void OnEnable()
        {
            if (isOnline) OnlineSession.Disconnected += OnOnlineDisconnected;
        }

        void OnDisable()
        {
            OnlineSession.Disconnected -= OnOnlineDisconnected;
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
            Time.timeScale = 1.0f;
            if (isOnline) return;               // Online में match की शुरुआत network तय करता है
            TicTacToeGameController.isPlaying = true;
        }

        public void Pause()
        {
            if (isOnline) return;               // Online में pause नहीं (दूसरे का game न रुके)
            TicTacToeGameController.isPlaying = false;
            Time.timeScale = 0.0f;
        }

        public void Restart()
        {
            if (isOnline)
            {
                // दोनों के लिए Host के जरिए reset होगा, scene reload नहीं
                if (NetworkGameSync.Instance != null) NetworkGameSync.Instance.RequestRestart();
                return;
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Play();
        }

        public void LoadTargetScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void Back()
        {
            if (isOnline) OnlineSession.Leave();   // connection बंद करके ही बाहर जाओ
            isBack = true;
            LoadTargetScene(0);
        }

        public void OnExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
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

        // NEW: Online restart पर result panel छिपाने के लिए
        public void HideResultPage()
        {
            StopAllCoroutines();
            winnerPage.SetActive(false);
        }

        private IEnumerator ShowWinnerPageAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            winnerPage.SetActive(true);
        }

        void OnOnlineDisconnected(string reason)
        {
            TicTacToeGameController.isPlaying = false;
            if (disconnectPage != null) disconnectPage.SetActive(true);
        }

        public void PToP()
        {
            isHuman = true;
            isOnline = false;
        }

        public void SmartAi()
        {
            isHuman = false;
            isOnline = false;
            aiMode = AiMode.SmartAi;
        }

        public void MinimaxAi()
        {
            isHuman = false;
            isOnline = false;
            aiMode = AiMode.Minimax;
        }

        public void EasyAi()
        {
            isHuman = false;
            isOnline = false;
            aiMode = AiMode.Easy;
        }

        public void AiModes(AiMode mode)
        {
            gameCont.isHuman = false;
            gameCont.aiMode = mode;
        }
    }
}
