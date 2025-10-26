public static class DirectionExtensions
{
    public static Direction Opposite(this Direction dir) => (Direction)(((int)dir + 2) % 4);

    public static Direction LeftOf(this Direction dir) => (Direction)(((int)dir + 3) % 4);

    public static Direction RightOf(this Direction dir) => (Direction)(((int)dir + 1) % 4);
}
