using static Chess.Piece;

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
            OccupyingPiece = CreatePiece(type, isWhite);
            IsOccupied = true;
        }

        internal void CreateEmptyTile()
        {
            IsOccupied = false;
            OccupyingPiece = null;
        }
    }
}
