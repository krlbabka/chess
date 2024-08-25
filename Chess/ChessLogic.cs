
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

    internal class ChessLogic
    {
        private Board board;
        private bool isWhiteTurn;

        public ChessLogic(Board board)
        {
            this.board = board;
            isWhiteTurn = true;
        }

        internal bool IsWhiteTurn() => isWhiteTurn;
        internal void SwitchTurn() => isWhiteTurn = !isWhiteTurn;

        internal bool CanMove(Vector Current, Vector New)
        {
            if (IsCheck(board, IsWhiteTurn()))
            {
                Board PotentialBoard = new Board();
                PotentialBoard.BoardGrid = board.getBoardCopy();
                MovePiece(PotentialBoard, Current, New);
                if (IsCheck(PotentialBoard, IsWhiteTurn()))
                {
                    return false;
                }
            }

            return true;
        }

        internal void MovePiece(Board chessboard, Vector Current, Vector New)
        {
            var CurrentTile = chessboard.BoardGrid[Current.X, Current.Y];
            var NewTile = chessboard.BoardGrid[New.X, New.Y];

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

        internal bool IsCheck(Board chessboard, bool whiteTurn)
        {
            return chessboard.IsKingUnderAttack(whiteTurn);
        }

        internal bool IsMate()
        {
            if (IsCheck(board, !IsWhiteTurn()))
            {
                for (int row = 0; row < board.GetBoardSize(); row++)
                {
                    for (int col = 0; col < board.GetBoardSize(); col++)
                    {
                        Tile tile = board.BoardGrid[row, col];
                        Vector currentPosition = new Vector(row, col);
                        if (tile.IsOccupied && tile.OccupyingPiece.IsWhite != IsWhiteTurn())
                        {
                            List<Vector>possibleMoves = board.GetPossibleMoves(tile.OccupyingPiece, currentPosition);
                            foreach (Vector move in possibleMoves)
                            {
                                Board PotentialBoard = new Board();
                                PotentialBoard.BoardGrid = board.getBoardCopy();
                                MovePiece(PotentialBoard, currentPosition, move);
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
            //TODO: implement
            return false;
        }
    }
}
