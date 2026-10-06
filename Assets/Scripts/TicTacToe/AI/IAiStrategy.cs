using UnityEngine;

namespace TicTacToe
{
    // AI को चलने के लिए जो जानकारी चाहिए
    public class AiContext
    {
        public BoardView Board { get; }
        public PlayerMoves Human { get; }
        public PlayerMoves Ai { get; }

        public AiContext(BoardView board, PlayerMoves human, PlayerMoves ai)
        {
            Board = board;
            Human = human;
            Ai = ai;
        }
    }

    // हर AI level यह interface implement करेगा
    public interface IAiStrategy
    {
        // कौन सा button चलना है, वो लौटाओ (खाली board हो तो null)
       public abstract GameObject ChooseMove();
    }
}
