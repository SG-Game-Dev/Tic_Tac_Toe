using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using static TicTacToe_With_AiPlaye;

namespace TicTacToe
{
    public class TicTacToeGameController : MonoBehaviour
    {
        [SerializeField] GameObject buttons;

        public bool turn = true; // false = O, true = X
        int countClick;

        [FormerlySerializedAs("isPlaye")]
        public bool isPlaying = true;
        public bool isHuman = true;

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

            if (isHuman)
                PlayerWithHumen(buttonText,cell);
            else PlayeWithAi(buttonText, cell);

        }
        void PlayeWithAi(TextMeshProUGUI buttonText, GameObject cell)
        {
            // Player X turn

            buttonText.text = player_X.Mark;
            player_X.Moves.Add(cell);
            board.LockCell(cell);
            countClick++;


            // Winner check
            if (player_X.Moves.Count >= 3 && WinChecker.CheckWinner(player_X.Moves))
            {
                Debug.Log("🎉 X जीत गया!");
                isPlaying = false;
                return;
            }

            // Player Y turn

            if (countClick < 9)
            {
                // player2 move                
                AiMove();
            }

            // Draw check
            if (countClick >= 9)
            {
                isPlaying = false;
                Debug.Log("🤝 Match Draw");
            }

        }
        void PlayerWithHumen(TextMeshProUGUI buttonText, GameObject cell)
        {
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
                    MovePlayer_O(buttonText, cell);
                    //AiMove();
                }
            }

            // Draw check
            if (countClick >= 9)
            {
                isPlaying = false;
                Debug.Log("🤝 Match Draw");
            }

            turn = !turn;
        }
        void AiMove()
        {
            IAiStrategy strategy = new EasyAi(new AiContext(board, player_X, player_O));
            GameObject move = strategy.ChooseMove();

            if (move != null) ApplyAiMove(move);
        }

        void ApplyAiMove(GameObject cell)
        {
            board.SetMark(cell, player_O.Mark);
            player_O.Moves.Add(cell);
            board.LockCell(cell);
            countClick++;

            if (WinChecker.CheckWinner(player_O.Moves))
            {
                Debug.Log("🤖 O (player_O) जीत गया!");
                isPlaying = false;
            }
        }

        void MovePlayer_O(TextMeshProUGUI buttonText, GameObject cell)
        {
            buttonText.text = player_O.Mark;
            player_O.Moves.Add(cell);
            board.LockCell(cell);
            countClick++;

            // Winner check
            if (player_O.Moves.Count >= 3 && WinChecker.CheckWinner(player_O.Moves))
            {
                Debug.Log("🎉 O जीत गया!");
                isPlaying = false;
                return;
            }
        }
    }
}