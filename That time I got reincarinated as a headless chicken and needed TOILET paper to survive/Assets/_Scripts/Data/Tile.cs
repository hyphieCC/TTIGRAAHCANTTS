using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Tile
{
    public int x, y;
    public TileType type = TileType.Empty;
    public bool[] doors = new bool[4];
    public bool isLocked = false;
    public bool isDiscovered = false;
    public GameObject instance;
    public RoomType roomType;
    public RoomDefinition instanceDefinition;
    public bool rewardCollected = false;

    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2Int Position => new Vector2Int(x, y);
    public bool HasDoor(Direction dir) => doors[(int)dir];
    public void SetDoor(Direction dir, bool open) => doors[(int)dir] = open;
}