using System.Reflection.Metadata.Ecma335;

namespace Chess
{
    internal class Board
    {
        const int BOARD_SIZE = 8;

        public Tile[,] BoardGrid;

        public Board()
        {
            BoardGrid = new Tile[BOARD_SIZE, BOARD_SIZE];
        
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    BoardGrid[row, col] = new Tile(new Vector(row, col));
                }
            }
        }

        private void FindLegalTiles(Tile CurrentTile, Piece ChessPiece)
        {
            foreach (Tile tile in BoardGrid)
            {
                tile.LegalMove = false;
            }

            var possibleMoves = GetPossibleMoves(ChessPiece, CurrentTile.Position);

            foreach (var item in possibleMoves)
            {
                BoardGrid[item.X, item.Y].LegalMove = true;
            }
        }

        private List<Vector> GetPossibleMoves(Piece piece, Vector currentPosition)
        {
            var possibleMoves = new List<Vector>();
            var multiSquarePieces = new[] { Piece.PieceType.Bishop, Piece.PieceType.Rook, Piece.PieceType.Queen };

            foreach (var vector in piece.MoveVectors)
            {
                if (multiSquarePieces.Contains(piece.Type))
                {
                    for (int distance_multiplier = 1; distance_multiplier < BOARD_SIZE; distance_multiplier++)
                    {
                        var newX = currentPosition.X + vector.X * distance_multiplier;
                        var newY = currentPosition.Y + vector.Y * distance_multiplier;
                        possibleMoves.Add(new Vector(newX, newY));
                    }
                }
                else
                {
                    var newX = currentPosition.X + vector.X;
                    var newY = currentPosition.Y + vector.Y;
                    possibleMoves.Add(new Vector(newX, newY));
                }
            }
            return possibleMoves;
        }
    }
}
