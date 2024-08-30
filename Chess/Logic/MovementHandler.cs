using Chess.HelperClasses;
using Chess.Pieces;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Chess.Logic
{
    internal class MovementHandler
    {
        private List<Move> _moves;
        private Move? _lastMove;

        private Dictionary<string, int> _boardStateCounts;
        private int _fiftyMoveCounter;

        private int _materialDifference;
        private List<Piece> _whiteTakenPieces;
        private List<Piece> _blackTakenPieces;

        public event Action<Board>? OnBoardSaved;
        public event Action<Move>? OnLastMoveChange;
        public event Action<Vector, Vector, Action<bool>>? OnPotentialCheck;
        public event Action<bool, Action<PieceType>>? OnPromotion;
        public event Action<Board, Action<bool>>? OnCheck;

        public MovementHandler()
        {
            _moves = new List<Move>();
            _lastMove = null;
            _boardStateCounts = new Dictionary<string,int>();
            _materialDifference = 0;
            _fiftyMoveCounter = 0;
            _whiteTakenPieces = new List<Piece>();
            _blackTakenPieces = new List<Piece>();
        }

        internal int MaterialDifference => _materialDifference;
        internal List<Piece> WhiteTakenPieces => _whiteTakenPieces;
        internal List<Piece> BlackTakenPieces => _blackTakenPieces;
        internal int FiftyMoveCounter => _fiftyMoveCounter;

        internal void SortTakenPieces()
        {
            _whiteTakenPieces = _whiteTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
            _blackTakenPieces = _blackTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
        }

        internal bool CanMove(Vector CurrentPosition, Vector NewPosition)
        {
            bool check = false;
            OnPotentialCheck?.Invoke(CurrentPosition, NewPosition, (isCheck) =>
            {
                check = !isCheck;
            });
            return check;
        }

        internal void MovePiece(Board board, Vector currentPosition, Vector newPosition, bool simulatingMove = false)
        {
            Tile CurrentTile = board.BoardGrid[currentPosition.X, currentPosition.Y];
            Tile NewTile = board.BoardGrid[newPosition.X, newPosition.Y];
            if (board.GetPieceAt(CurrentTile) == null)
            {
                return;
            }
            Piece CurrentPiece = board.GetPieceAt(CurrentTile)!;
            bool isCapture = board.IsTileOccupied(NewTile);
            bool enPassantCapture = NewTile.MoveType == MoveType.EnPassant;
            Piece? capturedPiece = null;
            if (isCapture)
            {
                capturedPiece = board.GetPieceAt(NewTile);
            }
            else if (enPassantCapture)
            {
                capturedPiece = board.GetPieceAt(new Vector(currentPosition.X, NewTile.Position.Y));

            }

            HandleTakenPieces(simulatingMove, isCapture, NewTile.OccupyingPiece!);
            UpdateTilesOnMove(CurrentTile, NewTile);

            if (!simulatingMove)
            {
                bool PawnMove = CurrentPiece.Type == PieceType.Pawn;
                bool KingMove = CurrentPiece.Type == PieceType.King;
                bool RookMove = CurrentPiece.Type == PieceType.Rook;

                UpdateDrawVariables(isCapture, PawnMove);

                OnBoardSaved?.Invoke(board);

                if (PawnMove || KingMove || RookMove)
                {
                    CurrentPiece.PieceMoved();
                }
                UpdateMoves(new Move(currentPosition, newPosition, CurrentPiece), isCapture || enPassantCapture, NewTile);
                HandleSpecialMoveTypes(board, NewTile);
                
                UpdateMaterialDifference(isCapture, NewTile, capturedPiece);
            }
        }

        private void UpdateMaterialDifference(bool isCapture, Tile tile, Piece? capturedPiece)
        {
            if (isCapture)
            {
                int pieceValue = capturedPiece.MaterialValue;

                if (capturedPiece.IsWhite)
                {
                    _materialDifference += pieceValue;
                }
                else
                {
                    _materialDifference -= pieceValue;
                }
            }
            if (tile.MoveType == MoveType.Promotion)
            {
                if (tile.Position.X == 0)
                {
                    _materialDifference -= (tile.OccupyingPiece.MaterialValue - 1);
                }
                else if (tile.Position.X == 7)
                {
                    _materialDifference += (tile.OccupyingPiece.MaterialValue - 1);
                }
            }
            if (tile.MoveType == MoveType.EnPassant)
            {
                _materialDifference += capturedPiece.IsWhite ? 1 : -1;
            }
        }

        private void UpdateDrawVariables(bool capture, bool PawnMove)
        {
            if (capture || PawnMove)
            {
                _fiftyMoveCounter = 0;
                _boardStateCounts.Clear();
            }
            else
            {
                _fiftyMoveCounter++;
            }
        }

        private void UpdateMoves(Move move, bool isCapture, Tile tile)
        {
            _moves.Add(move);
            move.isCapture = isCapture;
            move.moveType = tile.MoveType;
            _lastMove = move;
            OnLastMoveChange?.Invoke(move);
        }

        private void HandleTakenPieces(bool simulation, bool isCapture, Piece piece)
        {
            if (simulation || !isCapture || piece!.IsPromotedPawn)
            {
                return;
            }
            if (piece.IsWhite)
            {
                _whiteTakenPieces.Add(piece);
            }
            else
            {
                _blackTakenPieces.Add(piece);
            }
        }

        private void HandleSpecialMoveTypes(Board board, Tile newTile)
        {
            if (newTile.MoveType == MoveType.EnPassant)
            {
                HandleEnPassant(board, newTile);
            }
            else if (newTile.MoveType == MoveType.Castling)
            {
                HandleCastling(board, newTile);
            }
            else if (newTile.MoveType == MoveType.Promotion)
            {
                HandlePromotion(board, newTile);
            }
        }

        private void HandleEnPassant(Board board, Tile tile) 
        {
            Vector aboveOrBelowNewTile = board.GetPieceAt(tile)!.IsWhite ? new Vector(1, 0) : new Vector(-1, 0);
            Vector pieceToDeletePosition = tile.Position + aboveOrBelowNewTile;
            board.BoardGrid[pieceToDeletePosition.X, pieceToDeletePosition.Y].SetEmpty();
        }

        private void HandleCastling(Board board, Tile tile) 
        {
            Piece KingPiece = board.GetPieceAt(tile)!;
            bool isQueenSide = tile.Position.Y == 2;
            bool isKingSide = tile.Position.Y == 6;

            if (isQueenSide)
            {
                // Queen side the rook moves by three to the right
                HandleCastlingRookMovement(board, KingPiece.IsWhite, 0, 3);
            }
            else if (isKingSide)
            {
                // King side the rook moves by two to the left
                HandleCastlingRookMovement(board, KingPiece.IsWhite, 7, 5);
            }
        }

        private void HandleCastlingRookMovement(Board board, bool isWhite, int OriginalFile, int TargetFile) 
        {
            int Rank = isWhite ? 7 : 0;
            Tile rookTile = board.GetTile(new Vector(Rank, OriginalFile));
            Tile rookTileTarget = board.GetTile(new Vector(Rank, TargetFile));

            UpdateTilesOnMove(rookTile, rookTileTarget);
        }

        private void HandlePromotion(Board board, Tile tile)
        {
            bool isWhite = board.GetPieceAt(tile)!.IsWhite;
            if (isWhite)
            {
                _whiteTakenPieces.Add(board.GetPieceAt(tile));
            }
            else
            {
                _blackTakenPieces.Add(board.GetPieceAt(tile));
            }
            OnPromotion?.Invoke(isWhite, (pieceType) =>
            {
                tile.SetEmpty();
                tile.AddPieceToTile(pieceType, isWhite);
                tile.OccupyingPiece!.IsPromotedPawn = true;
            });
        }

        internal void FindLegalTiles(Board board, Tile CurrentTile, Piece ChessPiece)
        {
            board.ResetLegalMoves();
            Vector position = CurrentTile.Position;
            List<PossibleMove>? possibleMoves = GetPossibleMoves(board, ChessPiece, position);
            foreach (PossibleMove possibleMove in possibleMoves)
            {
                if (possibleMove.moveType == MoveType.Castling)
                {
                    if (PotentialCastleCheck(board, position, possibleMove.vector, ChessPiece))
                    {
                        continue;
                    }
                }
                Vector tileVector = possibleMove.vector;
                if (board.WithinBounds(tileVector))
                {
                    if (TileNotValid(board, position, tileVector))
                    {
                        continue;
                    }

                    Tile tile = board.GetTile(tileVector);
                    tile.LegalMove = true;
                    tile.MoveType = possibleMove.moveType;
                }
            }
        }

        private bool TileNotValid(Board board, Vector position, Vector tileVector)
        {
            bool OccupiedByAlliedPiece = board.IsTileOccupied(tileVector) && !board.AreEnemies(position, tileVector);

            if (OccupiedByAlliedPiece || !CanMove(position, tileVector)) return true;

            return false;
        }

        private bool PotentialCastleCheck(Board board, Vector from, Vector to, Piece piece)
        {
            bool check = false;
            OnCheck?.Invoke(board, (isCheck) =>
            {
                check = isCheck;
            });
            if (check) return true;

            int direction = (to.Y - from.Y) > 0 ? 1 : -1;
            Vector possibleCheckTile = from + new Vector(0, direction);
            
            if (!CanMove(from, possibleCheckTile)) return true;

            return false;
        }

        internal List<PossibleMove> GetPossibleMoves(Board board, Piece piece, Vector currentPosition)
        {
            List<PossibleMove> possibleMoves = new List<PossibleMove>();
            
            if (piece.Type == PieceType.Pawn) piece.SetLastMove(_lastMove!);

            MoveType type;

            foreach (Tile tile in board.BoardGrid)
            {
                if (piece.CanMove(board, currentPosition, tile.Position, out type))
                {
                    possibleMoves.Add(new PossibleMove(tile.Position, type));

                }
            }
            return possibleMoves;
        }

        internal void GetPossibleMoves(Board board, Piece piece, Vector position, Action<List<PossibleMove>> possibleMoves) 
        {
            possibleMoves?.Invoke(GetPossibleMoves(board, piece, position));
        }

        private void UpdateTilesOnMove(Tile from, Tile to) 
        {
            to.SetOccupyingPiece(from.OccupyingPiece!);
            from.SetEmpty();
        }
    }
}