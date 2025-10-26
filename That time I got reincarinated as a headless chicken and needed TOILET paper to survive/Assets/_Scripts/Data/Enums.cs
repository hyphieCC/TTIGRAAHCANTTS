//using UnityEngine;

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public enum RoomType
{
    DeadEnd,
    Straight,
    TurnLeft,
    TurnRight,
    ThreeWay,
    Cross
}

public enum TileType
{
    Empty,
    Floor,
    Obstacle,
    Start,
    End
}
