using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    class EasyAi : IAiStrategy
    {
        readonly AiContext ctx;
        public EasyAi(AiContext ctx) { this.ctx = ctx; }

        public GameObject ChooseMove()
        {
            List<GameObject> emptyButtons = ctx.Board.GetEmptyCells();

            if (emptyButtons.Count == 0) return null;
            return emptyButtons[Random.Range(0, emptyButtons.Count)];
        }
    }
}
