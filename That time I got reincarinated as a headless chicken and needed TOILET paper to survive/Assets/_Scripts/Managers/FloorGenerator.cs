using UnityEngine;

namespace Managers
{
    public class FloorGenerator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RoomDatabase roomDatabase; // assign in inspector if possible

        [Header("Visuals")]
        [SerializeField] private Sprite blankGridSprite;

        public Vector2Int keyRoomPos;
        public Vector2Int endRoomPos;

        public bool shopBuiltThisFloor = false;

        void Start()
        {
            if (roomDatabase == null)
                roomDatabase = FindFirstObjectByType<RoomDatabase>();

            GenerateFloor(1);
        }

        public void GenerateFloor(int floorIndex)
        {
            ClearPreviousFloor();
            shopBuiltThisFloor = false;

            int width = 5;
            int height = 3 + floorIndex;
            GridManager.Instance.InitializeGrid(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var tile = GridManager.Instance.GetTile(x, y);

                    GameObject gridVisual = new GameObject($"BlankGrid_{x}_{y}");
                    var sr = gridVisual.AddComponent<SpriteRenderer>();
                    sr.sprite = blankGridSprite;
                    sr.color = new Color(1f, 1f, 1f, 0.25f);
                    gridVisual.transform.position = new Vector3(x, y, 1f);
                    tile.instance = gridVisual;
                }
            }

            // START ROOM
            int startX = width / 2; // middle of the grid’s width
            int startY = 0; // bottom row
            var startTile = GridManager.Instance.GetTile(startX, startY);
            startTile.type = TileType.Start;
            startTile.roomType = RoomType.ThreeWay;
            startTile.SetDoor(Direction.Up, true);
            startTile.SetDoor(Direction.Left, true);
            startTile.SetDoor(Direction.Right, true);

            // Fetch the Start Room definition from database
            RoomDefinition startRoomDef = null;
            if (roomDatabase != null)
                startRoomDef = roomDatabase.GetRoomByName("Start Room");

            if (startRoomDef != null)
            {
                startTile.instanceDefinition = startRoomDef;

                GameObject startObj = new GameObject("StartRoomVisual");
                var sr = startObj.AddComponent<SpriteRenderer>();
                sr.sprite = startRoomDef.spriteFromUp != null
                    ? startRoomDef.spriteFromUp
                    : startRoomDef.spriteFromDown != null
                        ? startRoomDef.spriteFromDown
                        : null;

                startObj.transform.position = new Vector3(startX, startY, 0);
                startTile.instance = startObj;
            }
            else
            {
                Debug.LogWarning("No 'Start Room' definition found in RoomDatabase!");
            }

            // END ROOM
            int endX = 2;
            int endY = GridManager.Instance.height - 1;
            var endTile = GridManager.Instance.GetTile(endX, endY);
            endTile.type = TileType.End;

            // Load the End Room
            RoomDefinition endRoomDef = null;
            if (roomDatabase != null)
                endRoomDef = roomDatabase.GetRoomByName("End Room");

            if (endRoomDef != null)
            {
                endTile.instanceDefinition = endRoomDef;

                GameObject endObj = new GameObject("EndRoomVisual");
                var sr = endObj.AddComponent<SpriteRenderer>();
                sr.sprite = endRoomDef.spriteFromDown ?? endRoomDef.spriteFromUp;

                endObj.transform.position = new Vector3(endX, endY, 0);
                endTile.instance = endObj;

                // Spawn Doorkeeper NPC if assigned
                if (endRoomDef.doorkeeperPrefab != null)
                {
                    GameObject npc = Instantiate(endRoomDef.doorkeeperPrefab);
                    npc.transform.position = new Vector3(endX - 0.15f, endY - 0.4f, 0f);
                    npc.transform.SetParent(endObj.transform);
                }
            }
            else
            {
                Debug.LogWarning("No 'End Room' definition found in RoomDatabase!");
            }
        }

        private void ClearPreviousFloor()
        {
            if (GridManager.Instance?.grid == null) return;

            for (int x = 0; x < GridManager.Instance.width; x++)
            {
                for (int y = 0; y < GridManager.Instance.height; y++)
                {
                    var tile = GridManager.Instance.GetTile(x, y);
                    if (tile?.instance != null)
                    {
                        Destroy(tile.instance);
                        tile.instance = null;
                    }
                }
            }
        }
    }
}