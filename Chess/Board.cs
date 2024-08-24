using System.Diagnostics;

namespace Chess
{
    internal class Board
    {
        public const int BOARD_SIZE = 8;

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

        internal void defaultPosition()
        {
            Piece.PieceType[] BackRankPieces = { 
                Piece.PieceType.Rook, 
                Piece.PieceType.Knight, 
                Piece.PieceType.Bishop, 
                Piece.PieceType.Queen, 
                Piece.PieceType.King,
                Piece.PieceType.Bishop, 
                Piece.PieceType.Knight, 
                Piece.PieceType.Rook 
            };
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                BoardGrid[0, col].AddPieceToTile(BackRankPieces[col], false);
                BoardGrid[1, col].AddPieceToTile(Piece.PieceType.Pawn, false);
                BoardGrid[6, col].AddPieceToTile(Piece.PieceType.Pawn, true);
                BoardGrid[7, col].AddPieceToTile(BackRankPieces[col], true);
            }
        }

        public Piece? GetPieceAtPosition(Vector position) => BoardGrid[position.X, position.Y].OccupyingPiece;
        public bool IsTileOccupied(Vector position) => BoardGrid[position.X, position.Y].IsOccupied;

        internal void ResetLegalTiles()
        {
            foreach (Tile tile in BoardGrid)
            {
                tile.LegalMove = false;
            }
        }

        internal void FindLegalTiles(Tile CurrentTile, Piece ChessPiece)
        {
            ResetLegalTiles();

            var possibleMoves = GetPossibleMoves(ChessPiece, CurrentTile.Position);

            foreach (var tileVector in possibleMoves)
            {
                if (tileVector.X >= 0 && tileVector.Y >= 0 && tileVector.X < BOARD_SIZE && tileVector.Y < BOARD_SIZE)
                {
                    bool isOccupied = BoardGrid[tileVector.X, tileVector.Y].IsOccupied;
                    bool isEnemy = false;
                    if (isOccupied)
                    {
                        isEnemy = BoardGrid[tileVector.X, tileVector.Y].OccupyingPiece.IsWhite != ChessPiece.IsWhite;
                    }
                    if (!isEnemy && BoardGrid[tileVector.X, tileVector.Y].IsOccupied || tileVector.X < 0 || tileVector.X >= BOARD_SIZE || tileVector.Y < 0 || tileVector.Y >= BOARD_SIZE)
                    {
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
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    possibleMoves.Add(new Vector(newX, newY));
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

        internal void MovePiece(Vector Current, Vector New)
        {
            var CurrentTile = BoardGrid[Current.X, Current.Y];
            var NewTile = BoardGrid[New.X, New.Y];

            if (CurrentTile.OccupyingPiece != null)
            {
                NewTile.AddPieceToTile(CurrentTile.OccupyingPiece.Type, CurrentTile.OccupyingPiece.IsWhite);
                CurrentTile.IsOccupied = false;
                CurrentTile.OccupyingPiece = null;
            }
        }

        public void printBoard()
        {
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    if (BoardGrid[row, col].IsOccupied)
                    {
                        char pieceColor = BoardGrid[row, col].OccupyingPiece.IsWhite ? 'W' : 'B';
                        Debug.Write($" {pieceColor}{BoardGrid[row, col].OccupyingPiece.Notation} ");
                    }
                    else
                    {
                        Debug.Write(" -- ");
                    }
                }
                Debug.Write("\n");
            }
        }

        internal bool IsKingUnderAttack(bool isKingWhite)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }
}
