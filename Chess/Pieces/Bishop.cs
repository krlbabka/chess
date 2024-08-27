using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Bishop : Piece
    {
        public Bishop(bool isWhite) : base(PieceType.Bishop, isWhite, new BishopStrategy()) { }
        public override char Notation => 'B';
        public override int MaterialValue => 3;
        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_bishop.png" : "b_bishop.png");
            return Image.FromFile(source);
        }
    }
}
