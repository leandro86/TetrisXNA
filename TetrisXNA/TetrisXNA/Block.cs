namespace TetrisXNA
{
    public class Block
    {
        public Point Point { get; set; }
        public ShapeType ShapeType { get; set; }
        /*
        public override bool Equals(object obj)
        {
            Block block = obj as Block;

            return Point.X == block.Point.X && Point.Y == block.Point.Y;
        }*/
    }
}
