using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class King : Piece
    {
        private bool Moved;

        public King(bool isWhite) : base(PieceType.King, isWhite, new KingStrategy())
        {
            Moved = false;
        }
        public override char Notation => 'K';
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_king.png" : "b_king.png");
            return Image.FromFile(source);
        }
    }
}
