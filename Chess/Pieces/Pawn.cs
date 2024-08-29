using Chess.Pieces.Strategies;
using System.Threading.Tasks.Dataflow;

namespace Chess.Pieces
{
    internal class Pawn : Piece
    {
        private bool Moved;
        const string sourceWhite = "../../../Resources/w_pawn.png";
        const string sourceBlack = "../../../Resources/b_pawn.png";
        public Pawn(bool isWhite) : base(PieceType.Pawn, isWhite, new PawnStrategy(), isWhite ? sourceWhite : sourceBlack)
        {
            Moved = false;
        }

        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
        public override char Notation => ' ';
        public override int MaterialValue => 1;
    }
}
