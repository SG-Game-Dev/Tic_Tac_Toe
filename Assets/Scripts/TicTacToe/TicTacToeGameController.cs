using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTacToe
{
    public class TicTacToeGameController : MonoBehaviour
    {
        [SerializeField] GameObject buttons;

        public bool turn = false; // false = O, true = X
        int countClick;

        [FormerlySerializedAs("isPlaye")]
        public bool isPlaying = true;

        BoardView board;
        PlayerMoves player_X;   
        PlayerMoves player_O;

        void Awake()
        {
            board = new BoardView(buttons);
            player_X = new PlayerMoves("X");
            player_O = new PlayerMoves("O");

            board.ClearAll();
        }

        void Start()
        {
            board.BindClicks(OnClick);
        }

        public void OnClick(TextMeshProUGUI buttonText, GameObject cell)
        {
            if (buttonText == null || !isPlaying) return;

            // Player X turn
            if (turn)
            {
                buttonText.text = player_X.Mark;
                player_X.Moves.Add(cell);
                board.LockCell(cell);
                countClick++;
            }

            // Winner check
            if (player_X.Moves.Count >= 3 && WinChecker.CheckWinner(player_X.Moves))
            {
                Debug.Log("🎉 X जीत गया!");
                isPlaying = false;
                return;
            }

            // Player Y turn
            if (!turn)
            {
                if (countClick < 9)
                {
                    // player2 move
                    print("> trun player 2");
                    buttonText.text = player_O.Mark;
                    player_O.Moves.Add(cell);
                    board.LockCell(cell);
                    countClick++;
                }
            }

            // Winner check
            if (player_O.Moves.Count >= 3 && WinChecker.CheckWinner(player_O.Moves))
            {
                Debug.Log("🎉 O जीत गया!");
                isPlaying = false;
                return;
            }

            // Draw check
            if (countClick >= 9)
            {
                isPlaying = false;
                Debug.Log("🤝 Match Draw");
            }

            turn = !turn;
        } 
    }
}