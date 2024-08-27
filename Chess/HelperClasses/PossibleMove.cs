namespace Chess.HelperClasses
{
    public enum MoveType
    {
        Normal,
        EnPassant,
        Castling,
        Promotion
    }

    internal class PossibleMove
    {
        public Vector vector;
        public MoveType moveType;
        public PossibleMove(Vector vector, MoveType moveType = MoveType.Normal)
        {
            this.vector = vector;
            this.moveType = moveType;
        }
    }
}
