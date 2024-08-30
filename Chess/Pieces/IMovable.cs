using Chess.HelperClasses;
using Chess.Representation;

namespace Chess.Pieces
{
    internal interface IMovable
    {
        bool CanMove(Board board, Vector from, Vector to, out MoveType type);
    }
}
