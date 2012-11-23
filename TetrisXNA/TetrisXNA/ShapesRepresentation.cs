namespace TetrisXNA
{
    public static class ShapesRepresentation
    {
        private static Point[][] _line = new Point[][]
                                             {
                                                 new Point[] {new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(3, 0)},
                                                 new Point[] {new Point(1, -1), new Point(1, 0), new Point(1, 1), new Point(1, 2)}
                                             };

        private static Point[][] _square = new Point[][]
                                               {
                                                   new Point[] {new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1)},
                                               };

        private static Point[][] _triangle = new Point[][]
                                                 {
                                                     new Point[] {new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(1, 1)},
                                                     new Point[] {new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(1, -1)},
                                                     new Point[] {new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(1, -1)},
                                                     new Point[] {new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 0)}
                                                 };

        private static Point[][] _leftS = new Point[][]
                                              {
                                                  new Point[] {new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1)},
                                                  new Point[] {new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, -1)}
                                              };

        private static Point[][] _rightS = new Point[][]
                                               {
                                                   new Point[] {new Point(1, 0), new Point(2, 0), new Point(1, 1), new Point(0, 1)},
                                                   new Point[] {new Point(1, -1), new Point(1, 0), new Point(2, 0), new Point(2, 1)}
                                               };

        private static Point[][] _leftL = new Point[][]
                                              {
                                                  new Point[] {new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(2, 1)},
                                                  new Point[] {new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(1, -1)},
                                                  new Point[] {new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1)},
                                                  new Point[] {new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 0)}
                                              };

        private static Point[][] _rightL = new Point[][]
                                               {
                                                   new Point[] {new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(0, 1)},
                                                   new Point[] {new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(1, 2)},
                                                   new Point[] {new Point(1, 0), new Point(1, 1), new Point(0, 1), new Point(-1, 1)},
                                                   new Point[] {new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(0, -1)}
                                               };

        private static Point[][][] _shapes = new Point[][][] {_leftL, _rightS, _line, _rightL, _leftS, _square, _triangle};
        
        public static Point[][] GetShape(ShapeType shapeType)
        {
            return _shapes[(int)shapeType];
        }
    }
}
