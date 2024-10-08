﻿using Chess.HelperClasses;
using Chess.Pieces;

namespace Chess.Representation
{
    internal class Board
    {
        public const int BOARD_SIZE = 8;

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
        internal Piece? GetPieceAt(Vector position) => BoardGrid[position.X, position.Y].OccupyingPiece;
        internal Piece? GetPieceAt(Tile tile) => tile.OccupyingPiece;
        internal bool IsTileOccupied(Vector position) => BoardGrid[position.X, position.Y].IsOccupied;
        internal bool IsTileOccupied(Tile tile) => tile.IsOccupied;
        internal Tile GetTile(Vector position) => BoardGrid[position.X, position.Y];

        public Tile[,] GetBoardCopy()
        {
            Tile[,] copy = new Tile[BOARD_SIZE, BOARD_SIZE];
            for (int row = 0; row < BOARD_SIZE; row++)
            {
                for (int col = 0; col < BOARD_SIZE; col++)
                {
                    copy[row, col] = new Tile(new Vector(row, col));
                    if (BoardGrid[row, col].IsOccupied)
                    {
                        copy[row, col].CreatePiece(BoardGrid[row, col].OccupyingPiece!.Type, BoardGrid[row, col].OccupyingPiece!.IsWhite);
                    }
                }
            }
            return copy;
        }
        internal void SetDefaultBoardPosition()
        {
            PieceType[] BackRankPieces = {
                PieceType.Rook,
                PieceType.Knight,
                PieceType.Bishop,
                PieceType.Queen,
                PieceType.King,
                PieceType.Bishop,
                PieceType.Knight,
                PieceType.Rook
            };
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                BoardGrid[0, col].CreatePiece(BackRankPieces[col], false);
                BoardGrid[1, col].CreatePiece(PieceType.Pawn, false);
                for (int row = 2; row <= 5; row++)
                {
                    BoardGrid[row, col].RemoveCurrentPiece();
                }
                BoardGrid[6, col].CreatePiece(PieceType.Pawn, true);
                BoardGrid[7, col].CreatePiece(BackRankPieces[col], true);
            }
        }
        internal void ResetLegalMoves()
        {
            foreach (Tile tile in BoardGrid)
            {
                tile.LegalMove = false;
            }
        }
        internal bool WithinBounds(Vector position)
        {
            return position.X >= 0 && position.X < BOARD_SIZE && position.Y >= 0 && position.Y < BOARD_SIZE;
        }
        internal bool AreEnemies(Vector myPosition, Vector newPosition)
        {
            return GetPieceAt(myPosition)!.IsWhite != GetPieceAt(newPosition)!.IsWhite;
        }
    }
}
