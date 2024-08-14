namespace Chess
{
    internal class Tile
    {
        public Vector Position { get; set;}
        public bool IsOccupied { get; set; }
        public bool LegalMove { get; set; }
        public Piece? OccupyingPiece { get; set; }

        public Tile(Vector position)
        {
            Position = position;
            IsOccupied = false;
            LegalMove = false;
            OccupyingPiece = null;
        }
    }
}
