using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class QueenStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            return new RookStrategy().CanMove(board, from, to) || new BishopStrategy().CanMove(board, from, to);
        }
    }
}
