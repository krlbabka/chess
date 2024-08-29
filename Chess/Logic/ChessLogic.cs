using Chess.HelperClasses;
using Chess.Pieces;

namespace Chess.Logic
{
    internal class ChessLogic
    {
        private readonly Board _board;
        private GameStateHandler _gameStateHandler;
        private MovementHandler _movementHandler;
        private bool _whiteTurn;

        private Move _lastMove;

        private List<Piece> _whiteTakenPieces;
        private List<Piece> _blackTakenPieces;

        public event Action<bool, Action<PieceType>> OnPromotion;

        public ChessLogic(Board board)
        {
            this._board = board;
            _movementHandler = new MovementHandler();
            _gameStateHandler = new GameStateHandler();

            _movementHandler.OnBoardSaved += _gameStateHandler.SaveBoardState;
            _movementHandler.OnLastMoveChange += setLastMove;
            _movementHandler.OnPotentialCheck += PotentialCheckAfterMove;
            _movementHandler.OnPromotion += HandlePromotion;
            _movementHandler.OnCheck += IsCheck;

            _gameStateHandler.OnPossibleMovesRequest += _movementHandler.GetPossibleMoves;
            _gameStateHandler.OnPotentialCheck += PotentialCheckAfterMove;

            _whiteTurn = true;
            _whiteTakenPieces = new List<Piece>();
            _blackTakenPieces = new List<Piece>();
        }

        public void setLastMove(Move move) => _lastMove = move;
        internal bool IsWhiteTurn => _whiteTurn;
        internal void SwitchTurn() => _whiteTurn = !_whiteTurn;
        internal void FindLegalTiles(Board board, Tile currentTile, Piece chessPiece) => _movementHandler.FindLegalTiles(board, currentTile, chessPiece);
        internal bool CanMove(Vector from, Vector to) => _movementHandler.CanMove(from, to);
        internal void MovePiece(Board board, Vector from, Vector to) => _movementHandler.MovePiece(board, from, to);
        internal Vector FindKingPosition(Board board) => _gameStateHandler.FindKingPosition(board, _whiteTurn);
        internal bool IsCheck(Board board) => _gameStateHandler.IsCheck(board, _whiteTurn);
        internal void IsCheck(Board board, Action<bool> onSuccess) => onSuccess?.Invoke(_gameStateHandler.IsCheck(board, _whiteTurn));
        internal bool IsMate(Board board, bool whiteTurn) => _gameStateHandler.IsMate(board, whiteTurn);
        internal bool IsStalemate(Board board) => _gameStateHandler.IsStalemate(board, _whiteTurn);
        internal bool IsDraw() => _gameStateHandler.IsDraw(_board);
        internal int GetMaterialDifference() => _movementHandler.MaterialDifference;
        internal void HandlePromotion(bool isWhite, Action<PieceType> type) => OnPromotion?.Invoke(isWhite, type);
        internal void PotentialCheckAfterMove(Vector from, Vector to, Action<bool> onSuccess)
        {
            Board PotentialBoard = new()
            {
                BoardGrid = _board.GetBoardCopy()
            };

            if (PotentialBoard.IsTileOccupied(to) && PotentialBoard.GetPieceAt(to).Type == PieceType.King)
            {
                onSuccess?.Invoke(true);
            }

            _movementHandler.MovePiece(PotentialBoard, from, to, true);

            onSuccess?.Invoke(IsCheck(PotentialBoard));
        }
        internal void SortTakenPiecesByValue() 
        {
            _whiteTakenPieces = _whiteTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
            _blackTakenPieces = _blackTakenPieces.OrderByDescending(piece => piece.MaterialValue).ToList();
        }
        internal string GetLastMoveNotation() 
        {
            if (_lastMove == null)
            {
                return "";
            }
            string notation = "";
            Piece piece = _lastMove.MovedPiece;

            notation += piece.Notation;
            if (_lastMove.isCapture)
            {
                if (_lastMove.MovedPiece.Type == PieceType.Pawn)
                {
                    notation += _lastMove.From.GetFile();
                }
                notation += "x";
            }

            notation += _lastMove.To.GetFile() + _lastMove.To.GetRank();

            if (_lastMove.moveType == MoveType.Castling)
            {
                notation = _lastMove.To.Y == 2 ? "O-O-O" : "O-O";
            }
            else if (_lastMove.moveType == MoveType.Promotion)
            {
                notation += $"={_board.GetPieceAt(_lastMove.To).Notation}";
            }
            return notation;
        }
    }
}