using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicTacToe_With_AiPlaye : MonoBehaviour
{
    [SerializeField] GameObject buttons;

    public bool turn = false; // false = O, true = X
    List<GameObject> Xplayer = new List<GameObject>();
    List<GameObject> Oplayer = new List<GameObject>();

    public AiMode aiSate;
    int countClick;
    public bool isPlaye = true;

    // Winning combinations (Button names)
    string[][] winningCombos = new string[][]
    {
        new string[] {"Button 1","Button 2","Button 3"}, // Row 1
        new string[] {"Button 4","Button 5","Button 6"}, // Row 2
        new string[] {"Button 7","Button 8","Button 9"}, // Row 3
        new string[] {"Button 1","Button 4","Button 7"}, // Col 1
        new string[] {"Button 2","Button 5","Button 8"}, // Col 2
        new string[] {"Button 3","Button 6","Button 9"}, // Col 3
        new string[] {"Button 1","Button 5","Button 9"}, // Diagonal
        new string[] {"Button 3","Button 5","Button 7"}  // Diagonal
    };
    public enum AiMode
    {
        Easy,
        smartAi,
        Minimax
    }

    private void Awake()
    {
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            buttons.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }
    void Start()
    {
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var button = buttons.transform.GetChild(i);
            var buttonText = button.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (buttonText != null)
            {
                button.GetComponent<Button>().onClick.AddListener(() => OnClick(buttonText, button.gameObject));
            }
        }
    }

    public void OnClick(TextMeshProUGUI buttonText, GameObject game_Object)
    {
        if (buttonText != null && isPlaye)
        {
            // Player move (X)
            buttonText.text = "X";
            Xplayer.Add(game_Object);
            game_Object.GetComponent<Button>().interactable = false;
            countClick++;

            // Winner check
            if (Xplayer.Count >= 3 && CheckWinner(Xplayer))
            {
                Debug.Log("🎉 X जीत गया!");
                isPlaye = false;
                return;
            }

            // अगर अभी तक drow नहीं हुआ तो AI move करो
            if (countClick < 9)
            {
                AiMove();  // <-- AI को call करो
            }

            // Draw check
            if (countClick >= 9)
            {
                isPlaye = false;
                Debug.Log("🤝 Match Draw");
            }
        }
    }
    private void OnGUI()
    {
        float margin = 10f;
        float buttonWithd = 100f;
        float buttonHight = 50f;
        float x = Screen.width - buttonWithd - margin;
        float y = Screen.height - buttonHight - margin;

        GUIStyle textStyle = new GUIStyle()
        {
            fontSize = 50,
            fontStyle = FontStyle.Bold,
        };
        textStyle.normal.textColor = Color.red;

        if (!turn)
            GUI.Label(new Rect(10, 10, 300, 50), "Your turn X", textStyle);
        else
            GUI.Label(new Rect(10, 10, 300, 50), "Ai turn O", textStyle);

        if (GUI.Button(new Rect(x, y, buttonWithd, buttonHight), "Restar"))
        {
            print("Reload screen");
            // Current scene reload
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    bool CheckWinner(List<GameObject> playerMoves)
    {
        foreach (var combo in winningCombos)
        {
            bool hasCombo = true;
            foreach (var btnName in combo)
            {
                if (!playerMoves.Exists(b => b.name == btnName))
                {
                    hasCombo = false;
                    break;
                }
            }
            if (hasCombo) return true;
        }
        return false;
    }
    //ai move leve
    void AiMove()
    {
        List<GameObject> emptyButtons = GetEmptyButtons();

        switch (aiSate)
        {
            case AiMode.Easy:
                EasyAiPlay();
                break;
            case AiMode.smartAi:
                SmartAiPlay();
                break;
            case AiMode.Minimax:
                InpiliMentMinimax_1(emptyButtons);
                break;
        }
    }
    void EasyAiPlay()
    {
        List<GameObject> emptyButtons = new List<GameObject>();

        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var btn = buttons.transform.GetChild(i).gameObject;
            var txt = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (txt.text == "") emptyButtons.Add(btn);
        }

        if (emptyButtons.Count > 0)
        {
            GameObject choice = emptyButtons[Random.Range(0, emptyButtons.Count)];
            var txt = choice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            txt.text = "O";
            Oplayer.Add(choice);
            choice.GetComponent<Button>().interactable = false;
            countClick++;

            if (CheckWinner(Oplayer))
            {
                Debug.Log("🤖 O (AI) जीत गया!");
                isPlaye = false;
            }
        }
    }
    void SmartAiPlay()
    {
        List<GameObject> emptyButtons = new List<GameObject>();
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var btn = buttons.transform.GetChild(i).gameObject;
            var txt = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (txt.text == "") emptyButtons.Add(btn);
        }

        // 1. Winning move check
        foreach (var combo in winningCombos)
        {
            var aiMoves = Oplayer.Where(b => combo.Contains(b.name)).ToList();
            var empty = emptyButtons.FirstOrDefault(b => combo.Contains(b.name));
            if (aiMoves.Count == 2 && empty != null)
            {
                MakeAiMove(empty);
                return;
            }
        }

        // 2. Blocking move check
        foreach (var combo in winningCombos)
        {
            var playerMoves = Xplayer.Where(b => combo.Contains(b.name)).ToList();
            var empty = emptyButtons.FirstOrDefault(b => combo.Contains(b.name));
            if (playerMoves.Count == 2 && empty != null)
            {
                MakeAiMove(empty);
                return;
            }
        }

        // 3. Random move
        if (emptyButtons.Count > 0)
        {
            GameObject choice = emptyButtons[Random.Range(0, emptyButtons.Count)];
            MakeAiMove(choice);
        }
    }
    void InpiliMentMinimax()
    {
        List<GameObject> emptyButtons = new List<GameObject>();
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var btn = buttons.transform.GetChild(i).gameObject;
            var txt = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (txt.text == "") emptyButtons.Add(btn);
        }

        int bestScore = int.MinValue;
        GameObject bestMove = null;

        foreach (var move in emptyButtons)
        {
            // Try AI move
            Oplayer.Add(move);
            var txt = move.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            txt.text = "O";

            List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
            newEmpty.Remove(move);

            int score = Minimax(Xplayer, Oplayer, newEmpty, false);

            // Undo
            Oplayer.Remove(move);
            txt.text = "";

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        if (bestMove != null)
        {
            var txt = bestMove.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            txt.text = "O";
            Oplayer.Add(bestMove);
            bestMove.GetComponent<Button>().interactable = false;
            countClick++;

            if (CheckWinner(Oplayer))
            {
                Debug.Log("🤖 O (AI) जीत गया!");
                isPlaye = false;
            }
        }
    }
    private void InpiliMentMinimax_1(List<GameObject> emptyButtons)
    {
        GameObject bestMove = null;
        int bestScore = int.MinValue;

        foreach (var move in emptyButtons)
        {
            // Simulate AI move
            Oplayer.Add(move);
            var txt = move.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            txt.text = "O";

            List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
            newEmpty.Remove(move);

            int score = Minimax(Xplayer, Oplayer, newEmpty, false);

            // Undo
            Oplayer.Remove(move);
            txt.text = "";

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        if (bestMove != null)
        {
            var txt = bestMove.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            txt.text = "O";
            Oplayer.Add(bestMove);
            bestMove.GetComponent<Button>().interactable = false;
            countClick++;

            if (CheckWinner(Oplayer))
            {
                Debug.Log("🤖 O (AI) जीत गया!");
                isPlaye = false;
            }
        }
    }

    List<GameObject> GetEmptyButtons()
    {
        List<GameObject> emptyButtons = new List<GameObject>();
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var btn = buttons.transform.GetChild(i).gameObject;
            var txt = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (txt.text == "") emptyButtons.Add(btn);
        }

        return emptyButtons;
    }
    int EvaluateBoard(List<GameObject> playerMoves, List<GameObject> aiMoves)
    {
        if (CheckWinner(aiMoves)) return +1;   // AI जीत गया
        if (CheckWinner(playerMoves)) return -1; // Player जीत गया
        return 0; // Draw
    }

    int Minimax(List<GameObject> playerMoves, List<GameObject> aiMoves, List<GameObject> emptyButtons, bool isAiTurn)
    {
        // Base case
        int score = EvaluateBoard(playerMoves, aiMoves);
        if (score != 0 || emptyButtons.Count == 0) return score;

        if (isAiTurn)
        {
            int bestScore = int.MinValue;
            foreach (var move in emptyButtons)
            {
                // AI move simulate
                aiMoves.Add(move);
                var txt = move.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                txt.text = "O";

                List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
                newEmpty.Remove(move);

                int result = Minimax(playerMoves, aiMoves, newEmpty, false);
                bestScore = Mathf.Max(bestScore, result);

                // Undo move
                aiMoves.Remove(move);
                txt.text = "";
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            foreach (var move in emptyButtons)
            {
                // Player move simulate
                playerMoves.Add(move);
                var txt = move.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                txt.text = "X";

                List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
                newEmpty.Remove(move);

                int result = Minimax(playerMoves, aiMoves, newEmpty, true);
                bestScore = Mathf.Min(bestScore, result);

                // Undo move
                playerMoves.Remove(move);
                txt.text = "";
            }
            return bestScore;
        }
    }
    void MakeAiMove(GameObject choice)
    {
        var txt = choice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txt.text = "O";
        Oplayer.Add(choice);
        choice.GetComponent<Button>().interactable = false;
        countClick++;

        if (CheckWinner(Oplayer))
            Debug.Log("🤖 O (AI) जीत गया!");
    }

}
