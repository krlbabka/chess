using System.Diagnostics;

namespace Chess
{
    internal static class Program
    {
        static void Main()
        {

            Board board = new Board();


            Debug.WriteLine("Start!");

            board.BoardGrid[3, 3].IsOccupied = true;
            board.BoardGrid[3, 3].OccupyingPiece = new Queen(true);

            board.BoardGrid[6, 6].IsOccupied = true;
            board.BoardGrid[6, 6].OccupyingPiece = new Knight(false);

            board.BoardGrid[3, 6].IsOccupied = true;
            board.BoardGrid[3, 6].OccupyingPiece = new Rook(true);

            board.printBoard();

            Debug.WriteLine("Piece test end");

            ApplicationConfiguration.Initialize();
            Application.Run(new GameWindow());
        }
    }
}