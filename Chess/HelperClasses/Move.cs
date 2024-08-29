using Chess.Pieces;

namespace Chess.HelperClasses
{
    internal class Move
    {
        public Vector From { get; set; }
        public Vector To { get; set; }
        public Piece MovedPiece { get; set; }
        public bool isCapture { get; set; }
        public MoveType moveType { get; set; }
        public Move(Vector from, Vector to, Piece piece)
        {
            From = from;
            To = to;
            MovedPiece = piece;
        }

        public bool IsEqual(Move other)
        {
            return From.IsEqual(other.From) && To.IsEqual(other.To);
        }
    }
}
