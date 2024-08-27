using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class BishopStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            if (Math.Abs(from.X - to.X) == Math.Abs(from.Y - to.Y))
            {
                return true;
            }
            return false;
        }
    }
}
