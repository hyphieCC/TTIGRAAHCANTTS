using UnityEngine;

namespace Managers
{
    public class GridManager : Singleton<GridManager>
    {
        public int width;
        public int height;
        public Tile[,] grid;

        public void InitializeGrid(int w, int h)
        {
            width = w; height = h;
            grid = new Tile[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    grid[x, y] = new Tile(x, y);
        }

        public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;
        public Tile GetTile(int x, int y) => InBounds(x, y) ? grid[x, y] : null;

        public void ConnectRooms(int x1, int y1, int x2, int y2)
        {
            Tile a = GetTile(x1, y1);
            Tile b = GetTile(x2, y2);
            if (a == null || b == null) return;

            if (x2 == x1 && y2 == y1 + 1) { a.SetDoor(Direction.Up, true); b.SetDoor(Direction.Down, true); }
            else if (x2 == x1 + 1 && y2 == y1) { a.SetDoor(Direction.Right, true); b.SetDoor(Direction.Left, true); }
            else if (x2 == x1 && y2 == y1 - 1) { a.SetDoor(Direction.Down, true); b.SetDoor(Direction.Up, true); }
            else if (x2 == x1 - 1 && y2 == y1) { a.SetDoor(Direction.Left, true); b.SetDoor(Direction.Right, true); }
        }

        public static Vector2Int DirectionToOffset(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return new Vector2Int(0, 1);
                case Direction.Right: return new Vector2Int(1, 0);
                case Direction.Down: return new Vector2Int(0, -1);
                case Direction.Left: return new Vector2Int(-1, 0);
                default: return Vector2Int.zero;
            }
        }

        public static Direction LocalToGlobal(Direction localDir, Direction facing)
        {
            int result = ((int)localDir + (int)facing) % 4;
            return (Direction)result;
        }

        void OnDrawGizmos()
        {
            if (grid == null) return;
            Gizmos.color = Color.gray;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    Gizmos.DrawWireCube(new Vector3(x, y, 0), Vector3.one);
        }
    }
}