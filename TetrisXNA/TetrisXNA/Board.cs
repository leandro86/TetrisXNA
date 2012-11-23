using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TetrisXNA
{
    public class Board : IEnumerable
    {
        public const int Width = 10;
        public const int Height = 20;
        private List<Block> _board = new List<Block>(Width * Height);

        public void Put(Shape shape)
        {
            foreach (Point point in shape)
            {
                _board.Add(new Block() {Point = new Point(point.X + shape.X, point.Y + shape.Y), ShapeType = shape.ShapeType});
            }
        }

        public void Clear()
        {
            _board.Clear();
        }

        public bool CanRotateShape(Shape shape)
        {
            Shape rotatedShape = (Shape)shape.Clone();
            rotatedShape.Rotate();

            return !IsSidesCollision(rotatedShape) && !IsRoofCollision(rotatedShape);
        }

        private bool IsRoofCollision(Shape shape)
        {
            if (shape.Y > 0)
            {
                return false;
            }

            foreach (Point point in shape)
            {
                if (point.Y == -1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsGroundCollision(Shape shape)
        {
            if (shape.Y > (Height - shape.Height))
            {
                return true;
            }

            foreach (Block block in _board)
            {
                foreach (Point point in shape)
                {
                    if ((point.X + shape.X == block.Point.X) && (point.Y + shape.Y == block.Point.Y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsSidesCollision(Shape shape)
        {
            if ((shape.X + shape.OriginX < 0) || (shape.X > (Width - shape.Width)))
            {
                return true;
            }

            foreach (Block block in _board)
            {
                foreach (Point point in shape)
                {
                    if ((point.X + shape.X == block.Point.X) && (point.Y + shape.Y == block.Point.Y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Block[] GetFilledLinesBlocks()
        {
            List<Block> filledLinesBlocks = new List<Block>();

            var blocksGroupsSameY = from b in _board
                                    group b by b.Point.Y
                                    into g
                                    orderby g.Key
                                    where g.Count() == Width
                                    select new {Y = g.Key, blocks = g};

            foreach (var blocksGroupSameY in blocksGroupsSameY)
            {
                filledLinesBlocks.AddRange(blocksGroupSameY.blocks);
            }
            
            return filledLinesBlocks.ToArray();
        }

        public void RemoveFilledLines()
        {
            foreach (IGrouping<int, Block> blocksGroupSameY in GetFilledLinesBlocks().GroupBy(b => b.Point.Y))
            {
                foreach (Block block in blocksGroupSameY)
                {
                    _board.Remove(block);
                }

                var blocksToGoDown = from b in _board
                                     where b.Point.Y < blocksGroupSameY.Key
                                     select b;

                foreach (Block blockToGoDown in blocksToGoDown)
                {
                    blockToGoDown.Point = new Point(blockToGoDown.Point.X, blockToGoDown.Point.Y + 1);
                }                
            }
        }

        public bool IsGameOver()
        {
            if (_board.Count == 0)
            {
                return false;
            }
            
            int minYBlock = _board.Min(b => b.Point.Y);
            if (minYBlock == 0)
            {
                return true;
            }

            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return _board.GetEnumerator();
        }
    }
}
