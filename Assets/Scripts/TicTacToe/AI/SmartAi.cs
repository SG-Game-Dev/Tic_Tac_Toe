using System.Collections.Generic;
using System.Linq;
using TicTacToe;
using UnityEngine;


namespace TicTacToe
{
    // Smart: 1) जीतने वाली चाल  2) रोकने वाली चाल  3) random
    public class SmartAi : IAiStrategy
    {
        readonly AiContext ctx;
        public SmartAi(AiContext ctx) { this.ctx = ctx; }

        public GameObject ChooseMove()
        {
            List<GameObject> emptyButtons = ctx.Board.GetEmptyCells();

            // 1. Winning move check
            foreach (var combo in WinChecker.WinningCombos)
            {
                var aiMoves = ctx.Ai.Moves.Where(b => combo.Contains(b.name)).ToList();
                var empty = emptyButtons.FirstOrDefault(b => combo.Contains(b.name));
                if (aiMoves.Count == 2 && empty != null) return empty;
            }

            // 2. Blocking move check
            foreach (var combo in WinChecker.WinningCombos)
            {
                var playerMoves = ctx.Human.Moves.Where(b => combo.Contains(b.name)).ToList();
                var empty = emptyButtons.FirstOrDefault(b => combo.Contains(b.name));
                if (playerMoves.Count == 2 && empty != null) return empty;
            }

            // 3. Random move
            if (emptyButtons.Count > 0)
                return emptyButtons[Random.Range(0, emptyButtons.Count)];

            return null;
        }
    }
}