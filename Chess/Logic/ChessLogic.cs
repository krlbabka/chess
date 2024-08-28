using Chess.HelperClasses;
using Chess.Pieces;
using System.Diagnostics;

namespace Chess.Logic
{
    internal class ChessLogic
    {
        private readonly Board board;
        private bool whiteTurn;

        private List<Move> Moves = new List<Move>();
        private Move lastMove;

        private int NoCaptureCounter;
        private int WhiteMaterial;
        private int BlackMaterial;

        public ChessLogic(Board board)
        {
            this.board = board;
            whiteTurn = true;
            NoCaptureCounter = 0;
            WhiteMaterial = 0;
            BlackMaterial = 0;
        }

        public void setLastMove(Move move) => lastMove = move;
        internal bool IsWhiteTurn() => whiteTurn;
        internal void SwitchTurn() => whiteTurn = !whiteTurn;
        internal int GetMaterialDifference()
        {
            return WhiteMaterial - BlackMaterial;
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
                NewTile.SetOccupyingPiece(CurrentPiece);
                CurrentTile.SetEmpty();
                if (!simulatingMove)
                {
                    if (isCapture || isPawnMove)
                    {
                        NoCaptureCounter = 0;
                    }
                    else
                    {
                        NoCaptureCounter++;
                    }

                    if (isPawnMove || isKingMove || isRookMove)
                    {
                        CurrentPiece.PieceMoved();
                    }
                    Moves.Add(move);
                    setLastMove(move);

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
                    // Move the rook from a1 to d1 (for white) or a8 to d8 (for black)
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 0);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 3);

                    // Move the rook to its new position
                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
                else if (isKingSide)
                {
                    // Move the rook from h1 to f1 (for white) or h8 to f8 (for black)
                    Vector rookOriginalPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 7);
                    Vector rookTargetPosition = new Vector(KingPiece.IsWhite ? 7 : 0, 5);

                    // Move the rook to its new position
                    chessboard.BoardGrid[rookTargetPosition.X, rookTargetPosition.Y].SetOccupyingPiece(
                        chessboard.GetPieceAt(rookOriginalPosition));
                    chessboard.BoardGrid[rookOriginalPosition.X, rookOriginalPosition.Y].SetEmpty();
                }
            }
        }

        internal bool CanCastle(Board chessboard, bool white, bool queenSide)
        {
            return false;
            int row = white ? 7 : 0;
            Tile kingTile = chessboard.BoardGrid[row, 4];
            Tile rookTile = chessboard.BoardGrid[row, queenSide ? 0 : 7];

            bool canKingCastle = chessboard.IsTileOccupied(kingTile) &&
                   chessboard.GetPieceAt(kingTile).Type == PieceType.King &&
                   !chessboard.GetPieceAt(kingTile).HasMoved();

            bool canRookCastle = chessboard.IsTileOccupied(rookTile) &&
                   chessboard.GetPieceAt(rookTile).Type == PieceType.Rook &&
                   !chessboard.GetPieceAt(rookTile).HasMoved();

            if (!canKingCastle || !canRookCastle)
                return false;

            if (queenSide)
                return CanCastleQueenSide(chessboard, white, row);

            return CanCastleKingSide(chessboard, white, row);
        }

        private bool CanCastleQueenSide(Board board, bool white, int row)
        {
            return !IsCheck(board, white) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 1]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 2]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 3]) &&
                   !IsKingUnderThreatOnPath(board, white, new Vector(row, 2));
        }

        private bool CanCastleKingSide(Board board, bool white, int row)
        {
            return !IsCheck(board, white) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 5]) &&
                   !board.IsTileOccupied(board.BoardGrid[row, 6]) &&
                   !IsKingUnderThreatOnPath(board, white, new Vector(row, 5));
        }
        private bool IsKingUnderThreatOnPath(Board board, bool white, Vector position)
        {
            return PotentialCheckAfterMove(new Vector(white ? 7 : 0, 4), position);
        }

        internal void FindLegalTiles(Board board, Tile CurrentTile, Piece ChessPiece)
        {
            board.ResetLegalMoves();

            List<PossibleMove>? possibleMoves = GetPossibleMoves(board, ChessPiece, CurrentTile.Position);

            foreach (PossibleMove possibleMove in possibleMoves)
            {
                Vector tileVector = possibleMove.vector;
                if (board.WithinBounds(tileVector))
                {
                    Tile tile = board.BoardGrid[tileVector.X, tileVector.Y];
                    if (board.IsTileOccupied(tileVector) && !board.AreEnemies(CurrentTile.Position, tileVector))
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
                if (piece.CanMove(board, currentPosition, tile.Position))
                {
                    possibleMoves.Add(new PossibleMove(tile.Position));
                    
                }
            }
            return possibleMoves;
            /*
            if (piece.Type == PieceType.King)
            {
                bool canCastleQueenSide = CanCastle(board, piece.IsWhite, true);
                bool canCastleKingSide = CanCastle(board, piece.IsWhite, false);
                if (canCastleQueenSide)
                {
                    Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 2);
                    possibleMoves.Add(new PossibleMove(kingTargetPosition, MoveType.Castling));
                }
                if (canCastleKingSide)
                {
                    Vector kingTargetPosition = new Vector(piece.IsWhite ? 7 : 0, 6);
                    possibleMoves.Add(new PossibleMove(kingTargetPosition, MoveType.Castling));
                }
            }
            */
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
            foreach(Tile tile in board.BoardGrid)
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
            return Repetition() || FiftyMoveRule();
        }

        private bool Repetition()
        {
            //save a list of chessboard states, if a pawn is pushed / piece taken -> I can delete all prior board states
            return false;
        }

        private bool FiftyMoveRule()
        {
            // 50 turns -> 100 moves
            return NoCaptureCounter >= 100;
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
    }
}
