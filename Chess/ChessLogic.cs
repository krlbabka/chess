
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
