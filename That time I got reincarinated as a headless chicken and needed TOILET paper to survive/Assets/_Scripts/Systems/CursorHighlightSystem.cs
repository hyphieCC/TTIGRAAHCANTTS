using UnityEngine;
using Managers;

namespace Systems
{
    public class CursorHighlightSystem : MonoBehaviour
    {
        private PlayerSystem player;
        private SpriteRenderer sprite;

        void Start()
        {
            player = Object.FindFirstObjectByType<PlayerSystem>();
            sprite = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (player == null) return;

            // Move highlight to selected tile position
            Vector2Int pos = player.selectedPos;
            transform.position = new Vector3(pos.x, pos.y, -1);

            // Change color depending on whether it’s buildable
            Tile t = GridManager.Instance.GetTile(pos.x, pos.y);
            if (t == null)
                sprite.color = new Color(1, 0, 0, 0.5f); // red = out of bounds
            else if (t.type == TileType.Empty)
                sprite.color = new Color(1, 1, 0, 0.5f); // yellow = buildable
            else
                sprite.color = new Color(0, 1, 0, 0.5f); // green = existing room
        }
    }
}