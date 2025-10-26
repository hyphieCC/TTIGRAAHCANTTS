using Systems;
using UnityEngine;
using  System.Collections;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        public int currentFloor = 1;
        private FloorGenerator floorGen;
        private PlayerSystem player;

        public System.Action<int> OnFloorChanged;

        protected override void Awake()
        {
            base.Awake();
            floorGen = FindFirstObjectByType<FloorGenerator>();
            player = FindFirstObjectByType<PlayerSystem>();
        }

        public void NextFloor()
        {
            currentFloor++;
            OnFloorChanged?.Invoke(currentFloor);
            Debug.Log($"Generating Floor {currentFloor}");

            // Regenerate the new grid
            floorGen.GenerateFloor(currentFloor);

            // Reset player to new start
            int startX = GridManager.Instance.width / 2; // Center horizontally
            int startY = 0; // Bottom row
            Vector2Int startPos = new Vector2Int(startX, startY);

            player.currentPos = startPos;
            player.selectedPos = startPos + Vector2Int.up;
            player.transform.position = new Vector3(startPos.x, startPos.y, player.transform.position.z);

            player.facingDirection = Direction.Up;

            Debug.Log($"Player reset to start of Floor {currentFloor} at {player.transform.position}");
        }

        public IEnumerator DelayedNextFloor(float delay)
        {
            Debug.Log($"Waiting {delay} seconds before next floor...");
            yield return new WaitForSeconds(delay);
            NextFloor();
        }
    }
}