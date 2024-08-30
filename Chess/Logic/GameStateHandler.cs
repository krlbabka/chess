
using Chess.HelperClasses;
using Chess.Pieces;
using System.Text;

namespace Chess.Logic
{
    internal class GameStateHandler
    {

        public event Action<Vector, Vector, Action<bool>>? OnPotentialCheck;
        public event Action<Board, Piece, Vector, Action<List<Move>>>? OnPossibleMovesRequest;

        private Dictionary<string, int> _boardStateCounts = new();
        private int _fiftyMoveCounter;

        private List<Move> GetPossibleMoves(Board board, Piece piece, Vector position)
        {
            List<Move> possibleMoves = new();
            OnPossibleMovesRequest?.Invoke(board, piece, position, (moves) =>
            {
                possibleMoves = moves;
            });
            return possibleMoves;
        }

        private bool PotentialCheck(Vector from, Vector to) 
        {
            bool check = false;
            OnPotentialCheck?.Invoke(from, to, (isCheck) =>
            {
                check = isCheck;
            });
            return check;
        }

        internal bool IsCheck(Board board, bool whiteTurn)
        {
            Vector kingPosition = FindKingPosition(board, whiteTurn);
            return IsKingUnderAttack(board, whiteTurn, kingPosition);
        }

        internal bool IsMate(Board board, bool whiteTurn)
        {
            return IsMateOrStalemate(board, whiteTurn, true);
        }

        internal bool IsStalemate(Board board, bool whiteTurn)
        {
            return IsMateOrStalemate(board, whiteTurn, false);
        }

        private bool IsMateOrStalemate(Board board, bool whiteTurn, bool check)
        {
            if (IsCheck(board, whiteTurn) != check)
            {
                return false;
            }
            return NoMoveLeft(board, whiteTurn);
        }

        private bool NoMoveLeft(Board board, bool whiteTurn)
        {
            foreach (Tile tile in board.BoardGrid)
            {
                if (tile.IsOccupied && tile.OccupyingPiece!.IsWhite == whiteTurn)
                {
                    if (!CanMoveWithoutCheck(board, tile)) return false;
                }
            }
            return true;
        }

        private bool CanMoveWithoutCheck(Board board, Tile tile) 
        {
            List<Move> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece!, tile.Position);
            foreach (Move move in possibleMoves)
            {
                if (!PotentialCheck(tile.Position, move.To))
                {
                    return false;
                }
            }
            return true;
        }

        internal bool IsDraw(Board board)
        {
            return Repetition() || FiftyMoveRule() || IsInsufficientMaterial(board);
        }

        private bool Repetition()
        {
            foreach (var count in _boardStateCounts.Values)
            {
                if (count >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        private bool FiftyMoveRule()
        {
            // 50 turns -> 100 moves
            return _fiftyMoveCounter >= 100;
        }

        internal bool IsKingUnderAttack(Board board, bool isKingWhite, Vector kingPosition)
        {
            foreach (Tile tile in board.BoardGrid)
            {
                if (tile.IsOccupied && board.GetPieceAt(tile)!.IsWhite != isKingWhite)
                {
                    if (CanAttackKing(board, tile, kingPosition)) return true;
                }
            }
            return false;
        }

        private bool CanAttackKing(Board board, Tile tile, Vector kingPosition)
        {
            List<Move> possibleMoves = GetPossibleMoves(board, board.GetPieceAt(tile)!, tile.Position);
            foreach (Move move in possibleMoves)
            {
                if (move.To.IsEqual(kingPosition))
                {
                    return true;
                }
            }
            return false;
        }

        internal Vector FindKingPosition(Board board, bool isKingWhite)
        {
            foreach (Tile tile in board.BoardGrid)
            {
                bool OccupiedByKing = tile.IsOccupied && tile.OccupyingPiece!.Type == PieceType.King;
                if (OccupiedByKing && tile.OccupyingPiece!.IsWhite == isKingWhite)
                {
                    return tile.Position;
                }
            }
            throw new InvalidOperationException("King not found on the board.");
        }

        private string GenerateBoardString(Board board)
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Tile tile = board.BoardGrid[row, col];
                    if (tile.IsOccupied)
                    {
                        Piece piece = tile.OccupyingPiece!;
                        sb.Append(piece.IsWhite ? 'w' : 'b');
                        sb.Append(piece.Notation);
                    }
                    else
                    {
                        sb.Append("-");
                    }
                }
            }
            return sb.ToString();
        }

        public void SaveBoardState(Board board)
        {
            string boardState = GenerateBoardString(board);
            if (_boardStateCounts.ContainsKey(boardState))
            {
                _boardStateCounts[boardState]++;
            }
            else
            {
                _boardStateCounts[boardState] = 1;
            }
        }

        internal bool IsInsufficientMaterial(Board board)
        {
            string boardString = GenerateBoardString(board);

            int kingCount = boardString.Count(c => c == 'K');
            int queenCount = boardString.Count(c => c == 'Q');
            int rookCount = boardString.Count(c => c == 'R');
            int pawnCount = boardString.Count(c => c == 'P');

            int whiteBishopCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'w');
            int blackBishopCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'b');
            int bishopCount = whiteBishopCount + blackBishopCount;

            int whiteKnightCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'w');
            int blackKnightCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'b');
            int knightCount = whiteKnightCount + blackKnightCount;

            bool InsufficiencyBase = kingCount == 2 && queenCount == 0 && rookCount == 0 && pawnCount == 0;

            // King vs King
            if (InsufficiencyBase && bishopCount == 0 && knightCount == 0)
            {
                return true;
            }

            // King vs King & Bishop    ||    King vs King & Knight
            if (InsufficiencyBase && (bishopCount == 1 || knightCount == 1))
            {
                return true;
            }

            // King & Bishop    ||    King & Bishop    -    Stricter Chess.com rules to not worry about the tile color bishops are using
            if (InsufficiencyBase && whiteBishopCount == 1 && blackBishopCount == 1)
            {
                return true;
            }

            return false;
        }
    }
}