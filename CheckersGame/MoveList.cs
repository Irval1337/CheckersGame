using System.Collections.Generic;
using System;

namespace CheckersGame
{
    internal class Move
    {
        public ValueTuple<int, int> from { get; set; }
        public ValueTuple<int, int> to { get; set; }

        public Move(ValueTuple<int, int> from, ValueTuple<int, int> to)
        {
            this.from = from;
            this.to = to;
        }
    }

    internal class MoveList
    {
        public List<Move> moves { get; set; }

        public MoveList()
        {
            moves = new List<Move>();
        }

        public void add(Move move)
        {
            moves.Add(move);
        }

        public void clear()
        {
            moves.Clear();
        }
    }
}
