namespace CheckersGame
{
    internal class Cell : ICloneable
    {
        public int playerId { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public bool isKing { get; set; }

        public Cell(int row_, int column_, int player_) { 
            row = row_; column = column_; playerId = player_;
            isKing = false;
        }

        public object Clone() => MemberwiseClone();
    }
}
