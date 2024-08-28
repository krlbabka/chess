﻿using Chess.HelperClasses;
using Chess.Logic;
using Chess.Pieces;

namespace Chess
{
    internal class Tile
    {
        public Vector Position { get; set;}
        public bool IsOccupied { get; set; }
        public bool LegalMove { get; set; }
        public MoveType MoveType { get; set; }
        public Piece? OccupyingPiece { get; set; }

        public Tile(Vector position, MoveType moveType = MoveType.Normal)
        {
            Position = position;
            IsOccupied = false;
            LegalMove = false;
            OccupyingPiece = null;
            MoveType = moveType;
        }

        internal void AddPieceToTile(PieceType type, bool isWhite)
        {
            OccupyingPiece = PieceFactory.CreatePiece(type, isWhite);
            IsOccupied = true;
        }

        internal void SetEmpty()
        {
            IsOccupied = false;
            OccupyingPiece = null;
        }

        public void SetOccupyingPiece(Piece piece)
        {
            OccupyingPiece = piece;
            IsOccupied = true;
        }
    }
}
