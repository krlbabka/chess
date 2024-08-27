
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
        private Board Chessboard;
        private bool WhiteTurn;
        private List<Move> Moves = new List<Move>();
        private int NoCaptureCounter;


        public ChessLogic(Board board)
        {
            Chessboard = board;
            WhiteTurn = true;
            NoCaptureCounter = 0;
        }

        internal bool IsWhiteTurn() => WhiteTurn;
        internal void SwitchTurn() => WhiteTurn = !WhiteTurn;

        internal bool CanMove(Vector Current, Vector New)
        {
            if (IsCheck(Chessboard, IsWhiteTurn()))
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
                Chessboard.setLastMove(lastMove);
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

        internal bool IsCheck(Board chessboard, bool whiteTurn)
        {
            return chessboard.IsKingUnderAttack(whiteTurn);
        }

        internal bool IsMate()
        {
            if (IsCheck(Chessboard, !IsWhiteTurn()))
            {
                for (int row = 0; row < Chessboard.GetBoardSize(); row++)
                {
                    for (int col = 0; col < Chessboard.GetBoardSize(); col++)
                    {
                        Tile tile = Chessboard.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite != IsWhiteTurn())
                        {
                            List<PossibleMove>possibleMoves = Chessboard.GetPossibleMoves(tile.OccupyingPiece, currentPosition);
                            foreach (PossibleMove move in possibleMoves)
                            {
                                Board PotentialBoard = new Board();
                                PotentialBoard.BoardGrid = Chessboard.getBoardCopy();
                                MovePiece(PotentialBoard, currentPosition, move.vector);
                                if (!IsCheck(PotentialBoard, !IsWhiteTurn()))
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

        internal bool IsStalemate()
        {
            if (!IsCheck(Chessboard, !IsWhiteTurn()))
            {
                for (int row = 0; row < Chessboard.GetBoardSize(); row++)
                {
                    for (int col = 0; col < Chessboard.GetBoardSize(); col++)
                    {
                        Tile tile = Chessboard.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == IsWhiteTurn())
                        {
                            List<PossibleMove> possibleMoves = Chessboard.GetPossibleMoves(tile.OccupyingPiece, currentPosition);
                            foreach (PossibleMove move in possibleMoves)
                            {
                                Board PotentialBoard = new Board();
                                PotentialBoard.BoardGrid = Chessboard.getBoardCopy();
                                MovePiece(PotentialBoard, currentPosition, move.vector);
                                if (!IsCheck(PotentialBoard, IsWhiteTurn()))
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
            Tile KingTile = Chessboard.BoardGrid[row, 4];
            Tile QSideRookTile = Chessboard.BoardGrid[row, 0];
            Tile KSideRookTile = Chessboard.BoardGrid[row, 7];
            // Pieces are either not there, or are on the correct tile, but have moved
            bool KingNotEligible = !KingTile.IsOccupied || !IsPieceOnTile(KingTile, PieceType.King) ||
                (IsPieceOnTile(KingTile, PieceType.King) && KingTile.OccupyingPiece.HasMoved());

            bool QueenSideRookNotEligible = !QSideRookTile.IsOccupied || !IsPieceOnTile(QSideRookTile, PieceType.Rook) ||
                (IsPieceOnTile(QSideRookTile, PieceType.Rook) && QSideRookTile.OccupyingPiece.HasMoved());

            bool KingSideRookNotEligible = !KSideRookTile.IsOccupied || !IsPieceOnTile(KSideRookTile, PieceType.Rook) ||
                (IsPieceOnTile(KSideRookTile, PieceType.Rook) && Chessboard.BoardGrid[row, 7].OccupyingPiece.HasMoved());

            bool KingSideEmptyWay = !Chessboard.BoardGrid[row, 5].IsOccupied && !Chessboard.BoardGrid[row, 6].IsOccupied;
            bool QueenSideEmptyWay = !Chessboard.BoardGrid[row, 1].IsOccupied && !Chessboard.BoardGrid[row, 2].IsOccupied && !Chessboard.BoardGrid[row, 3].IsOccupied;

            if (KingNotEligible)
            {
                return false;
            }
            if (QueenSide)
            {
                bool KingPathCheckBool_1 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, -1));
                bool KingPathCheckBool_2 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, -2));
                if (QueenSideRookNotEligible || QueenSideEmptyWay || IsCheck(Chessboard, White) || KingPathCheckBool_1 || KingPathCheckBool_2)
                {
                    return false;
                }
            }
            else
            {
                bool KingPathCheckBool_1 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, 1));
                bool KingPathCheckBool_2 = PotentialCheckAfterMove(KingTile.Position, KingTile.Position + new Vector(0, 2));
                if (KingSideRookNotEligible || KingSideEmptyWay || IsCheck(Chessboard, White) || KingPathCheckBool_1 || KingPathCheckBool_2)
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
                BoardGrid = Chessboard.getBoardCopy()
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

        private bool IsPieceOnTile(Tile tile, PieceType type) 
        {
            return tile.OccupyingPiece.Type == type;
        }
    }
}
