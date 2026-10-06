using UnityEngine;
using System.Collections.Generic;

public class TicTacToe
{
    public class PlayerMoves
    {
        public string Mark { get; }
        public List<GameObject> Moves { get; } = new List<GameObject>();

        public PlayerMoves(string mark)
        {
            Mark = mark;
        }
    }

}
