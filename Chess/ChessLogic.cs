
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

        private bool CanMove(Vector from, Vector to) 
        {
            Piece? piece = board.GetPieceAtPosition(from);
            if (piece == null) return false;

            if (piece.IsWhite != isWhiteTurn) return false;

            if (board.IsTileOccupied(to) && board.GetPieceAtPosition(to)?.IsWhite == isWhiteTurn) return false;

            return true;
        }

        public void MovePiece(Vector from, Vector to)
        {
            Piece? piece = board.GetPieceAtPosition(from);
            if (piece == null || piece.IsWhite != isWhiteTurn)
                return;

            if (CanMove(from, to))
            {
                board.MovePiece(from, to);
                SwitchTurn();
            }
        }

        private bool IsWhiteTurn() => isWhiteTurn;
        private void SwitchTurn() => isWhiteTurn = !isWhiteTurn;

        private bool IsCheck()
        {
            return board.IsKingUnderAttack(!IsWhiteTurn());
        }

        private bool IsMate()
        {
            if (!IsCheck()) return false;
            //TODO: implement
            return false;
        }

        private bool IsStalemate()
        {
            //TODO: implement
            return false;
        }
    }
}
