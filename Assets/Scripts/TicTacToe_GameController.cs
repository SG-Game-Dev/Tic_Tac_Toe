using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicTacToe_GameController : MonoBehaviour
{
    [SerializeField] GameObject buttons;

    public bool turn = false; // false = O, true = X
    List<GameObject> Xplayer = new List<GameObject>();
    List<GameObject> Oplayer = new List<GameObject>();

    int countClick;

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

    void Start()
    {
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            var button = buttons.transform.GetChild(i);
            var buttonText = button.GetChild(0).GetComponent<TextMeshProUGUI>();

            if (buttonText != null)
            {
                //Add all Button listerner method, when click any button so call OnClick Method.
                button.GetComponent<Button>().onClick.AddListener(() => OnClick(buttonText, button.gameObject));
            }
        }
    }

    public void OnClick(TextMeshProUGUI buttonText, GameObject gameObject)
    {
        if (buttonText != null)
        {
            //Both Player one by one turn to play
            if (!turn)
            {
                buttonText.text = "X";
                Xplayer.Add(gameObject);
            }
            else
            {
                buttonText.text = "O";
                Oplayer.Add(gameObject);
            }

            //Disable button
            gameObject.GetComponent<Button>().interactable = false;

            countClick++;

            // Check winner
            if (Xplayer.Count >= 3 && CheckWinner(Xplayer))
            {
                Debug.Log("🎉 X जीत गया!");
                return;
            }
            else if (Oplayer.Count >= 3 && CheckWinner(Oplayer))
            {
                Debug.Log("🎉 O जीत गया!");
                return;
            }
            else if (countClick >= 9)
            {
                Debug.Log("Match Draw");
                return;
            }

            // Toggle turn at the end
            turn = !turn;
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
}
