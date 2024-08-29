using Chess.HelperClasses;
using Chess.Pieces;
using System.Text;

namespace Chess.Logic
{
    internal class ChessLogic
    {
        private readonly Board board;
        private bool whiteTurn;

        private List<Move> Moves;
        private Move lastMove;

        private Dictionary<string, int> boardStateCounts;

        private int FiftyMoveCounter;
        private List<Piece> WhiteTakenPieces;
        private List<Piece> BlackTakenPieces;

        public event Action<bool, Action<PieceType>> OnPromotion;

        public ChessLogic(Board board)
        {
            this.board = board;
            whiteTurn = true;
            FiftyMoveCounter = 0;
            Moves = new List<Move>();
            boardStateCounts = new Dictionary<string, int>();
            WhiteTakenPieces = new List<Piece>();
            BlackTakenPieces = new List<Piece>();
        }

        public void setLastMove(Move move) => lastMove = move;
        internal bool IsWhiteTurn() => whiteTurn;
        internal void SwitchTurn() => whiteTurn = !whiteTurn;
        internal int GetMaterialDifference()
        {
            int whiteValue = BlackTakenPieces.Sum(piece => piece.MaterialValue);
            int blackValue = WhiteTakenPieces.Sum(piece => piece.MaterialValue);
            return whiteValue - blackValue;
        }

        internal int GetMaterialValue(bool isWhite)
        {
            return isWhite ? WhiteTakenPieces.Sum(piece => piece.MaterialValue) : BlackTakenPieces.Sum(piece => piece.MaterialValue);
        }

        #region Movement

        internal bool CanMove(Vector CurrentPosition, Vector NewPosition)
        {
            return !(PotentialCheckAfterMove(CurrentPosition, NewPosition));
        }

        internal void MovePiece(Board chessboard, Vector CurrentPosition, Vector NewPosition, bool simulatingMove = false)
        {
            Tile CurrentTile = chessboard.BoardGrid[CurrentPosition.X, CurrentPosition.Y];
            Tile NewTile = chessboard.BoardGrid[NewPosition.X, NewPosition.Y];
            if (chessboard.GetPieceAt(CurrentTile) != null)
            {
                Piece CurrentPiece = chessboard.GetPieceAt(CurrentTile)!;
                Move move = new(CurrentPosition, NewPosition, CurrentPiece);

                bool isCapture = board.IsTileOccupied(NewTile);
                bool isPawnMove = CurrentPiece.Type == PieceType.Pawn;
                bool isKingMove = CurrentPiece.Type == PieceType.King;
                bool isRookMove = CurrentPiece.Type == PieceType.Rook;

                if (!simulatingMove && isCapture)
                {
                    if (!NewTile.OccupyingPiece.IsPromotedPawn)
                    {
                        if (NewTile.OccupyingPiece!.IsWhite)
                        {
                            WhiteTakenPieces.Add(NewTile.OccupyingPiece);
                        }
                        else
                        {
                            BlackTakenPieces.Add(NewTile.OccupyingPiece);
                        }
                    }
                }

                NewTile.SetOccupyingPiece(CurrentPiece);
                CurrentTile.SetEmpty();

                if (!simulatingMove)
                {
                    
                    if (isCapture || isPawnMove)
                    {
                        FiftyMoveCounter = 0;
                        boardStateCounts.Clear();
                    }
                    else
                    {
                        FiftyMoveCounter++;
                    }

                    SaveBoardState();

                    if (isPawnMove || isKingMove || isRookMove)
                    {
                        CurrentPiece.PieceMoved();
                    }
                    Moves.Add(move);
                    setLastMove(move);
                    lastMove.isCapture = isCapture;
                    lastMove.moveType = NewTile.MoveType;

                    HandleSpecialMoveTypes(chessboard, CurrentTile, NewTile);
                }
            }
        }

        private void HandleSpecialMoveTypes(Board chessboard, Tile currentTile, Tile newTile)
        {
            if (newTile.MoveType == MoveType.EnPassant)
            {
                Vector aboveOrBelowNewTile = chessboard.GetPieceAt(newTile).IsWhite ? new Vector(1, 0) : new Vector(-1, 0);
                Vector pieceToDeletePosition = newTile.Position + aboveOrBelowNewTile;
                chessboard.BoardGrid[pieceToDeletePosition.X, pieceToDeletePosition.Y].SetEmpty();
            }

            if (newTile.MoveType == MoveType.Castling)
            {
                Piece KingPiece = chessboard.GetPieceAt(newTile)!;
                bool isQueenSide = newTile.Position.Y == 2;
                bool isKingSide = newTile.Position.Y == 6;

                if (isQueenSide)
                {
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 0);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 3);

                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
                else if (isKingSide)
                {
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 7);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 5);

                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
            }
            if (newTile.MoveType == MoveType.Promotion)
            {
                bool isWhite = board.GetPieceAt(newTile).IsWhite;
                OnPromotion?.Invoke(board.GetPieceAt(newTile).IsWhite, (pieceType) => 
                {
                    newTile.SetEmpty();
                    newTile.AddPieceToTile(pieceType, isWhite);
                    newTile.OccupyingPiece!.IsPromotedPawn = true;
                });
            }
        }

        internal void FindLegalTiles(Board board, Tile CurrentTile, Piece ChessPiece)
        {
            board.ResetLegalMoves();

            List<PossibleMove>? possibleMoves = GetPossibleMoves(board, ChessPiece, CurrentTile.Position);
            //Castle checks
            foreach (PossibleMove possibleMove in possibleMoves)
            {
                if (possibleMove.moveType == MoveType.Castling)
                {
                    if (IsCheck(board, IsWhiteTurn()))
                    {
                        continue;
                    }
                    int direction = (possibleMove.vector.Y - CurrentTile.Position.Y) > 0 ? 1 : -1;
                    Vector possibleCheckTile = CurrentTile.Position + new Vector(0, direction);
                    if (PotentialCheckAfterMove(CurrentTile.Position, possibleCheckTile))
                    {
                        continue;
                    }
                }
                Vector tileVector = possibleMove.vector;
                if (board.WithinBounds(tileVector))
                {
                    
                    Tile tile = board.BoardGrid[tileVector.X, tileVector.Y];
                    if (board.IsTileOccupied(tileVector) && !board.AreEnemies(CurrentTile.Position, tileVector))
                    {
                        continue;
                    }

                    if (PotentialCheckAfterMove(CurrentTile.Position, tileVector))
                    {
                        continue;
                    }

                    tile.LegalMove = true;
                    tile.MoveType = possibleMove.moveType;
                }
            }
        }

        internal List<PossibleMove> GetPossibleMoves(Board board, Piece piece, Vector currentPosition)
        {
            List<PossibleMove> possibleMoves = new List<PossibleMove>();
            if (piece.Type == PieceType.Pawn)
            {
                piece.SetLastMove(lastMove);
            }
            MoveType type;
            PieceType[] slidingPieces = { PieceType.Bishop, PieceType.Rook, PieceType.Queen };

            foreach (Tile tile in board.BoardGrid)
            {
                if (piece.CanMove(board, currentPosition, tile.Position, out type))
                {
                    possibleMoves.Add(new PossibleMove(tile.Position, type));

                }
            }
            return possibleMoves;
        }

        #endregion

        #region Game State Handling

        internal bool IsCheck(Board board, bool whiteTurn)
        {
            Vector kingPosition = FindKingPosition(board, whiteTurn);
            return IsKingUnderAttack(board, whiteTurn, kingPosition);
        }

        internal bool IsMate(Board board, bool whiteTurn)
        {
            if (!IsCheck(board, whiteTurn))
            {
                return false;
            }
            foreach (Tile tile in board.BoardGrid)
            {
                if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == whiteTurn)
                {
                    List<PossibleMove> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, tile.Position);
                    foreach (PossibleMove move in possibleMoves)
                    {
                        bool tryingToTakeKing = board.IsTileOccupied(move.vector) && board.GetPieceAt(move.vector).Type == PieceType.King;
                        if (!PotentialCheckAfterMove(tile.Position, move.vector) && !tryingToTakeKing)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal bool IsStalemate(Board board, bool whiteTurn)
        {
            if (IsCheck(board, whiteTurn))
            {
                return false;
            }

            foreach (Tile tile in board.BoardGrid)
            {
                Vector currentPosition = tile.Position;
                if (tile.IsOccupied && tile.OccupyingPiece.IsWhite == whiteTurn)
                {
                    List<PossibleMove> possibleMoves = GetPossibleMoves(board, tile.OccupyingPiece, currentPosition);
                    foreach (PossibleMove move in possibleMoves)
                    {
                        if (!PotentialCheckAfterMove(currentPosition, move.vector))
                        {
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        internal bool IsDraw()
        {
            return Repetition() || FiftyMoveRule() || IsInsufficientMaterial();
        }

        private bool Repetition()
        {
            foreach (var count in boardStateCounts.Values)
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
            return FiftyMoveCounter >= 100;
        }

        #endregion

        internal bool IsKingUnderAttack(Board board, bool isKingWhite, Vector kingPosition)
        {
            foreach (Tile tile in board.BoardGrid)
            {
                Vector currentPosition = tile.Position;
                if (tile.IsOccupied && board.GetPieceAt(tile).IsWhite != isKingWhite)
                {
                    List<PossibleMove> possibleMoves = GetPossibleMoves(board, board.GetPieceAt(tile), currentPosition);
                    foreach (PossibleMove move in possibleMoves)
                    {
                        if (move.vector.IsEqual(kingPosition))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal Vector FindKingPosition(Board chessboard, bool isKingWhite)
        {
            foreach (Tile tile in chessboard.BoardGrid)
            {
                if (tile.IsOccupied
                    && tile.OccupyingPiece.Type == PieceType.King
                    && tile.OccupyingPiece.IsWhite == isKingWhite)
                {
                    return tile.Position;
                }
            }
            throw new InvalidOperationException("King not found on the board.");
        }

        internal bool PotentialCheckAfterMove(Vector from, Vector to)
        {
            Board PotentialBoard = new()
            {
                BoardGrid = board.getBoardCopy()
            };

            if (PotentialBoard.IsTileOccupied(to) && PotentialBoard.GetPieceAt(to).Type == PieceType.King)
            {
                return true;
            }

            MovePiece(PotentialBoard, from, to, true);

            return IsCheck(PotentialBoard, IsWhiteTurn());
        }
        private string GenerateBoardString()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Tile tile = board.BoardGrid[row, col];
                    if (tile.IsOccupied)
                    {
                        Piece piece = tile.OccupyingPiece;
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
        public void SaveBoardState()
        {
            string boardState = GenerateBoardString();
            if (boardStateCounts.ContainsKey(boardState))
            {
                boardStateCounts[boardState]++;
            }
            else
            {
                boardStateCounts[boardState] = 1;
            }
        }

        internal bool IsInsufficientMaterial()
        {
            string boardString = GenerateBoardString();

            int kingCount = boardString.Count(c => c == 'K');
            int queenCount = boardString.Count(c => c == 'Q');
            int rookCount = boardString.Count(c => c == 'R');
            int bishopCount = boardString.Count(c => c == 'B');
            int knightCount = boardString.Count(c => c == 'N');
            int pawnCount = boardString.Count(c => c == 'P');

            int whiteBishopCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'w');
            int blackBishopCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'b');
            int whiteKnightCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'w');
            int blackKnightCount = boardString.Count(c => c == 'B' && boardString[boardString.IndexOf(c) - 1] == 'b');

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

        internal void SortTakenPiecesByValue() 
        {
            WhiteTakenPieces = WhiteTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
            BlackTakenPieces = BlackTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
        }

        internal string GetLastMoveNotation() 
        {
            if (lastMove == null)
            {
                return "";
            }
            string notation = "";
            Piece piece = lastMove.MovedPiece;

            notation += piece.Notation;
            if (lastMove.isCapture)
            {
                if (lastMove.MovedPiece.Type == PieceType.Pawn)
                {
                    notation += lastMove.From.GetFile();
                }
                notation += "x";
            }

            notation += lastMove.To.GetFile() + lastMove.To.GetRank();

            if (lastMove.moveType == MoveType.Castling)
            {
                notation = lastMove.To.Y == 2 ? "O-O-O" : "O-O";
            }
            else if (lastMove.moveType == MoveType.Promotion)
            {
                notation += $"={board.GetPieceAt(lastMove.To).Notation}";
            }
            return notation;
        }
    }
}