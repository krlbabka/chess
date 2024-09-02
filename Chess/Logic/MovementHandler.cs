using Chess.HelperClasses;
using Chess.Pieces;
using Chess.Representation;

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

        /// <summary>
        /// Calls the ChessLogic's PotentialCheckAfterMove method.
        /// </summary>
        internal bool CanMove(Vector CurrentPosition, Vector NewPosition)
        {
            bool check = false;
            OnPotentialCheck?.Invoke(CurrentPosition, NewPosition, (isCheck) =>
            {
                check = !isCheck;
            });
            return check;
        }

        /// <summary>
        /// Moves the piece from starting Tile to the ending Tile, handles taking pieces.
        /// </summary>
        internal void MovePiece(Board board, Vector currentPosition, Vector newPosition, bool simulatingMove = false)
        {
            Tile CurrentTile = board.BoardGrid[currentPosition.X, currentPosition.Y];
            Tile NewTile = board.BoardGrid[newPosition.X, newPosition.Y];

            // No piece.
            if (board.GetPieceAt(CurrentTile) == null)
            {
                return;
            }

            Piece CurrentPiece = board.GetPieceAt(CurrentTile)!;
            bool isCapture = board.IsTileOccupied(NewTile);
            bool enPassantCapture = NewTile.MoveType == MoveType.EnPassant;

            // Update the piece that is to be taken.
            Piece? capturedPiece = null;
            if (isCapture)
            {
                capturedPiece = board.GetPieceAt(NewTile);
            }
            else if (enPassantCapture)
            {
                capturedPiece = board.GetPieceAt(new Vector(currentPosition.X, NewTile.Position.Y));

            }

            // Updating values for the board.
            HandleTakenPieces(simulatingMove, isCapture, NewTile.OccupyingPiece!);
            UpdateTilesOnMove(CurrentTile, NewTile);

            // Handle the rest of the updates.
            if (!simulatingMove)
            {
                bool PawnMove = CurrentPiece.Type == PieceType.Pawn;
                bool KingMove = CurrentPiece.Type == PieceType.King;
                bool RookMove = CurrentPiece.Type == PieceType.Rook;

                UpdateDrawVariables(isCapture, PawnMove);

                // Saves the current board state.
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

        /// <summary>
        /// Checks the piece value and updates the current difference between players.
        /// </summary>
        private void UpdateMaterialDifference(bool isCapture, Tile tile, Piece? capturedPiece)
        {
            if (isCapture)
            {
                int pieceValue = capturedPiece!.MaterialValue;

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
                    _materialDifference -= (tile.OccupyingPiece!.MaterialValue - 1);
                }
                else if (tile.Position.X == 7)
                {
                    _materialDifference += tile.OccupyingPiece!.MaterialValue - 1;
                }
            }
            if (tile.MoveType == MoveType.EnPassant)
            {
                _materialDifference += capturedPiece!.IsWhite ? 1 : -1;
            }
        }

        /// <summary>
        /// Updates the variables that are checked for a draw.
        /// </summary>
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

        /// <summary>
        /// Adds the move to a list of moves and sets the last move.
        /// </summary>
        private void UpdateMoves(Move move, bool isCapture, Tile tile)
        {
            _moves.Add(move);
            move.isCapture = isCapture;
            move.moveType = tile.MoveType;
            _lastMove = move;
            OnLastMoveChange?.Invoke(move);
        }

        /// <summary>
        /// Updates the lists of taken pieces.
        /// </summary>
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

        /// <summary>
        /// Helper method for calling respective special move methods. 
        /// </summary>
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

        /// <summary>
        /// Updates the board as per enPassant move.
        /// </summary>
        private void HandleEnPassant(Board board, Tile tile) 
        {
            Vector aboveOrBelowNewTile = board.GetPieceAt(tile)!.IsWhite ? new Vector(1, 0) : new Vector(-1, 0);
            Vector pieceToDeletePosition = tile.Position + aboveOrBelowNewTile;
            if (board.GetTile(pieceToDeletePosition).OccupyingPiece.IsWhite)
            {
                _whiteTakenPieces.Add(board.GetPieceAt(pieceToDeletePosition)!);
            }
            else 
            {
                _blackTakenPieces.Add(board.GetPieceAt(pieceToDeletePosition)!);
            }
            board.GetTile(pieceToDeletePosition).RemoveCurrentPiece();
        }

        /// <summary>
        /// Updates the board as per castling move.
        /// </summary>
        private void HandleCastling(Board board, Tile tile)
        {
            Piece KingPiece = board.GetPieceAt(tile)!;
            bool isQueenSide = tile.Position.Y == 2;
            bool isKingSide = tile.Position.Y == 6;

            if (isQueenSide)
            {
                // Queen side the rook moves by three to the right.
                HandleCastlingRookMovement(board, KingPiece.IsWhite, 0, 3);
            }
            else if (isKingSide)
            {
                // King side the rook moves by two to the left.
                HandleCastlingRookMovement(board, KingPiece.IsWhite, 7, 5);
            }
        }

        // Helper method to move the rook
        private void HandleCastlingRookMovement(Board board, bool isWhite, int OriginalFile, int TargetFile) 
        {
            int Rank = isWhite ? 7 : 0;
            Tile rookTile = board.GetTile(new Vector(Rank, OriginalFile));
            Tile rookTileTarget = board.GetTile(new Vector(Rank, TargetFile));

            UpdateTilesOnMove(rookTile, rookTileTarget);
        }

        /// <summary>
        /// Updates the promoting piece.
        /// </summary>
        private void HandlePromotion(Board board, Tile tile)
        {
            bool isWhite = board.GetPieceAt(tile)!.IsWhite;
            if (isWhite)
            {
                _whiteTakenPieces.Add(board.GetPieceAt(tile)!);
            }
            else
            {
                _blackTakenPieces.Add(board.GetPieceAt(tile)!);
            }
            OnPromotion?.Invoke(isWhite, (pieceType) =>
            {
                tile.RemoveCurrentPiece();
                tile.CreatePiece(pieceType, isWhite);
                tile.OccupyingPiece!.IsPromotedPawn = true;
            });
        }

        /// <summary>
        /// Finds all possible moves for given tile & piece and updates board so that all legal Tiles are marked as such.
        /// </summary>
        internal void FindLegalTiles(Board board, Tile CurrentTile, Piece ChessPiece)
        {
            board.ResetLegalMoves();
            Vector position = CurrentTile.Position;
            List<Move>? possibleMoves = GetPossibleMoves(board, ChessPiece, position);
            foreach (Move possibleMove in possibleMoves)
            {
                if (possibleMove.moveType == MoveType.Castling)
                {
                    if (PotentialCastleCheck(board, position, possibleMove.To))
                    {
                        continue;
                    }
                }
                Vector tileVector = possibleMove.To;
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

        // Helper method with checks for a tile.
        private bool TileNotValid(Board board, Vector position, Vector tileVector)
        {
            bool OccupiedByAlliedPiece = board.IsTileOccupied(tileVector) && !board.AreEnemies(position, tileVector);

            if (OccupiedByAlliedPiece || !CanMove(position, tileVector)) return true;

            return false;
        }

        /// <summary>
        /// Checks if the king is trying to castle from a check or trying to go through a check during castling.
        /// </summary>
        private bool PotentialCastleCheck(Board board, Vector from, Vector to)
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

        /// <summary>
        /// Iterates over all tiles and looks if the current piece can move there.
        /// </summary>
        internal List<Move> GetPossibleMoves(Board board, Piece piece, Vector currentPosition)
        {
            List<Move> possibleMoves = new List<Move>();
            
            if (piece.Type == PieceType.Pawn) piece.SetLastMove(_lastMove!);

            MoveType type;

            foreach (Tile tile in board.BoardGrid)
            {
                if (piece.CanMove(board, currentPosition, tile.Position, out type))
                {
                    Move move = new Move(currentPosition, tile.Position, piece);
                    possibleMoves.Add(move);
                    move.moveType = type;

                }
            }
            return possibleMoves;
        }

        // Method override calling the original method for access from ChessLogic class.
        internal void GetPossibleMoves(Board board, Piece piece, Vector position, Action<List<Move>> possibleMoves) 
        {
            possibleMoves?.Invoke(GetPossibleMoves(board, piece, position));
        }

        /// <summary>
        /// Adds the piece to the new Tile and removes it from the old one, sets corresponding Tile values.
        /// </summary>
        private void UpdateTilesOnMove(Tile from, Tile to) 
        {
            to.SetNewPiece(from.OccupyingPiece!);
            from.RemoveCurrentPiece();
        }
    }
}