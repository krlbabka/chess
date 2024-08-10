namespace Chess
{
    internal class Board
    {
        const int BOARD_SIZE = 8;

        public Tile[,] boardGrid;

        public Board()
        {
            boardGrid = new Tile[BOARD_SIZE, BOARD_SIZE];
        
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    boardGrid[i, j] = new Tile(new Vector(i, j));
                }
            }
        }
    }
}
