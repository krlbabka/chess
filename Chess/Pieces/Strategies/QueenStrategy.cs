using Chess.HelperClasses;
using Chess.Representation;

namespace Chess.Pieces.Strategies
{
    internal class QueenStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to, out MoveType type)
        {
            return new RookStrategy().CanMove(board, from, to, out type) || new BishopStrategy().CanMove(board, from, to, out type);
        }
    }
}
