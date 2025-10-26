using UnityEngine;
using Managers;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class PlayerSystem : MonoBehaviour
    {
        [Header("Grid Position")]
        public Vector2Int currentPos; // The player’s current room
        public Vector2Int selectedPos; // The tile currently highlighted for building

        [Header("Player State")]
        public Direction facingDirection = Direction.Up;
        public float moveSpeed = 5f;
        private bool isMoving;
        private Vector3 targetPos;
        private bool isBuilding = false;

        [Header("Player Inventory")]
        public PlayerInventory inventory = new PlayerInventory();

        [Header("Stamina")]
        public int stamina;
        public int maxStamina = 100;

        [Header("Room Database")]
        [SerializeField] private RoomDatabase roomDatabase;
        private RoomDefinition selectedRoom;

        private PlayerAnimationController anim;

        void Start()
        {
            currentPos = new Vector2Int(2, 0);
            selectedPos = currentPos + Vector2Int.up;
            transform.position = new Vector3(currentPos.x, currentPos.y, transform.position.z);

            inventory.stamina = stamina;

            anim = GetComponent<PlayerAnimationController>();
        }

        private bool isDead = false;

        void Update()
        {
            if (isDead || isBuilding) return;

            if (isMoving)
            {
                Move();
                return;
            }

            HandleCursorSelection();

            //// TEMP: press 1 to select StraightRoom_UpDown
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    selectedRoom = roomDatabase.GetRoomByName("Straight Room");
            //    Debug.Log($"Selected: {selectedRoom.roomName}");
            //}

            // Build new room in facing direction
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryBuildRoom();
            }

            // Move into connected, built room
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryEnterRoom();
            }
        }

        // Selection: move the cursor highlight
        void HandleCursorSelection()
        {
            if (isBuilding) return;

            Vector2Int next = selectedPos;
            bool directionChanged = false;

            if (Input.GetKeyDown(KeyCode.W)) { facingDirection = Direction.Up; next = currentPos + Vector2Int.up; directionChanged = true; }
            if (Input.GetKeyDown(KeyCode.D)) { facingDirection = Direction.Right; next = currentPos + Vector2Int.right; directionChanged = true; }
            if (Input.GetKeyDown(KeyCode.S)) { facingDirection = Direction.Down; next = currentPos + Vector2Int.down; directionChanged = true; }
            if (Input.GetKeyDown(KeyCode.A)) { facingDirection = Direction.Left; next = currentPos + Vector2Int.left; directionChanged = true; }

            if (GridManager.Instance.InBounds(next.x, next.y))
                selectedPos = next;

            if (directionChanged)
            {
                var anim = GetComponent<PlayerAnimationController>();
            }
        }

        // Build a new room in facing direction
        void TryBuildRoom()
        {
            Debug.Log("Pressed E to build — checking UI");

            // Check that target tile is valid
            if (!GridManager.Instance.InBounds(selectedPos.x, selectedPos.y))
            {
                Debug.Log("Out of bounds — can't build here!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            Tile current = GridManager.Instance.GetTile(currentPos.x, currentPos.y);
            Tile target = GridManager.Instance.GetTile(selectedPos.x, selectedPos.y);

            if (current == null || target == null)
            {
                Debug.Log("Invalid build target!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            if (!current.HasDoor(facingDirection))
            {
                Debug.Log("Can't build here — no connecting door!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            if (target.type != TileType.Empty)
            {
                Debug.Log("Can't build here — tile already occupied!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            // Instead of immediately building, open the selection UI
            var ui = FindFirstObjectByType<RoomSelectionUI>();
            if (ui != null)
            {
                if (!ui.gameObject.activeSelf)
                    ui.gameObject.SetActive(true);

                isBuilding = true;
                ui.ShowForBuild(this, selectedPos, facingDirection);
            }
        }

        // Move to already-built connected room
        void TryEnterRoom()
        {
            if (GridManager.Instance == null)
            {
                Debug.LogError("GridManager.Instance is missing!");
                return;
            }

            Tile current = GridManager.Instance.GetTile(currentPos.x, currentPos.y);
            Tile target = GridManager.Instance.GetTile(selectedPos.x, selectedPos.y);

            Debug.Log($"[Player] Entered room of type: {target.type}");

            if (current == null || target == null)
            {
                Debug.LogError($"Invalid tile access! current: {current}, target: {target}");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            if (target == null || target.type == TileType.Empty)
            {
                Debug.Log("No room there to move into!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            // Must be connected by doors
            Direction dir = facingDirection;
            if (!current.HasDoor(dir) || !target.HasDoor(dir.Opposite()))
            {
                Debug.Log("No connecting door in that direction!");
                FindFirstObjectByType<CameraSystem>()?.Shake();
                return;
            }

            // Move to target room
            currentPos = selectedPos;
            targetPos = new Vector3(currentPos.x, currentPos.y, transform.position.z);
            isMoving = true;

            // Reset cursor to the new position (so it's aligned with player again)
            ApplyRoomRewards(target);
            selectedPos = currentPos;

            if (target.instanceDefinition != null && target.instanceDefinition.roomName == "Shop Room")
            {
                // Show "SHOP 'F'" prompt
                ShopUI.Instance?.ShowPrompt();
            }
            else
            {
                ShopUI.Instance?.HidePrompt();
            }

            // Wait until the player finishes moving before applying the travel stamina loss
            StartCoroutine(HandlePostMoveStaminaLoss());

            Tile endGoal = GridManager.Instance.GetTile(currentPos.x, currentPos.y);
            if (endGoal.type == TileType.End)
            {
                StartCoroutine(WaitUntilArrivedAtEndRoom());
            }
        }

        private IEnumerator HandlePostMoveStaminaLoss()
        {
            // Wait until the player has finished moving into the new tile
            while (isMoving)
                yield return null;

            // Now apply stamina loss for traveling
            inventory.LoseStamina(1);

            if (inventory.stamina <= 0)
            {
                Debug.Log("Out of stamina! You collapse...");
                Die();
                yield break;
            }
        }

        // Camera/player movement
        void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                isMoving = false;
        }

        private IEnumerator WaitUntilArrivedAtEndRoom()
        {
            // Wait until movement is done
            while (isMoving)
                yield return null;

            // Stay in the End Room for a short moment
            yield return new WaitForSeconds(1.5f);

            // Then trigger floor transition
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.NextFloor();
            else
                Debug.LogError("GameManager not found when trying to load next floor!");
        }

        private void ApplyRoomRewards(Tile tile)
        {
            RoomDefinition def = tile.instanceDefinition;
            if (def == null) return;

            // Player world position for popup placement
            Vector3 popupPos = transform.position + Vector3.up * 1.5f;

            // One-time collectible rewards (coins, gems)
            if (def.oneTimeReward && tile.rewardCollected)
            {
                Debug.Log($"{def.roomName} reward already collected!");
            }
            else
            {
                if (def.coinReward > 0)
                {
                    inventory.AddCoins(def.coinReward);
                    RewardPopupManager.Instance.ShowPopup($"+{def.coinReward} Coins!", Color.yellow, popupPos);
                }

                if (def.gemReward > 0)
                {
                    inventory.AddGems(def.gemReward);
                    RewardPopupManager.Instance.ShowPopup($"+{def.gemReward} Gem!", new Color(1f, 0.3f, 1f), popupPos);
                }

                if (def.oneTimeReward)
                    tile.rewardCollected = true;
            }

            // Stamina effects (repeatable or first-time only)
            bool applyStamina = def.repeatableStamina || !tile.rewardCollected;

            if (applyStamina)
            {
                if (def.staminaGain > 0)
                {
                    inventory.AddStamina(def.staminaGain);
                    RewardPopupManager.Instance.ShowPopup($"+{def.staminaGain} Stamina", Color.green, popupPos);
                }

                if (def.staminaLoss > 0)
                {
                    inventory.LoseStamina(def.staminaLoss);
                    RewardPopupManager.Instance.ShowPopup($"–{def.staminaLoss} Stamina", Color.red, popupPos);
                }
            }

            Debug.Log($"Applied effects from {def.roomName}: {inventory}");
        }

        public void SelectRoom(RoomDefinition room)
        {
            selectedRoom = room;
            Debug.Log($"Selected room: {room.roomName}");
        }

        public void EndBuildMode()
        {
            isBuilding = false;
        }

        public void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("[PlayerSystem] Player has died. Playing death animation...");

            // Stop inputs
            enabled = false;
            isMoving = false;
            isBuilding = false;

            // Let animation play even if timeScale = 0
            Time.timeScale = 1f;

            // Play death animation
            if (anim != null)
                StartCoroutine(HandleDeathSequence());
        }

        private IEnumerator HandleDeathSequence()
        {
            anim.PlayDeathAnimation();

            // Wait for the actual animation clip length
            float clipLength = anim.GetDeathClipLength();
            yield return new WaitForSecondsRealtime(clipLength + 2f);

            Debug.Log("[PlayerSystem] Death animation complete. Restarting...");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}