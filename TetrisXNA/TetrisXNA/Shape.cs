using System;
using System.Collections;
using System.Linq;

namespace TetrisXNA
{
    public class Shape : ICloneable, IEnumerable
    {
        private Point[][] _shape;
        private int _shapeRotationIndex = 0;

        public ShapeType ShapeType { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int OriginX { get; private set; }
        public int OriginY { get; private set; }

        public Shape(ShapeType shapeType)
        {
            ShapeType = shapeType;
            _shape = ShapesRepresentation.GetShape(ShapeType);

            CalculateWidthAndHeight();
        }

        private void CalculateWidthAndHeight()
        {           
            Point[] shapePointsOrderByX = _shape[_shapeRotationIndex].OrderBy(p => p.X).ToArray();
            Width = shapePointsOrderByX[3].X + 1;
            OriginX = shapePointsOrderByX[0].X;

            Point[] shapePointsOrderByY = _shape[_shapeRotationIndex].OrderByDescending(p => p.Y).ToArray();
            Height = shapePointsOrderByY[0].Y + 1;
            OriginY = shapePointsOrderByY[3].Y;
        }
              
        public void Rotate()
        {            
            _shapeRotationIndex += 1;
            _shapeRotationIndex = _shapeRotationIndex % _shape.Count();

            CalculateWidthAndHeight();
        }

        public void ChangeActualShape(ShapeType shapeType)
        {            
            ShapeType = shapeType;
            _shape = ShapesRepresentation.GetShape(shapeType);
            _shapeRotationIndex = 0;
            CalculateWidthAndHeight();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _shape[_shapeRotationIndex].GetEnumerator();
        }
    }
}
