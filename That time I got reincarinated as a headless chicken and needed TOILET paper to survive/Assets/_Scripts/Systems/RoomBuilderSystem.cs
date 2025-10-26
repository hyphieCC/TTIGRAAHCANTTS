using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Systems
{
    public class RoomBuilderSystem : MonoBehaviour
    {
        public void BuildRoom(Vector2Int origin, Vector2Int targetPos, RoomType type, RoomDefinition def = null)
        {
            // Determine direction from origin to target
            Vector2Int diff = targetPos - origin;
            Direction dir = Direction.Up;

            if (diff == Vector2Int.up) dir = Direction.Up;
            else if (diff == Vector2Int.down) dir = Direction.Down;
            else if (diff == Vector2Int.left) dir = Direction.Left;
            else if (diff == Vector2Int.right) dir = Direction.Right;
            else
            {
                Debug.Log("Invalid build direction!");
                return;
            }

            // Get tiles
            Tile originTile = GridManager.Instance.GetTile(origin.x, origin.y);
            Tile targetTile = GridManager.Instance.GetTile(targetPos.x, targetPos.y);

            // Safety checks
            if (!GridManager.Instance.InBounds(targetPos.x, targetPos.y))
            {
                Debug.Log("Out of bounds — can't build here!");
                return;
            }

            if (originTile == null || targetTile == null)
            {
                Debug.Log("Invalid build target — out of bounds!");
                return;
            }

            if (targetTile.type != TileType.Empty)
            {
                Debug.Log("Can't build here — tile already occupied!");
                return;
            }

            // Create new room
            targetTile.type = TileType.Floor;
            targetTile.roomType = type;
            targetTile.instanceDefinition = def;

            ApplyRoomTypeDoors(targetTile, type, dir);

            // Connect both sides
            originTile.SetDoor(dir, true);
            targetTile.SetDoor(dir.Opposite(), true);

            // Visual creation
            GameObject roomObj = new GameObject($"Room_{targetPos.x}_{targetPos.y}");
            SpriteRenderer sr = roomObj.AddComponent<SpriteRenderer>();
            roomObj.transform.position = new Vector3(targetPos.x, targetPos.y, 0);
            roomObj.transform.localScale = Vector3.one;

            // Directional sprite logic
            if (def != null)
            {
                Sprite chosenSprite = null;
                switch (dir)
                {
                    case Direction.Up: chosenSprite = def.spriteFromUp; break;
                    case Direction.Down: chosenSprite = def.spriteFromDown; break;
                    case Direction.Left: chosenSprite = def.spriteFromLeft; break;
                    case Direction.Right: chosenSprite = def.spriteFromRight; break;
                }

                if (chosenSprite != null)
                    sr.sprite = chosenSprite;
                else
                    sr.color = Color.magenta; // fallback color
            }
            else
            {
                sr.color = Color.white;
            }

            targetTile.instance = roomObj;
            Debug.Log($"Built a {def?.roomName ?? type.ToString()} room at {targetPos} (entered from {dir})");

            if (def != null && def.shopkeeperPrefab != null)
            {
                GameObject npc = Instantiate(def.shopkeeperPrefab);
                npc.transform.position = new Vector3(targetPos.x + 0.3f, targetPos.y + 0.17f, 0f);
                
                npc.GetComponent<SpriteRenderer>().sortingOrder = 2;
                npc.transform.SetParent(roomObj.transform);

                Debug.Log($"[RoomBuilderSystem] Spawned Shopkeeper in {def.roomName} at {targetPos}");
            }

            // Spawn Doorkeeper if this is an End Room type
            if (def != null && def.doorkeeperPrefab != null && def.roomName == "End Room")
            {
                GameObject npc = GameObject.Instantiate(def.doorkeeperPrefab);
                npc.transform.position = new Vector3(targetPos.x, targetPos.y - 0.3f, 0f);
                npc.transform.SetParent(roomObj.transform);
                Debug.Log("[RoomBuilderSystem] Spawned Doorkeeper in End Room at " + targetPos);
            }
        }

        private void ApplyRoomTypeDoors(Tile tile, RoomType type, Direction facing)
        {
            for (int i = 0; i < 4; i++)
                tile.doors[i] = false;

            switch (type)
            {
                case RoomType.DeadEnd:
                    tile.SetDoor(facing.Opposite(), true);
                    break;
                case RoomType.Straight:
                    tile.SetDoor(facing, true);
                    tile.SetDoor(facing.Opposite(), true);
                    break;
                case RoomType.TurnLeft:
                    tile.SetDoor(facing, true);
                    tile.SetDoor(facing.LeftOf(), true);
                    break;
                case RoomType.TurnRight:
                    tile.SetDoor(facing, true);
                    tile.SetDoor(facing.RightOf(), true);
                    break;
                case RoomType.ThreeWay:
                    tile.SetDoor(facing.LeftOf(), true);
                    tile.SetDoor(facing.RightOf(), true);
                    tile.SetDoor(facing.Opposite(), true);
                    break;
                case RoomType.Cross:
                    for (int i = 0; i < 4; i++)
                        tile.SetDoor((Direction)i, true);
                    break;
            }
        }
    }
}