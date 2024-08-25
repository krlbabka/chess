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

            List<Vector>? possibleMoves = GetPossibleMoves(ChessPiece, CurrentTile.Position);

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

            foreach (Vector vector in piece.MoveVectors)
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
                        Vector newPosition = new Vector(newX, newY);

                        if (!WithinBounds(newPosition))
                        {
                            PATH_FREE = false;
                        }
                        else if (BoardGrid[newX, newY].IsOccupied)
                        {
                            if (CheckEnemy(currentPosition, newPosition))
                            {
                                possibleMoves.Add(newPosition);
                                PATH_FREE = false;
                            }
                            else
                            {
                                PATH_FREE = false;
                            }
                        }
                        else
                        {
                            possibleMoves.Add(newPosition);
                        }
                        distance++;
                    }
                }
                else if (piece.Type == Piece.PieceType.Pawn)
                {
                    // Weird pawn logic

                    List<Vector> MoveVectors = [vector];
                    Vector[] takeVectors = { new Vector(vector.X, -1), new Vector(vector.X, 1) };

                    if (!piece.HasMoved())
                    {
                        MoveVectors.Add(new Vector(vector.X * 2, 0));
                    }
                    Vector[] enPassantCheck = { new Vector(0, -1), new Vector(0, 1) };
                    foreach (Vector moveVector in MoveVectors)
                    {
                        Vector movePosition = currentPosition + moveVector;
                        if (WithinBounds(movePosition) && !BoardGrid[movePosition.X, movePosition.Y].IsOccupied)
                        {
                            possibleMoves.Add(movePosition);
                        }
                    }
                    foreach (Vector takeVector in takeVectors)
                    {
                        Vector takePosition = currentPosition + takeVector;
                        if (WithinBounds(takePosition) && BoardGrid[takePosition.X, takePosition.Y].IsOccupied && CheckEnemy(currentPosition, takePosition))
                        {
                            possibleMoves.Add(takePosition);
                        }
                    }
                }
                else
                {
                    // For King / Knight
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    possibleMoves.Add(new Vector(newX, newY));
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
                if (CurrentTile.OccupyingPiece.Type == Piece.PieceType.Pawn)
                {
                    CurrentTile.OccupyingPiece.PawnMoved();
                }
                NewTile.OccupyingPiece = CurrentTile.OccupyingPiece;
                NewTile.IsOccupied = true;
                CurrentTile.IsOccupied = false;
                CurrentTile.OccupyingPiece = null;
            }
        }

        internal bool IsKingUnderAttack(bool isKingWhite)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        private bool WithinBounds(Vector position)
        {
            return position.X >= 0 && position.X < BOARD_SIZE && position.Y >= 0 && position.Y < BOARD_SIZE;
        }

        private bool CheckEnemy(Vector myPosition, Vector newPosition) 
        {
            return BoardGrid[myPosition.X, myPosition.Y].OccupyingPiece.IsWhite != BoardGrid[newPosition.X, newPosition.Y].OccupyingPiece.IsWhite;
        }
    }
}
