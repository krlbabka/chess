using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class KnightStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to, out MoveType type)
        {
            type = MoveType.Normal;
            int distanceX = Math.Abs(from.X - to.X);
            int distanceY = Math.Abs(from.Y - to.Y);
            bool canMoveToTile = board.GetPieceAt(to) == null || board.GetPieceAt(to).IsWhite != board.GetPieceAt(from).IsWhite;
            return ((distanceX == 2 && distanceY == 1) || (distanceX == 1 && distanceY == 2)) && canMoveToTile;
        }
    }
}
