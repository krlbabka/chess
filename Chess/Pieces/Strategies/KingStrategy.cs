using Chess.HelperClasses;
using Chess.Logic;

namespace Chess.Pieces.Strategies
{
    internal class KingStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            int distanceX = Math.Abs(from.X - to.X);
            int distanceY = Math.Abs(from.Y - to.Y);
            bool canMoveToTile = board.GetPieceAt(to) == null || board.GetPieceAt(to).IsWhite != board.GetPieceAt(from).IsWhite;

            return distanceX <= 1 && distanceY <= 1 && canMoveToTile;
        }
    }
}