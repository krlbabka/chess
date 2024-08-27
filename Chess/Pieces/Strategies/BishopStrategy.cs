﻿using Chess.HelperClasses;

namespace Chess.Pieces.Strategies
{
    internal class BishopStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to)
        {
            bool obstacle = ObstacleInPath(board, from, to);
            if ((Math.Abs(from.X - to.X) == Math.Abs(from.Y - to.Y)) && !obstacle)
            {
                return true;
            }
            return false;
        }

        private bool ObstacleInPath(Board board, Vector from, Vector to)
        {
            int stepX = from.X < to.X ? 1 : -1;
            int stepY = from.Y < to.Y ? 1 : -1;
            for (int i = 1; i < Math.Abs(from.X - to.X); i++)
            {
                Vector position = new Vector(from.X + i * stepX, from.Y + i * stepY);
                if (board.WithinBounds(position) && board.GetPieceAt(position) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
