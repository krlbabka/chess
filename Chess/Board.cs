using System.Diagnostics;

namespace Chess
{
    internal class Board
    {
        const int BOARD_SIZE = 8;

        public Tile[,] BoardGrid;

        public Board()
        {
            BoardGrid = new Tile[BOARD_SIZE, BOARD_SIZE];
        
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    BoardGrid[row, col] = new Tile(new Vector(row, col));
                }
            }
        }

        private void FindLegalTiles(Tile CurrentTile, Piece ChessPiece)
        {
            foreach (Tile tile in BoardGrid)
            {
                tile.LegalMove = false;
            }
            Debug.WriteLine($"Current tile: {CurrentTile.Position.X} - {CurrentTile.Position.Y}");

            var possibleMoves = GetPossibleMoves(ChessPiece, CurrentTile.Position);

            foreach (var tileVector in possibleMoves)
            {
                if (tileVector.X >= 0 && tileVector.Y >= 0 && tileVector.X < BOARD_SIZE && tileVector.Y < BOARD_SIZE)
                {
                    bool isOccupied = BoardGrid[tileVector.X, tileVector.Y].IsOccupied;
                    if (isOccupied)
                    {
                        bool isEnemy = isOccupied && BoardGrid[tileVector.X, tileVector.Y].OccupyingPiece.IsWhite != ChessPiece.IsWhite;
                    }
                    if (BoardGrid[tileVector.X, tileVector.Y].IsOccupied || tileVector.X < 0 || tileVector.X >= BOARD_SIZE || tileVector.Y < 0 || tileVector.Y >= BOARD_SIZE)
                    {
                        Debug.WriteLine($"Tile {tileVector.X} - {tileVector.Y} is occupied or out of bounds");
                        continue;
                    }
                    BoardGrid[tileVector.X, tileVector.Y].LegalMove = true;
                }
            }
        }

        private List<Vector> GetPossibleMoves(Piece piece, Vector currentPosition)
        {
            var possibleMoves = new List<Vector>();
            var multiSquarePieces = new[] { Piece.PieceType.Bishop, Piece.PieceType.Rook, Piece.PieceType.Queen };
            Debug.WriteLine($"Current piece: {piece.Type}");

            foreach (var vector in piece.MoveVectors)
            {
                if (multiSquarePieces.Contains(piece.Type))
                {
                    // For Bishop / Rook / Queen, to check the path
                    int distance = 1;
                    bool PATH_FREE = true;
                    while (distance <= BOARD_SIZE && PATH_FREE)
                    {
                        var newX = currentPosition.X + vector.X * distance;
                        var newY = currentPosition.Y + vector.Y * distance;

                        if (newX < 0 || newX >= BOARD_SIZE || newY < 0 || newY >= BOARD_SIZE)
                        {
                            PATH_FREE = false;
                        }
                        else if (BoardGrid[newX, newY].IsOccupied)
                        {
                            if (BoardGrid[currentPosition.X, currentPosition.Y].OccupyingPiece.IsWhite != BoardGrid[newX, newY].OccupyingPiece.IsWhite)
                            {
                                possibleMoves.Add(new Vector(newX, newY));
                                PATH_FREE = false;
                            }
                            else
                            {
                                PATH_FREE = false;
                            }
                            //TODO: Check if the conditions are correct via debug later
                        }
                        else
                        {
                            possibleMoves.Add(new Vector(newX, newY));
                        }
                        distance++;
                    }
                }
                else if (piece.Type == Piece.PieceType.Pawn)
                {
                    //TODO: Add pawn logic and checks
                }
                else
                {
                    // For King / Knight
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    possibleMoves.Add(new Vector(newX, newY));
                    Debug.WriteLine($"Added vector: {newX} - {newY}");
                }
            }
            return possibleMoves;
        }

        private void MovePiece(Tile CurrentTile, Tile NewTile)
        {
            NewTile.IsOccupied = true;
            NewTile.OccupyingPiece = CurrentTile.OccupyingPiece;
            CurrentTile.IsOccupied = false;
            CurrentTile.OccupyingPiece = null;
            //TODO: add taken piece to a list of taken pieces and add the value of taken piece to the player
        }

        public void printBoard()
        {
            // For debugging only
            FindLegalTiles(BoardGrid[3, 3], BoardGrid[3, 3].OccupyingPiece);
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if (BoardGrid[row, col].IsOccupied)
                    {
                        Debug.Write($" {BoardGrid[row, col].OccupyingPiece.ToString()[6]} ");
                        if (BoardGrid[row, col].LegalMove == true)
                        {
                            Debug.Write(" * ");
                        }
                    }
                    else if (BoardGrid[row, col].LegalMove == true)
                    {
                        Debug.Write(" * ");
                    }
                    else
                    {
                        Debug.Write(" - ");
                    }
                }
                Debug.Write("\n");
            }
        }
    }
}
