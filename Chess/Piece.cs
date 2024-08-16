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
        public abstract char Notation { get; }

        public Piece(PieceType type, bool isWhite)
        {
            Type = type;
            IsWhite = isWhite;
        }

        internal static Piece CreatePiece(PieceType type, bool isWhite)
        {
            return type switch
            {
                PieceType.Rook => new Rook(isWhite),
                PieceType.Knight => new Knight(isWhite),
                PieceType.Bishop => new Bishop(isWhite),
                PieceType.Queen => new Queen(isWhite),
                PieceType.King => new King(isWhite),
                PieceType.Pawn => new Pawn(isWhite),
            };
        }

        public abstract Vector[] MoveVectors { get; }
    }

    internal class Knight : Piece 
    {
        
        internal readonly Vector[] moveVectors = {
            new Vector(1, 2),
            new Vector(2, 1),
            new Vector(2, -1),
            new Vector(1, -2),
            new Vector(-1, -2),
            new Vector(-2, -1),
            new Vector(-2, 1),
            new Vector(-1, 2)
        };

        public Knight(bool isWhite) : base(PieceType.Knight, isWhite) { }

        public override Vector[] MoveVectors => moveVectors;

        public override char Notation => 'N';
    }

    internal class Bishop : Piece
    {

        internal readonly Vector[] moveVectors = {
            new Vector(1, 1),
            new Vector(1, -1),
            new Vector(-1, -1),
            new Vector(-1, 1)
        };

        public Bishop(bool isWhite) : base(PieceType.Bishop, isWhite) { }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'B';
    }

    internal class Rook : Piece
    {

        internal readonly Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(0, 1),
            new Vector(-1, 0),
            new Vector(0, -1)
        };

        public Rook(bool isWhite) : base(PieceType.Rook, isWhite) { }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'R';
    }

    internal class Queen : Piece
    {

        internal readonly Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(1, 1),
            new Vector(0, 1),
            new Vector(1, -1),
            new Vector(-1, 0),
            new Vector(-1, -1),
            new Vector(0, -1),
            new Vector(-1, 1)
        };

        public Queen(bool isWhite) : base(PieceType.Queen, isWhite) { }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'Q';
    }

    internal class King : Piece
    {

        internal readonly Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(1, 1),
            new Vector(0, 1),
            new Vector(1, -1),
            new Vector(-1, 0),
            new Vector(-1, -1),
            new Vector(0, -1),
            new Vector(-1, 1)
        };

        public King(bool isWhite) : base(PieceType.King, isWhite) { }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'K';
    }

    internal class Pawn : Piece
    {
        //TODO Add pawn vectors and logic
        public Pawn(bool isWhite) : base(PieceType.Pawn, isWhite) { }

        public override Vector[] MoveVectors => [];
        public override char Notation => 'P';
    }
}