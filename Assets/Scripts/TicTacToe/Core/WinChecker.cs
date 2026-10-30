using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    public static class WinChecker
    {
        // Winning combinations (Button names)
        public static readonly string[][] WinningCombos = new string[][]
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

        public static bool CheckWinner(List<GameObject> playerMoves)
        {
            foreach (var combo in WinningCombos)
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
}
