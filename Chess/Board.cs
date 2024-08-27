namespace Chess
{
    internal class Board
    {
        public const int BOARD_SIZE = 8;

        public Tile[,] BoardGrid;
        private Move lastMove;

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

        public void setLastMove(Move move) => lastMove = move;
        public int GetBoardSize() => BOARD_SIZE;

        public Tile[,] getBoardCopy()
        {
            Tile[,] copy = new Tile[BOARD_SIZE, BOARD_SIZE];
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    copy[row, col] = new Tile(new Vector(row, col));
                    if (BoardGrid[row, col].IsOccupied)
                    {
                        copy[row, col].AddPieceToTile(BoardGrid[row, col].OccupyingPiece.Type, BoardGrid[row, col].OccupyingPiece.IsWhite);
                    }
                }
            }
            return copy;
        }

        internal void defaultPosition()
        {
            PieceType[] BackRankPieces = { 
                PieceType.Rook, 
                PieceType.Knight, 
                PieceType.Bishop, 
                PieceType.Queen, 
                PieceType.King,
                PieceType.Bishop, 
                PieceType.Knight, 
                PieceType.Rook 
            };
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                BoardGrid[0, col].AddPieceToTile(BackRankPieces[col], false);
                BoardGrid[1, col].AddPieceToTile(PieceType.Pawn, false);
                for (int row = 2; row <= 5; row++)
                {
                    BoardGrid[row, col].CreateEmptyTile();
                }
                BoardGrid[6, col].AddPieceToTile(PieceType.Pawn, true);
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

            List<PossibleMove>? possibleMoves = GetPossibleMoves(ChessPiece, CurrentTile.Position);

            foreach (PossibleMove possibleMove in possibleMoves)
            {
                Vector tileVector = possibleMove.vector;
                if (WithinBounds(tileVector))
                {
                    bool isOccupied = IsTileOccupied(tileVector);
                    if (BoardGrid[tileVector.X, tileVector.Y].IsOccupied && !AreEnemies(CurrentTile.Position, tileVector))
                    {
                        continue;
                    }
                    BoardGrid[tileVector.X, tileVector.Y].LegalMove = true;
                    BoardGrid[tileVector.X, tileVector.Y].MoveType = possibleMove.moveType;
                }
            }
        }

        internal List<PossibleMove> GetPossibleMoves(Piece piece, Vector currentPosition)
        {
            List<PossibleMove> possibleMoves = [];
            PieceType[] multiSquarePieces = [PieceType.Bishop, PieceType.Rook, PieceType.Queen];

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
                        PossibleMove newMove = new PossibleMove(newPosition);

                        if (!WithinBounds(newPosition))
                        {
                            PATH_FREE = false;
                        }
                        else if (BoardGrid[newX, newY].IsOccupied)
                        {
                            if (AreEnemies(currentPosition, newPosition))
                            {
                                possibleMoves.Add(newMove);
                                PATH_FREE = false;
                            }
                            else
                            {
                                PATH_FREE = false;
                            }
                        }
                        else
                        {
                            possibleMoves.Add(newMove);
                        }
                        distance++;
                    }
                }
                else if (piece.Type == PieceType.Pawn)
                {
                    List<Vector> MoveVectors = [vector];
                    Vector[] takeVectors = { new Vector(vector.X, -1), new Vector(vector.X, 1) };
                    Vector[] enPassantVectors = { new Vector(0, -1), new Vector(0, 1) };

                    if (!piece.HasMoved() && !IsTileOccupied(currentPosition + vector))
                    {
                        MoveVectors.Add(new Vector(vector.X * 2, 0));
                    }

                    foreach (Vector ePVector in enPassantVectors)
                    {
                        Vector pawnPosition = currentPosition + ePVector;
                        if (lastMove != null &&
                            lastMove.MovedPiece.Type == PieceType.Pawn &&
                            Math.Abs(lastMove.To.X - lastMove.From.X) == 2 &&
                            lastMove.To.IsEqual(pawnPosition) &&
                            AreEnemies(currentPosition, pawnPosition))
                        {
                            possibleMoves.Add(new PossibleMove(new Vector(pawnPosition.X + (piece.IsWhite ? -1 : 1), pawnPosition.Y), MoveType.EnPassant));
                        }
                    }

                    foreach (Vector moveVector in MoveVectors)
                    {
                        Vector movePosition = currentPosition + moveVector;
                        if (WithinBounds(movePosition) && !BoardGrid[movePosition.X, movePosition.Y].IsOccupied)
                        {
                            possibleMoves.Add(new PossibleMove(movePosition));
                        }
                    }
                    foreach (Vector takeVector in takeVectors)
                    {
                        Vector takePosition = currentPosition + takeVector;
                        if (WithinBounds(takePosition) && BoardGrid[takePosition.X, takePosition.Y].IsOccupied && AreEnemies(currentPosition, takePosition))
                        {
                            possibleMoves.Add(new PossibleMove(takePosition));
                        }
                    }
                }
                else
                {
                    // For Knight & King
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    Vector newPosition = new Vector(newX, newY);
                    if (WithinBounds(newPosition))
                    {
                        if (BoardGrid[newPosition.X, newPosition.Y].IsOccupied)
                        {
                            if (AreEnemies(currentPosition, newPosition))
                            {
                                possibleMoves.Add(new PossibleMove(newPosition));
                            }
                        }
                        else
                        {
                            possibleMoves.Add(new PossibleMove(newPosition));
                        }
                    }
                    if (piece.Type == PieceType.King)
                    {
                        //TODO: Castling -> prolly need to move stuff to chessLogic class
                    }
                }
            }
            return possibleMoves;
        }

        internal bool IsKingUnderAttack(Board chessboard, bool isKingWhite)
        {
            Vector kingPosition = FindKingPosition(chessboard, isKingWhite);
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    Vector currentPosition = new Vector(row, col);
                    if (chessboard.IsTileOccupied(currentPosition) && 
                        chessboard.GetPieceAtPosition(currentPosition).IsWhite != isKingWhite)
                    {
                        Piece piece = chessboard.GetPieceAtPosition(currentPosition);
                        List<PossibleMove> possibleMoves = chessboard.GetPossibleMoves(piece, currentPosition);

                        foreach (PossibleMove move in possibleMoves)
                        {
                            if (move.vector.IsEqual(kingPosition))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal Vector FindKingPosition(Board chessboard, bool isKingWhite)
        {
            Vector kingPosition = isKingWhite ? new Vector(0, 4) : new Vector(7, 4);
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    Vector current = new(row, col);
                    if (chessboard.IsTileOccupied(current) && 
                        chessboard.GetPieceAtPosition(current).Type == PieceType.King && 
                        chessboard.GetPieceAtPosition(current).IsWhite == isKingWhite)
                    {
                        kingPosition = new Vector(row, col);
                    }
                }
            }
            return kingPosition;
        }

        private bool WithinBounds(Vector position)
        {
            return position.X >= 0 && position.X < BOARD_SIZE && position.Y >= 0 && position.Y < BOARD_SIZE;
        }

        private bool AreEnemies(Vector myPosition, Vector newPosition)
        {
            return GetPieceAtPosition(myPosition).IsWhite != GetPieceAtPosition(newPosition).IsWhite;
        }
    }
}
