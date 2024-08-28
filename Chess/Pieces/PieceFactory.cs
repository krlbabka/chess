using Chess.Logic;

namespace Chess.Pieces
{
    internal static class PieceFactory
    {
        public static Piece CreatePiece(PieceType type, bool isWhite)
        {
            return type switch
            {
                PieceType.Rook => new Rook(isWhite),
                PieceType.Knight => new Knight(isWhite),
                PieceType.Bishop => new Bishop(isWhite),
                PieceType.Queen => new Queen(isWhite),
                PieceType.King => new King(isWhite),
                PieceType.Pawn => new Pawn(isWhite),
                _ => throw new NotImplementedException()
            };
        }
    }
}
