namespace Chess
{
    public class Vector
    {
        public int X { get; }
        public int Y { get; }
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector first, Vector second)
        {
            return new Vector(first.X + second.X, first.Y + second.Y);
        }

        public static Vector operator -(Vector first, Vector second)
        {
            return new Vector(first.X - second.X, first.Y - second.Y);
        }

        public static Vector operator *(Vector vector, int value)
        {
            return new Vector(vector.X * value, vector.Y * value);
        }

        public bool IsEqual(Vector Other)
        {
            return X == Other.X && Y == Other.Y;
        }
    }
    public enum MoveType
    {
        Normal,
        EnPassant,
        Castling,
        Promotion
    }

    internal class PossibleMove 
    {
        public Vector vector;
        public MoveType moveType;
        public PossibleMove(Vector vector, MoveType moveType = MoveType.Normal)
        {
            this.vector = vector;
            this.moveType = moveType;
        }
    }

    internal class Move
    {
        public Vector From { get; set; }
        public Vector To { get; set; }
        public Piece MovedPiece { get; set; }
        public Move(Vector from, Vector to, Piece piece)
        {
            From = from;
            To = to;
            MovedPiece = piece;
        }

        public bool IsEqual(Move other)
        {
            return From.IsEqual(other.From) && To.IsEqual(other.To);
        }
    }

    internal class ChessLogic
    {
        private readonly Board board;
        private bool whiteTurn;

        private List<Move> Moves = new List<Move>();
        private Move lastMove;

        private int NoCaptureCounter;
        private int WhiteMaterial;
        private int BlackMaterial;

        public ChessLogic(Board board)
        {
            this.board = board;
            whiteTurn = true;
            NoCaptureCounter = 0;
            WhiteMaterial = 0;
            BlackMaterial = 0;
        }

        public void setLastMove(Move move) => lastMove = move;
        internal bool IsWhiteTurn() => whiteTurn;
        internal void SwitchTurn() => whiteTurn = !whiteTurn;
        internal int GetMaterialDifference()
        {
            return WhiteMaterial - BlackMaterial;
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        #region Movement logic

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        internal bool CanMove(Vector CurrentPosition, Vector NewPosition)
        {
            if (IsCheck(board, IsWhiteTurn()))
            {
                if (PotentialCheckAfterMove(CurrentPosition, NewPosition)) return false;
            }
            return true;
        }

        internal void MovePiece(Board chessboard, Vector CurrentPosition, Vector NewPosition, bool simulatingMove = false)
        {
            Tile CurrentTile = chessboard.BoardGrid[CurrentPosition.X, CurrentPosition.Y];
            Tile NewTile = chessboard.BoardGrid[NewPosition.X, NewPosition.Y];
            if (chessboard.GetPieceAt(CurrentTile) != null)
            {
                Piece CurrentPiece = chessboard.GetPieceAt(CurrentTile)!;
                Move move = new(CurrentPosition, NewPosition, CurrentPiece);

                bool isCapture = board.IsTileOccupied(NewTile);
                bool isPawnMove = CurrentPiece.Type == PieceType.Pawn;
                bool isKingMove = CurrentPiece.Type == PieceType.King;
                bool isRookMove = CurrentPiece.Type == PieceType.Rook;
                NewTile.SetOccupyingPiece(CurrentPiece);
                CurrentTile.SetEmpty();
                if (!simulatingMove)
                {
                    if (isCapture || isPawnMove)
                    {
                        NoCaptureCounter = 0;
                    }
                    else
                    {
                        NoCaptureCounter++;
                    }

                    if (isPawnMove || isKingMove || isRookMove)
                    {
                        CurrentPiece.PieceMoved();
                    }
                    Moves.Add(move);
                    setLastMove(move);

                    HandleSpecialMoveTypes(chessboard, CurrentTile, NewTile);
                }
            }
        }

        private void HandleSpecialMoveTypes(Board chessboard, Tile currentTile, Tile newTile)
        {
            if (newTile.MoveType == MoveType.EnPassant)
            {
                Vector aboveOrBelowNewTile = chessboard.GetPieceAt(newTile).IsWhite ? new Vector(1, 0) : new Vector(-1, 0);
                Vector pieceToDeletePosition = newTile.Position + aboveOrBelowNewTile;
                chessboard.BoardGrid[pieceToDeletePosition.X, pieceToDeletePosition.Y].SetEmpty();
            }

            if (newTile.MoveType == MoveType.Castling)
            {
                Piece KingPiece = chessboard.GetPieceAt(newTile)!;
                bool isQueenSide = newTile.Position.Y == 2;
                bool isKingSide = newTile.Position.Y == 6;

                if (isQueenSide)
                {
                    // Move the rook from a1 to d1 (for white) or a8 to d8 (for black)
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 0);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 3);

                    // Move the rook to its new position
                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
                else if (isKingSide)
                {
                    // Move the rook from h1 to f1 (for white) or h8 to f8 (for black)
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 7);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 5);

                    // Move the rook to its new position
                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        #endregion

        #region Game End Checking

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        internal bool IsCheck(Board board, bool whiteTurn)
        {
            //return IsKingUnderAttack(board, whiteTurn);
            return false;
        }

        internal bool IsMate(Board board, bool whiteTurn)
        {
            if (!IsCheck(board, whiteTurn))
            {
                return false;
            }
            for (int row = 0; row < board.GetBoardSize(); row++)
            {
                for (int col = 0; col < board.GetBoardSize(); col++)
                {
                    Tile tile = board.BoardGrid[row, col];
                    if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == whiteTurn)
                    {
                        Vector currentPosition = new Vector(row, col);
                        List<PossibleMove> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, currentPosition);
                        foreach (PossibleMove move in possibleMoves)
                        {
                            if (!PotentialCheckAfterMove(currentPosition, move.vector))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal bool IsStalemate(Board board, bool whiteTurn)
        {
            if (!IsCheck(board, !whiteTurn))
            {
                for (int row = 0; row < board.GetBoardSize(); row++)
                {
                    for (int col = 0; col < board.GetBoardSize(); col++)
                    {
                        Tile tile = board.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == whiteTurn)
                        {
                            List<PossibleMove> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, currentPosition);
                            foreach (PossibleMove move in possibleMoves)
                            {
                                if (!PotentialCheckAfterMove(currentPosition, move.vector))
                                {
                                    return false;
                                }
                                
                            }
                        }
                    }
                }
            }
            return true;
        }

        internal bool IsDraw()
        {
            return Repetition() || FiftyMoveRule();
        }

        private bool Repetition()
        {
            //save a list of chessboard states, if a pawn is pushed / piece taken -> I can delete all prior board states
            return false;
        }

        private bool FiftyMoveRule() 
        {
            // 50 turns -> 100 moves
            return NoCaptureCounter >= 100;
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        #endregion
        
        #region Castling logic

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        internal bool CanCastle(Board chessboard, bool white, bool queenSide)
        {
            int row = white ? 7 : 0;
            Tile kingTile = chessboard.BoardGrid[row, 4];
            Tile rookTile = chessboard.BoardGrid[row, queenSide ? 0 : 7];

            bool canKingCastle = chessboard.IsTileOccupied(kingTile) &&
                   chessboard.GetPieceAt(kingTile).Type == PieceType.King &&
                   !chessboard.GetPieceAt(kingTile).HasMoved();

            bool canRookCastle = chessboard.IsTileOccupied(rookTile) &&
                   chessboard.GetPieceAt(rookTile).Type == PieceType.Rook &&
                   !chessboard.GetPieceAt(rookTile).HasMoved();

            if (!canKingCastle || !canRookCastle)
                return false;

            if (queenSide)
                return CanCastleQueenSide(chessboard, white, row);

            return CanCastleKingSide(chessboard, white, row);
        }

        private bool CanCastleQueenSide(Board board, bool white, int row)
        {
            return !IsCheck(board, white) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 1]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 2]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 3]) &&
                   !IsKingUnderThreatOnPath(board, white, new Vector(row, 2));
        }

        private bool CanCastleKingSide(Board board, bool white, int row)
        {
            return !IsCheck(board, white) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 5]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 6]) &&
                   !IsKingUnderThreatOnPath(board, white, new Vector(row, 5));
        }
        private bool IsKingUnderThreatOnPath(Board board, bool white, Vector position)
        {
            return PotentialCheckAfterMove(new Vector(white ? 7 : 0, 4), position);
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        #endregion
        
        #region Movement helpers

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        internal void FindLegalTiles(Board board, Tile CurrentTile, Piece ChessPiece)
        {
            board.ResetLegalMoves();

            List<PossibleMove>? possibleMoves = GetPossibleMoves(board, ChessPiece, CurrentTile.Position);

            foreach (PossibleMove possibleMove in possibleMoves)
            {
                Vector tileVector = possibleMove.vector;
                if (board.WithinBounds(tileVector))
                {
                    Tile tile = board.BoardGrid[tileVector.X, tileVector.Y];
                    if (board.IsTileOccupied(tileVector) && !board.AreEnemies(CurrentTile.Position, tileVector))
                    {
                        continue;
                    }
                    tile.LegalMove = true;
                    tile.MoveType = possibleMove.moveType;
                }
            }
        }

        internal List<PossibleMove> GetPossibleMoves(Board board, Piece piece, Vector currentPosition)
        {
            List<PossibleMove> possibleMoves = new List<PossibleMove>();
            PieceType[] slidingPieces = { PieceType.Bishop, PieceType.Rook, PieceType.Queen };

            foreach (Vector vector in piece.MoveVectors)
            {
                if (slidingPieces.Contains(piece.Type))
                {
                    // For Bishop / Rook / Queen, to check the path
                    int distance = 1;
                    bool PATH_FREE = true;
                    while (distance <= board.GetBoardSize() && PATH_FREE)
                    {
                        var newX = currentPosition.X + vector.X * distance;
                        var newY = currentPosition.Y + vector.Y * distance;
                        Vector newPosition = new Vector(newX, newY);
                        PossibleMove newMove = new PossibleMove(newPosition);

                        if (!board.WithinBounds(newPosition))
                        {
                            PATH_FREE = false;
                            continue;
                        }
                        else if (board.IsTileOccupied(newPosition))
                        {
                            if (board.AreEnemies(currentPosition, newPosition))
                            {
                                possibleMoves.Add(newMove);
                                PATH_FREE = false;
                                continue;
                            }
                            else
                            {
                                PATH_FREE = false;
                                continue;
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
                    List<Vector> MoveVectors = new List<Vector> { vector };
                    Vector[] CaptureVectors = { new Vector(vector.X, -1), new Vector(vector.X, 1) };

                    AddPawnMoves(board, piece, possibleMoves, MoveVectors, currentPosition);
                    AddPawnCaptureMoves(board, possibleMoves, CaptureVectors, currentPosition);
                    AddEnPassantMoves(board, piece, possibleMoves, currentPosition);
                }
                else
                {
                    // For Knight & King
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    Vector newPosition = new Vector(newX, newY);
                    if (board.WithinBounds(newPosition))
                    {
                        if (board.IsTileOccupied(newPosition))
                        {
                            if (board.AreEnemies(currentPosition, newPosition))
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
                        bool canCastleQueenSide = CanCastle(board, piece.IsWhite, true);
                        bool canCastleKingSide = CanCastle(board, piece.IsWhite, false);
                        if (canCastleQueenSide)
                        {
                            Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 2);
                            possibleMoves.Add(new PossibleMove(kingTargetPosition, MoveType.Castling));
                        }
                        if (canCastleKingSide)
                        {
                            Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 6);
                            possibleMoves.Add(new PossibleMove(kingTargetPosition, MoveType.Castling));
                        }
                    }
                }
            }
            return possibleMoves;
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // Sliding pieces movement
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        private void AddSlidingPieceMoves(Board board, Piece piece, List<PossibleMove> possibleMoves, Vector direction, Vector currentPosition)
        {
            int distance = 1;
            while (true)
            {
                Vector newPosition = currentPosition + (direction * distance);
                if (!board.WithinBounds(newPosition))
                    break;

                if (board.IsTileOccupied(newPosition))
                {
                    if (board.AreEnemies(currentPosition, newPosition))
                        possibleMoves.Add(new PossibleMove(newPosition));

                    break;
                }
                else
                {
                    possibleMoves.Add(new PossibleMove(newPosition));
                }

                distance++;
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // Pawn movement
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        private void AddPawnMoves(Board board, Piece piece, List<PossibleMove> possibleMoves, List<Vector> moveVectors, Vector currentPosition)
        {
            if (!piece.HasMoved() && !board.IsTileOccupied(currentPosition + moveVectors[0]))
            {
                moveVectors.Add(new Vector(moveVectors[0].X * 2, 0));
            }

            foreach (Vector moveVector in moveVectors)
            {
                Vector movePosition = currentPosition + moveVector;
                if (board.WithinBounds(movePosition) && !board.IsTileOccupied(movePosition))
                {
                    possibleMoves.Add(new PossibleMove(movePosition));
                }
            }
        }

        private void AddPawnCaptureMoves(Board board, List<PossibleMove> possibleMoves, Vector[] captureVectors, Vector currentPosition)
        {
            foreach (Vector captureVector in captureVectors)
            {
                Vector capturePosition = currentPosition + captureVector;
                if (board.WithinBounds(capturePosition) && board.IsTileOccupied(capturePosition) && board.AreEnemies(currentPosition, capturePosition))
                {
                    possibleMoves.Add(new PossibleMove(capturePosition));
                }
            }
        }

        private void AddEnPassantMoves(Board board, Piece piece, List<PossibleMove> possibleMoves, Vector currentPosition)
        {
            Vector[] enPassantVectors = { new Vector(0, -1), new Vector(0, 1) };

            foreach (Vector ePVector in enPassantVectors)
            {
                Vector pawnPosition = currentPosition + ePVector;
                if (lastMove == null) 
                    continue;

                if (lastMove.MovedPiece.Type != PieceType.Pawn)
                    continue;

                if (Math.Abs(lastMove.To.X - lastMove.From.X) != 2)
                    continue;

                if (!lastMove.To.IsEqual(pawnPosition))
                    continue;

                if (!board.AreEnemies(currentPosition, pawnPosition))
                    continue;

                Vector enPassantTarget = new Vector(lastMove.To.X + (piece.IsWhite ? -1 : 1), lastMove.To.Y);
                if (board.WithinBounds(enPassantTarget))
                {
                    possibleMoves.Add(new PossibleMove(enPassantTarget, MoveType.EnPassant));
                }
            }
        }

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // King movement
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----



        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        #endregion
        
        #region Helper functions

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

        internal bool IsKingUnderAttack(Board board, bool isKingWhite)
        {
            Vector kingPosition = FindKingPosition(board, isKingWhite);
            for (int row = 0; row < board.GetBoardSize(); row++)
            {
                for (int col = 0; col < board.GetBoardSize(); col++)
                {
                    Vector currentPosition = new Vector(row, col);
                    if (board.IsTileOccupied(currentPosition) &&
                        board.GetPieceAt(currentPosition).IsWhite != isKingWhite)
                    {
                        Piece piece = board.GetPieceAt(currentPosition);
                        List<PossibleMove> possibleMoves = GetPossibleMoves(board, piece, currentPosition);

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
            for (int row = 0; row < board.GetBoardSize(); row++)
            {
                for (int col = 0; col < board.GetBoardSize(); col++)
                {
                    Vector current = new(row, col);
                    if (chessboard.IsTileOccupied(current) &&
                        chessboard.GetPieceAt(current).Type == PieceType.King &&
                        chessboard.GetPieceAt(current).IsWhite == isKingWhite)
                    {
                        kingPosition = new Vector(row, col);
                    }
                }
            }
            return kingPosition;
        }

        private bool PotentialCheckAfterMove(Vector from, Vector to)
        {
            Board PotentialBoard = new()
            {
                BoardGrid = board.getBoardCopy()
            };
            MovePiece(PotentialBoard, from, to, true);
            if (IsCheck(PotentialBoard, IsWhiteTurn()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
        // ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
    }
}
