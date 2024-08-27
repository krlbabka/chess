using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Knight : Piece
    {
        public Knight(bool isWhite) : base(PieceType.Knight, isWhite, new KnightStrategy()) { }
        public override char Notation => 'N';
        public override int MaterialValue => 3;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_knight.png" : "b_knight.png");
            return Image.FromFile(source);
        }
    }
}
