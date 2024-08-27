using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class RookStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            if (from.X == to.X || from.Y == to.Y)
            {
                return true;
            }
            return false;
        }
    }
}
