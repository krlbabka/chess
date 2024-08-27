using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class KingStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            int distanceX = Math.Abs(from.X - to.X);
            int distanceY = Math.Abs(from.Y - to.Y);
            return distanceX <= 1 && distanceY <= 1;
        }
    }
}