using Chess.HelperClasses;
using Chess.Pieces.Strategies;

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
        public bool IsPromotedPawn { get; set; }

        protected IMovable _movable;
        public Image PieceImage { get; }

        public Piece(PieceType type, bool isWhite, IMovable movable, string imagePath)
        {
            Type = type;
            IsWhite = isWhite;
            _movable = movable;
            PieceImage = GetPieceImage(imagePath);
        }

        internal static Image GetPieceImage(string path)
        {
            return Image.FromFile(path);
        }
        public bool CanMove(Board board, Vector from, Vector to, out MoveType type)
        {
            return _movable.CanMove(board, from, to, out type);
        }
        public virtual bool HasMoved() { return false; }
        public virtual bool PieceMoved() { return false; }
        public void SetLastMove(Move move)
        {
            if (_movable is PawnStrategy x)
            {
                x.setLastMove(move);
            }
        }
    }
}
