
namespace Chess
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

    internal abstract class Piece
    {
        public PieceType Type { get; set; }
        public bool IsWhite { get; set; }
        public abstract char Notation { get; }
        public virtual int MaterialValue { get; }

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
                _ => throw new NotImplementedException()
            };
        }

        internal abstract Image GetPieceImage(bool isWhite);
        public abstract Vector[] MoveVectors { get; }
        public virtual bool HasMoved() { return false; }
        public virtual bool PieceMoved() { return false; }
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
        public override int MaterialValue => 3;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_knight.png" : "b_knight.png");
            return Image.FromFile(source);
        }
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
        public override int MaterialValue => 3;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_bishop.png" : "b_bishop.png");
            return Image.FromFile(source);
        }
    }

    internal class Rook : Piece
    {

        internal readonly Vector[] moveVectors = {
            new Vector(1, 0),
            new Vector(0, 1),
            new Vector(-1, 0),
            new Vector(0, -1)
        };
        private bool Moved;

        public Rook(bool isWhite) : base(PieceType.Rook, isWhite)
        {
            Moved = false;
        }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'R';
        public override int MaterialValue => 5;
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_rook.png" : "b_rook.png");
            return Image.FromFile(source);
        }
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
        public override int MaterialValue => 9;

        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_queen.png" : "b_queen.png");
            return Image.FromFile(source);
        }
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
        private bool Moved;

        public King(bool isWhite) : base(PieceType.King, isWhite)
        {
            Moved = false;
        }

        public override Vector[] MoveVectors => moveVectors;
        public override char Notation => 'K';
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
        internal override Image GetPieceImage(bool isWhite)
        {
            string source = "../../../Resources/" + (isWhite ? "w_king.png" : "b_king.png");
            return Image.FromFile(source);
        }
    }

    internal class Pawn : Piece
    {
        private readonly Vector[] moveVectors;
        private bool Moved;

        public Pawn(bool isWhite) : base(PieceType.Pawn, isWhite)
        {
            Moved = false;
            moveVectors = isWhite ? [new Vector(-1, 0)] : [new Vector(1, 0)];
        }

        public override Vector[] MoveVectors => moveVectors;
        public override bool HasMoved() => Moved;
        public override bool PieceMoved() => Moved = true;
        public override char Notation => ' ';
        public override int MaterialValue => 1;

        internal override Image GetPieceImage(bool isWhite) 
        {
            string source = "../../../Resources/" + (isWhite ? "w_pawn.png" : "b_pawn.png");
            return Image.FromFile(source);
        }
    }
}