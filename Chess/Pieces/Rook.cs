using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Rook : Piece
    {
        private bool Moved;

        public Rook(bool isWhite) : base(PieceType.Rook, isWhite, new RookStrategy())
        {
            Moved = false;
        }
        public override char Notation => 'R';
        public override int MaterialValue => 5;
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_rook.png" : "b_rook.png");
            return Image.FromFile(source);
        }
    }
}
