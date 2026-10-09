using UnityEngine;
using UnityEngine.SceneManagement;

namespace TicTacToe
{
    public class UiManager : MonoBehaviour
    {
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
        }
    }
}
