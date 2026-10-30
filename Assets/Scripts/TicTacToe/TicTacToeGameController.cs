using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        //Ui variable
        [SerializeField] public UiManager uiManager;

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

            if (UiManager.isOnline) SetupOnline();
        }

        public void OnClick(TextMeshProUGUI buttonText, GameObject cell)
        {
            if (buttonText == null || !isPlaying) return;

            // Online: चाल सीधे नहीं चलती, Host को भेजी जाती है
            if (UiManager.isOnline)
            {
                OnlineClick(cell);
                return;
            }

            // ---- नीचे का हिस्सा आपका पुराना (offline) logic है, बिल्कुल वैसा ही ----
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
                uiManager.ShowResultPage(player.Mark, true, false);
                Debug.Log($"🎉 {player.Mark} जीत गया!");
                isPlaying = false;
            }
        }

        void CheckDraw()
        {
            if (countClick >= 9 && isPlaying)
            {
                uiManager.ShowResultPage("", false, true);
                isPlaying = false;
                Debug.Log("🤝 Match Draw");
            }
        }

        // =====================  ONLINE (LAN)  =====================
        // Host = X (turn true), Client = O

        void SetupOnline()
        {
            isHuman = true;
            isPlaying = false; // दोनों के तैयार होने तक कोई नहीं चल सकता
            if (NetworkGameSync.Instance != null) NetworkGameSync.Instance.Register(this);
        }

        // Host बताता है कि दोनों तैयार हैं
        public void BeginOnlineMatch()
        {
            turn = true;
            isPlaying = true;
            Time.timeScale = 1f;
        }

        void OnlineClick(GameObject cell)
        {
            // मेरी बारी नहीं तो कुछ मत करो (Host = X = turn true)
            if (turn != OnlineSession.IsHost) return;
            if (NetworkGameSync.Instance == null) return;

            NetworkGameSync.Instance.RequestMove(cell.transform.GetSiblingIndex());
        }

        // Host इसे चाल जाँचने के लिए बुलाता है
        public bool IsValidOnlineMove(int index, bool senderIsHost)
        {
            if (!isPlaying) return false;
            if (index < 0 || index >= buttons.transform.childCount) return false;
            if (senderIsHost != turn) return false;
            return IsCellEmpty(index);
        }

        // Host से "चाल सही है" का संदेश आने पर दोनों तरफ यही चलता है
        public void ApplyNetworkMove(int index)
        {
            GameObject cell = buttons.transform.GetChild(index).gameObject;

            ApplyMove(turn ? player_X : player_O, cell);
            turn = !turn;
            CheckDraw();
        }

        // Online restart
        public void ResetMatch()
        {
            player_X.Moves.Clear();
            player_O.Moves.Clear();
            countClick = 0;

            for (int i = 0; i < buttons.transform.childCount; i++)
            {
                Transform cell = buttons.transform.GetChild(i);
                cell.GetComponentInChildren<TextMeshProUGUI>().text = "";
                cell.GetComponent<Button>().interactable = true;
            }

            uiManager.HideResultPage();
            BeginOnlineMatch();
        }

        bool IsCellEmpty(int index)
        {
            return buttons.transform.GetChild(index)
                          .GetComponentInChildren<TextMeshProUGUI>().text == "";
        }
    }
}
