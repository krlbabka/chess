using Chess.HelperClasses;
using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Pawn : Piece
    {
        private bool Moved;

        public Pawn(bool isWhite) : base(PieceType.Pawn, isWhite, new PawnStrategy())
        {
            Moved = false;
        }

        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
        public override char Notation => ' ';
        public override int MaterialValue => 1;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_pawn.png" : "b_pawn.png");
            return Image.FromFile(source);
        }
    }
}
