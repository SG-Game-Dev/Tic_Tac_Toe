using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe
{
    public class UiManager : MonoBehaviour
    {
        public GameObject homePage;
        public GameObject startPage;
        public GameObject winnerPage;
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
        private void Update()
        {
            if ( TicTacToeGameController.isWinner)
            {
                winnerPage.SetActive(true);
                TicTacToeGameController.isWinner = false;
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
    }
}