using Chess.HelperClasses;
using Chess.Logic;

namespace Chess.Pieces.Strategies
{
    internal class KingStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to, out MoveType type)
        {
            type = MoveType.Normal;
            int distanceX = Math.Abs(from.X - to.X);
            int distanceY = Math.Abs(from.Y - to.Y);
            bool canMoveToTile = board.GetPieceAt(to) == null || board.AreEnemies(from, to);
            bool Castle = CanCanCastle(board, from, to);
            if (Castle)
            {
                type = MoveType.Castling;
            }
            return (distanceX <= 1 && distanceY <= 1 && canMoveToTile) || Castle;
        }
        
        internal bool CanCastle(Board chessboard, bool white, bool queenSide)
        {
            int row = white ? 7 : 0;
            Tile kingTile = chessboard.BoardGrid[row, 4];
            Tile rookTile = chessboard.BoardGrid[row, queenSide ? 0 : 7];

            bool canKingCastle = chessboard.IsTileOccupied(kingTile) &&
                   chessboard.GetPieceAt(kingTile)!.Type == PieceType.King &&
                   !chessboard.GetPieceAt(kingTile)!.HasMoved();

            bool canRookCastle = chessboard.IsTileOccupied(rookTile) &&
                   chessboard.GetPieceAt(rookTile)!.Type == PieceType.Rook &&
                   !chessboard.GetPieceAt(rookTile)!.HasMoved();

            if (!canKingCastle || !canRookCastle)
                return false;

            if (queenSide)
                return CanCastleQueenSide(chessboard, white, row);

            return CanCastleKingSide(chessboard, white, row);
        }

        private bool CanCastleQueenSide(Board board, bool white, int row)
        {
            return !board.IsTileOccupied(board.BoardGrid[row, 1]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 2]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 3]);
        }

        private bool CanCastleKingSide(Board board, bool white, int row)
        {
            return !board.IsTileOccupied(board.BoardGrid[row, 5]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 6]);
        }

        private bool CanCanCastle(Board board, Vector from, Vector to)
        {
            Piece piece = board.GetPieceAt(from)!;
            bool canCastleQueenSide = CanCastle(board, piece.IsWhite, true);
            bool canCastleKingSide = CanCastle(board, piece.IsWhite, false);

            if (canCastleQueenSide)
            {
                Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 2);
                if (kingTargetPosition.IsEqual(to))
                {
                    return true;
                }
            }
            if (canCastleKingSide)
            {
                Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 6);
                if (kingTargetPosition.IsEqual(to))
                {
                    return true;
                }
            }
            return false;
        }
    }
}