using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Knight : Piece
    {
        const string sourceWhite = "Resources/w_knight.png";
        const string sourceBlack = "Resources/b_knight.png";
        public Knight(bool isWhite) : base(PieceType.Knight, isWhite, new KnightStrategy(), isWhite ? sourceWhite : sourceBlack) { }
        public override char Notation => 'N';
        public override int MaterialValue => 3;
    }
}
