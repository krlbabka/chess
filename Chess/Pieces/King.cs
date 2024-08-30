using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class King : Piece
    {
        private bool Moved;
        const string sourceWhite = "Resources/w_king.png";
        const string sourceBlack = "Resources/b_king.png";
        public King(bool isWhite) : base(PieceType.King, isWhite, new KingStrategy(), isWhite ? sourceWhite : sourceBlack)
        {
            Moved = false;
        }
        public override char Notation => 'K';
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
    }
}
