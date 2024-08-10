namespace Chess
{
    internal class Tile
    {
        public Vector position { get; set;}
        public bool isOccupied { get; set; }
        public bool legalMove { get; set; }

        public Tile(Vector position)
        {
            this.position = position;
            isOccupied = false;
            legalMove = false;
        }
    }
}
