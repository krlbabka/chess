namespace Chess
{
    internal abstract class Piece
    {
        internal enum PieceType
        {
            Pawn,
            Rook,
            Knight,
            Bishop,
            Queen,
            King
        }

        public PieceType Type { get; set; }
        public bool IsWhite { get; set; }

        public Piece(PieceType type, bool isWhite)
        {
            Type = type;
            IsWhite = isWhite;
        }

        public abstract Vector[] MoveVectors { get; }
    }

    internal class Knight : Piece 
    {
        
        internal Vector[] moveVectors = {
            new Vector(1, 2),
            new Vector(2, 1),
            new Vector(2, -1),
            new Vector(1, -2),
            new Vector(-1, -2),
            new Vector(-2, -1),
            new Vector(-2, 1),
            new Vector(-1, 2)
        };

        public Knight(bool isWhite) : base(PieceType.Knight, isWhite)
        {

        }

        public override Vector[] MoveVectors => moveVectors;
    }

    internal class Bishop : Piece
    {

        internal Vector[] moveVectors = {
            new Vector(1, 1),
            new Vector(1, -1),
            new Vector(-1, -1),
            new Vector(-1, 1)
        };

        public Bishop(bool isWhite) : base(PieceType.Bishop, isWhite)
        {

        }

        public override Vector[] MoveVectors => moveVectors;
    }

    internal class Rook : Piece
    {

        public Vector[] moveVectors { get; }

        public Rook(bool isWhite) : base(PieceType.Rook, isWhite)
        {
            moveVectors = new Vector[] {
                new Vector(1, 0),
                new Vector(0, 1),
                new Vector(-1, 0),
                new Vector(0, -1)
            };
        }

        public override Vector[] MoveVectors => moveVectors;
    }

    internal class Queen : Piece
    {

        internal Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(1, 1),
            new Vector(0, 1),
            new Vector(1, -1),
            new Vector(-1, 0),
            new Vector(-1, -1),
            new Vector(0, -1),
            new Vector(-1, 1)
        };

        public Queen(bool isWhite) : base(PieceType.Queen, isWhite)
        {

        }

        public override Vector[] MoveVectors => moveVectors;
    }

    internal class King : Piece
    {

        public readonly Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(1, 1),
            new Vector(0, 1),
            new Vector(1, -1),
            new Vector(-1, 0),
            new Vector(-1, -1),
            new Vector(0, -1),
            new Vector(-1, 1)
        };

        public King(bool isWhite) : base(PieceType.King, isWhite)
        {

        }

        public override Vector[] MoveVectors => moveVectors;
    }

    internal class Pawn : Piece
    {
        public Pawn(bool isWhite) : base(PieceType.Pawn, isWhite)
        {

        }

        public override Vector[] MoveVectors => [];
    }
}