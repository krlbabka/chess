﻿using Chess.HelperClasses;
using Chess.Representation;

namespace Chess.Pieces.Strategies
{
    internal class RookStrategy : IMovable
    {
        public bool CanMove(Board board, Vector from, Vector to, out MoveType type)
        {
            type = MoveType.Normal;
            bool obstacle = ObstacleInPath(board, from, to);
            bool canMoveToTile = board.GetPieceAt(to) == null || board.AreEnemies(from, to);

            if ((from.X == to.X || from.Y == to.Y) && !obstacle && canMoveToTile)
            {
                return true;
            }
            return false;
        }

        private bool ObstacleInPath(Board board, Vector from, Vector to)
        {
            if (from.X == to.X)
            {
                int step = from.Y < to.Y ? 1 : -1;
                for (int i = from.Y + step; (step == 1 ? i < to.Y : i > to.Y); i += step)
                {
                    Vector position = new Vector(from.X, i);
                    if (board.WithinBounds(position) && board.GetPieceAt(position) != null)
                    {
                        return true;
                    }
                }
            }
            else if (from.Y == to.Y)
            {
                int step = from.X < to.X ? 1 : -1;
                for (int i = from.X + step; (step == 1 ? i < to.X : i > to.X); i += step)
                {
                    Vector position = new Vector(i, from.Y);
                    if (board.WithinBounds(position) && board.GetPieceAt(position) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
