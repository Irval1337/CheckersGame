using System;

namespace CheckersGame
{
    internal class Move : ICloneable
    {
        public ValueTuple<int, int> from { get; set; }
        public ValueTuple<int, int> to { get; set; }

        public Move(ValueTuple<int, int> from, ValueTuple<int, int> to)
        {
            this.from = from;
            this.to = to;
        }

        public object Clone()
        {
            Move move = new Move((from.Item1, from.Item2), (to.Item1, to.Item2));
            return move;
        }
    }
}
