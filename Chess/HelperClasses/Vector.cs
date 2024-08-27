﻿namespace Chess.HelperClasses
{
    public class Vector
    {
        public int X { get; }
        public int Y { get; }
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector first, Vector second)
        {
            return new Vector(first.X + second.X, first.Y + second.Y);
        }

        public static Vector operator -(Vector first, Vector second)
        {
            return new Vector(first.X - second.X, first.Y - second.Y);
        }

        public static Vector operator *(Vector vector, int value)
        {
            return new Vector(vector.X * value, vector.Y * value);
        }

        public bool IsEqual(Vector Other)
        {
            return X == Other.X && Y == Other.Y;
        }
    }
}
