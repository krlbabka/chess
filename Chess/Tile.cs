namespace Chess
{
    internal class Tile
    {
        public Vector Position { get; set;}
        public bool IsOccupied { get; set; }
        public bool LegalMove { get; set; }

        public Tile(Vector position)
        {
            this.Position = position;
            IsOccupied = false;
            LegalMove = false;
        }
    }
}
