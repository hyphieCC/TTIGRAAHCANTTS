using System.Collections.Generic;
//using static Enums;

public static class RoomPresets
{
    public static List<Direction> GetDoorsForRoom(RoomType type)
    {
        switch (type)
        {
            case RoomType.DeadEnd:
                return new List<Direction> { Direction.Down };
            case RoomType.Straight:
                return new List<Direction> { Direction.Up, Direction.Down };
            case RoomType.TurnLeft:
                return new List<Direction> { Direction.Up, Direction.Left };
            case RoomType.TurnRight:
                return new List<Direction> { Direction.Up, Direction.Right };
            case RoomType.ThreeWay:
                return new List<Direction> { Direction.Up, Direction.Left, Direction.Right };
            case RoomType.Cross:
                return new List<Direction> { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
            default:
                return new List<Direction>();
        }
    }
}