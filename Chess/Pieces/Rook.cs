using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Rook : Piece
    {
        private bool Moved;
        const string sourceWhite = "Resources/w_rook.png";
        const string sourceBlack = "Resources/b_rook.png";
        public Rook(bool isWhite) : base(PieceType.Rook, isWhite, new RookStrategy(), isWhite ? sourceWhite : sourceBlack)
        {
            Moved = false;
        }
        public override char Notation => 'R';
        public override int MaterialValue => 5;
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
    }
}
