
namespace Chess
{
    public class Vector
    {
        public int X { get; set; }
        public int Y { get; set; }
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
        private Board board;
        private bool WhiteTurn;

        private List<Move> Moves = new List<Move>();
        private Move lastMove;

        private int NoCaptureCounter;
        private int WhiteMaterial;
        private int BlackMaterial;

        public ChessLogic(Board board)
        {
            this.board = board;
            WhiteTurn = true;
            NoCaptureCounter = 0;
            WhiteMaterial = 0;
            BlackMaterial = 0;
        }

        public void setLastMove(Move move) => lastMove = move;
        internal bool IsWhiteTurn() => WhiteTurn;
        internal void SwitchTurn() => WhiteTurn = !WhiteTurn;
        internal int GetMaterialDifference()
        {
            return WhiteMaterial - BlackMaterial;
        }

        internal bool CanMove(Vector Current, Vector New)
        {
            if (IsCheck(board, IsWhiteTurn()))
            {
                if (PotentialCheckAfterMove(Current, New)) return false;
            }

            return true;
        }

        internal void MovePiece(Board chessboard, Vector Current, Vector New)
        {
            Tile CurrentTile = chessboard.BoardGrid[Current.X, Current.Y];
            Tile NewTile = chessboard.BoardGrid[New.X, New.Y];
            NoCaptureCounter++;
            if (CurrentTile.OccupyingPiece != null)
            {
                Move lastMove = new(Current, New, CurrentTile.OccupyingPiece);
                Moves.Add(lastMove);
                if (NewTile.IsOccupied)
                {
                    NoCaptureCounter = 0;
                }
                setLastMove(lastMove);
                if (CurrentTile.OccupyingPiece.Type == PieceType.Pawn)
                {
                    CurrentTile.OccupyingPiece.PieceMoved();
                }
                NewTile.OccupyingPiece = CurrentTile.OccupyingPiece;
                NewTile.IsOccupied = true;
                CurrentTile.IsOccupied = false;
                CurrentTile.OccupyingPiece = null;
                if (NewTile.MoveType == MoveType.EnPassant)
                {
                    Vector pieceToDelete = New;
                    if (NewTile.OccupyingPiece.IsWhite)
                    {
                        pieceToDelete += new Vector(1, 0);
                    }
                    else
                    {
                        pieceToDelete += new Vector(-1, 0);
                    }
                    chessboard.BoardGrid[pieceToDelete.X, pieceToDelete.Y].IsOccupied = false;
                    chessboard.BoardGrid[pieceToDelete.X, pieceToDelete.Y].OccupyingPiece = null;
                }
                if (NewTile.MoveType == MoveType.Castling)
                {

                }
            }
        }

        internal bool IsCheck(Board board, bool whiteTurn)
        {
            return IsKingUnderAttack(board, whiteTurn);
        }

        internal bool IsMate(Board board, bool whiteTurn)
        {
            if (IsCheck(board, whiteTurn))
            {
                for (int row = 0; row < board.GetBoardSize(); row++)
                {
                    for (int col = 0; col < board.GetBoardSize(); col++)
                    {
                        Tile tile = board.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == whiteTurn)
                        {
                            List<PossibleMove>possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, currentPosition);
                            foreach (PossibleMove move in possibleMoves)
                            {
                                Board PotentialBoard = new Board();
                                PotentialBoard.BoardGrid = board.getBoardCopy();
                                MovePiece(PotentialBoard, currentPosition, move.vector);
                                if (!IsCheck(PotentialBoard, whiteTurn))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        internal bool IsStalemate(Board board, bool WhiteTurn)
        {
            if (!IsCheck(board, !WhiteTurn))
            {
                for (int row = 0; row < board.GetBoardSize(); row++)
                {
                    for (int col = 0; col < board.GetBoardSize(); col++)
                    {
                        Tile tile = board.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == WhiteTurn)
                        {
                            List<PossibleMove> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, currentPosition);
                            foreach (PossibleMove move in possibleMoves)
                            {
                                Board PotentialBoard = new Board();
                                PotentialBoard.BoardGrid = board.getBoardCopy();
                                MovePiece(PotentialBoard, currentPosition, move.vector);
                                if (!IsCheck(PotentialBoard, WhiteTurn))
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
            if (Repetition())
            {
                return true;
            }
            return false;
        }

        private bool Repetition()
        {
            // 50 turns -> 100 moves
            if (NoCaptureCounter == 100)
            {
                return true;
            }
            //save a list of chessboard states, if a pawn is pushed / piece taken -> I can delete all prior board states
            return false;
        }

        internal bool CanCastle(bool White, bool QueenSide)
        {
            int row = White ? 7 : 0;
            Tile KingTile = board.BoardGrid[row, 4];
            Tile QSideRookTile = board.BoardGrid[row, 0];
            Tile KSideRookTile = board.BoardGrid[row, 7];
            // Pieces are either not there, or are on the correct tile, but have moved
            bool KingNotEligible = !board.IsTileOccupied(KingTile) || !IsPieceOnTile(KingTile, PieceType.King) ||
                (IsPieceOnTile(KingTile, PieceType.King) && KingTile.OccupyingPiece.HasMoved());

            bool QueenSideRookNotEligible = !board.IsTileOccupied(QSideRookTile) || !IsPieceOnTile(QSideRookTile, PieceType.Rook) ||
                (IsPieceOnTile(QSideRookTile, PieceType.Rook) && QSideRookTile.OccupyingPiece.HasMoved());

            bool KingSideRookNotEligible = !board.IsTileOccupied(KSideRookTile) || !IsPieceOnTile(KSideRookTile, PieceType.Rook) ||
                (IsPieceOnTile(KSideRookTile, PieceType.Rook) && board.BoardGrid[row, 7].OccupyingPiece.HasMoved());

            bool KingSideEmptyWay = !board.IsTileOccupied(new Vector(row, 5)) && !board.IsTileOccupied(new Vector(row, 6));
            bool QueenSideEmptyWay = !board.IsTileOccupied(new Vector(row, 1)) && !board.IsTileOccupied(new Vector(row, 2)) && !board.IsTileOccupied(new Vector(row, 3));

            if (KingNotEligible)
            {
                return false;
            }
            if (QueenSide)
            {
                bool KingPathCheckBool_1 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, -1));
                bool KingPathCheckBool_2 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, -2));
                if (QueenSideRookNotEligible || QueenSideEmptyWay || IsCheck(board, White) || KingPathCheckBool_1 || KingPathCheckBool_2)
                {
                    return false;
                }
            }
            else
            {
                bool KingPathCheckBool_1 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, 1));
                bool KingPathCheckBool_2 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, 2));
                if (KingSideRookNotEligible || KingSideEmptyWay || IsCheck(board, White) || KingPathCheckBool_1 || KingPathCheckBool_2)
                {
                    return false;
                }
            }
            return true;
        }

        private bool PotentialCheckAfterMove(Vector from, Vector to)
        {
            Board PotentialBoard = new()
            {
                BoardGrid = board.getBoardCopy()
            };
            MovePiece(PotentialBoard, from, to);
            if (IsCheck(PotentialBoard, IsWhiteTurn()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsPieceOnTile(Tile tile, PieceType type) 
        {
            return tile.OccupyingPiece.Type == type;
        }

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
            PieceType[] multiSquarePieces = [PieceType.Bishop, PieceType.Rook, PieceType.Queen];

            foreach (Vector vector in piece.MoveVectors)
            {
                if (multiSquarePieces.Contains(piece.Type))
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
                        }
                        else if (board.IsTileOccupied(newPosition))
                        {
                            if (board.AreEnemies(currentPosition, newPosition))
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
                        //TODO: Castling -> prolly need to move stuff to chessLogic class
                    }
                }
            }
            return possibleMoves;
        }

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
                if (lastMove != null &&
                    lastMove.MovedPiece.Type == PieceType.Pawn &&
                    Math.Abs(lastMove.To.X - lastMove.From.X) == 2 &&
                    lastMove.To.IsEqual(pawnPosition) &&
                    board.AreEnemies(currentPosition, pawnPosition))
                {
                    Vector enPassantTarget = new Vector(lastMove.To.X + (piece.IsWhite ? -1 : 1), lastMove.To.Y);
                    if (board.WithinBounds(enPassantTarget))
                    {
                        possibleMoves.Add(new PossibleMove(enPassantTarget, MoveType.EnPassant));
                    }
                }
            }
        }

    }
}
