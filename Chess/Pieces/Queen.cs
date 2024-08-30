using Chess.Pieces.Strategies;

namespace Chess.Pieces
{
    internal class Queen : Piece
    {
        const string sourceWhite = "Resources/w_queen.png";
        const string sourceBlack = "Resources/b_queen.png";
        public Queen(bool isWhite) : base(PieceType.Queen, isWhite, new QueenStrategy(), isWhite ? sourceWhite : sourceBlack) { }
        public override char Notation => 'Q';
        public override int MaterialValue => 9;
    }
}
