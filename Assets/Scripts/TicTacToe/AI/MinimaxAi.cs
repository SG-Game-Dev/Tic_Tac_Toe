using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    // Minimax: हर संभव चाल सोचकर सबसे अच्छी चाल चुनता है
    public class MinimaxAi : IAiStrategy
    {
        readonly AiContext ctx;
        public MinimaxAi(AiContext ctx) { this.ctx = ctx; }

        public GameObject ChooseMove()
        {
            List<GameObject> emptyButtons = ctx.Board.GetEmptyCells();
            GameObject bestMove = null;
            int bestScore = int.MinValue;

            foreach (var move in emptyButtons)
            {
                // Simulate AI move
                ctx.Ai.Moves.Add(move);
                ctx.Board.SetMark(move, ctx.Ai.Mark);

                List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
                newEmpty.Remove(move);

                int score = Minimax(ctx.Human.Moves, ctx.Ai.Moves, newEmpty, false);

                // Undo
                ctx.Ai.Moves.Remove(move);
                ctx.Board.ClearMark(move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            return bestMove;
        }

        int EvaluateBoard(List<GameObject> playerMoves, List<GameObject> aiMoves)
        {
            if (WinChecker.CheckWinner(aiMoves)) return +1;      // AI जीत गया
            if (WinChecker.CheckWinner(playerMoves)) return -1;  // Player जीत गया
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
                    aiMoves.Add(move);
                    ctx.Board.SetMark(move, ctx.Ai.Mark);

                    List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
                    newEmpty.Remove(move);

                    int result = Minimax(playerMoves, aiMoves, newEmpty, false);
                    bestScore = Mathf.Max(bestScore, result);

                    aiMoves.Remove(move);
                    ctx.Board.ClearMark(move);
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (var move in emptyButtons)
                {
                    playerMoves.Add(move);
                    ctx.Board.SetMark(move, ctx.Human.Mark);

                    List<GameObject> newEmpty = new List<GameObject>(emptyButtons);
                    newEmpty.Remove(move);

                    int result = Minimax(playerMoves, aiMoves, newEmpty, true);
                    bestScore = Mathf.Min(bestScore, result);

                    playerMoves.Remove(move);
                    ctx.Board.ClearMark(move);
                }
                return bestScore;
            }
        }
    }
}
