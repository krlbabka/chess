using Chess.HelperClasses;

namespace Chess.Pieces
{
    internal enum PieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    internal abstract class Piece
    {
        public PieceType Type { get; set; }
        public bool IsWhite { get; set; }
        public abstract char Notation { get; }
        public virtual int MaterialValue { get; }

        protected IMovable _movable;

        public Piece(PieceType type, bool isWhite, IMovable movable)
        {
            Type = type;
            IsWhite = isWhite;
            _movable = movable;
        }

        internal abstract Image GetPieceImage(bool isWhite);
        public bool CanMove(Board board, Vector from, Vector to)
        {
            return _movable.CanMove(board, from, to);
        }
        public virtual bool HasMoved() { return false; }
        public virtual bool PieceMoved() { return false; }
    }
}
