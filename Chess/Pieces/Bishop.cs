using Chess.HelperClasses;
using Chess.Pieces.Strategies;
using System.ComponentModel;

namespace Chess.Pieces
{
    internal class Bishop : Piece
    {
        const string sourceWhite = "../../../Resources/w_bishop.png";
        const string sourceBlack = "../../../Resources/b_bishop.png";
        public Bishop(bool isWhite) : base(PieceType.Bishop, isWhite, new BishopStrategy(), isWhite ? sourceWhite : sourceBlack) { }
        public override char Notation => 'B';
        public override int MaterialValue => 3;
    }
}
