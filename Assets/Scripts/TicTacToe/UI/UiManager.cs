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
        //public GameObject 

        static bool isRestart = false;

        private void Awake()
        {
            // हर scene load पर timeScale सही करें
            Time.timeScale = 1.0f;
        }

        private void Start()
        {
            if (isRestart)
            {
                homePage.SetActive(false);
                startPage.SetActive(true);
                isRestart = false;
                Play();
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
            print("Call Restart:");

            isRestart = true;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ShowResultPage(string text,bool isWinner,bool isDraw)
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

    }
}