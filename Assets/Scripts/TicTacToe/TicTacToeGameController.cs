using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TicTacToe
{
    public class TicTacToeGameController : MonoBehaviour
    {
        [SerializeField] GameObject buttons;

        public bool turn = true; // false = O, true = X
        int countClick;

        [SerializeField]
        [FormerlySerializedAs("aiSate")]
        internal AiMode aiMode;

        [FormerlySerializedAs("isPlaye")]
        public static bool isPlaying = true;
        public bool isHuman = true;

        BoardView board;
        PlayerMoves player_X;
        PlayerMoves player_O;  //player2 or ai

        void Awake()
        {
            board = new BoardView(buttons);
            player_X = new PlayerMoves("X");
            player_O = new PlayerMoves("O");

            board.ClearAll();
        }

        void Start()
        {
            isPlaying = false;
            board.BindClicks(OnClick);
        }

        public void OnClick(TextMeshProUGUI buttonText, GameObject cell)
        {
            if (buttonText == null || !isPlaying) return;

            if (isHuman)
            {
                ApplyMove(turn ? player_X : player_O, cell);
                turn = !turn;
            }
            else
            {
                ApplyMove(player_X, cell);
                if (isPlaying && countClick < 9) AiMove();
            }

            CheckDraw();

        }

        void AiMove()
        {
            IAiStrategy strategy = AiFactory.Create(aiMode, new AiContext(board, player_X, player_O));
            GameObject move = strategy.ChooseMove();

            if (move != null) ApplyMove(player_O, move);
        }
        void ApplyMove(PlayerMoves player, GameObject cell)
        {
            board.SetMark(cell, player.Mark);
            player.Moves.Add(cell);
            board.LockCell(cell);
            countClick++;

            if (player.Moves.Count >= 3 && WinChecker.CheckWinner(player.Moves))
            {
                Debug.Log($"🎉 {player.Mark} जीत गया!");
                isPlaying = false;
            }
        }

        void CheckDraw()
        {
            if (countClick >= 9 && isPlaying)
            {
                isPlaying = false;
                Debug.Log("🤝 Match Draw");
            }
        }
    }
}