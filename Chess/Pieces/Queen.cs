using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Queen : Piece
    {
        public Queen(bool isWhite) : base(PieceType.Queen, isWhite, new QueenStrategy()) { }
        public override char Notation => 'Q';
        public override int MaterialValue => 9;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_queen.png" : "b_queen.png");
            return Image.FromFile(source);
        }
    }
}
