using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class PawnStrategy : IMovable
    {
        private Move lastMove;
        public bool CanMove(Board board, Vector from, Vector to)
        {
            Piece piece = board.GetPieceAt(from);
            int direction = piece.IsWhite ? -1 : 1;
            bool firstStraightMove = from.Y == to.Y && to.X == from.X + direction;
            bool secondStraightMove = from.Y == to.Y && to.X == from.X + direction * 2;

            if ((firstStraightMove || (secondStraightMove && !piece.HasMoved())) && !board.IsTileOccupied(to))
            {
                return true;
            }
            if (AddPawnCaptureMoves(board, from, to, direction))
            {
                return true;
            }
            if (AddEnPassantMoves(board, piece, from, to))
            {
                return true;
            }
            return false;
        }

        public void setLastMove(Move move) 
        {
            lastMove = move;
        }

        private bool AddPawnCaptureMoves(Board board, Vector currentPosition, Vector targetPosition, int direction)
        {
            Vector[] captureVectors = { new Vector(currentPosition.X + direction, currentPosition.Y -1), 
                                        new Vector(currentPosition.X + direction, currentPosition.Y + 1) };
            foreach (Vector captureVector in captureVectors)
            {
                if (board.WithinBounds(captureVector) && board.IsTileOccupied(captureVector) && board.AreEnemies(currentPosition, captureVector) && captureVector.IsEqual(targetPosition))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AddEnPassantMoves(Board board, Piece piece, Vector currentPosition, Vector targetPosition)
        {
            Vector[] enPassantVectors = { new Vector(0, -1), new Vector(0, 1) };

            foreach (Vector ePVector in enPassantVectors)
            {
                Vector pawnPosition = currentPosition + ePVector;
                if (lastMove == null)
                    continue;

                if (lastMove.MovedPiece.Type != PieceType.Pawn)
                    continue;

                if (Math.Abs(lastMove.To.X - lastMove.From.X) != 2)
                    continue;

                if (!lastMove.To.IsEqual(pawnPosition))
                    continue;

                if (!board.AreEnemies(currentPosition, pawnPosition))
                    continue;


                Vector enPassantTarget = new Vector(lastMove.To.X + (piece.IsWhite ? -1 : 1), lastMove.To.Y);

                if (!enPassantTarget.IsEqual(targetPosition))
                {
                    continue;
                }

                if (board.WithinBounds(enPassantTarget))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
