using UnityEngine;
using System.Collections.Generic;
using Systems;
using System.Collections;
using Managers;
using UnityEditor.Rendering.LookDev;

public class RoomSelectionUI : MonoBehaviour
{
    [Header("Visual Elements")]
    public GameObject roomSlotPrefab;
    public Transform choicesContainer;
    public GameObject toiletPaperBG;
    //public GameObject toiletRoll;
    public int choiceCount = 3;

    private PlayerSystem currentPlayer;
    [SerializeField] private RoomDatabase roomDatabase;
    private List<RoomSlotUI> activeSlots = new();
    private int currentIndex = 0;
    private bool isOpen = false;
    private bool justOpened = false;
    private Vector2Int buildTarget;
    private Direction buildDirection;

    [SerializeField] private Animator animator;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (FindObjectsByType<RoomSelectionUI>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        currentPlayer = FindFirstObjectByType<PlayerSystem>();
    }

    void Update()
    {
        if (currentPlayer == null)
            currentPlayer = FindFirstObjectByType<PlayerSystem>();

        if (!isOpen) return;

        if (justOpened)
        {
            justOpened = false;
            return;
        }

        // Make sure Update runs even if the game is paused
        if (Input.GetKeyDown(KeyCode.A))
            MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.D))
            MoveSelection(1);

        if (Input.GetKeyDown(KeyCode.E) && activeSlots.Count > 0)
            SelectRoom(activeSlots[currentIndex].roomData);
    }

    public void ShowForBuild(PlayerSystem player, Vector2Int target, Direction dir)
    {
        currentPlayer = player;
        buildTarget = target;
        buildDirection = dir;

        currentIndex = 0;

        if (toiletPaperBG != null)
            toiletPaperBG.SetActive(true);

        if (choicesContainer != null)
            choicesContainer.gameObject.SetActive(true);

        GenerateChoices();

        if (animator) animator.Play("ToiletPaper_WipeIn", 0, 0);

        if (ScreenDimmer.Instance != null)
            ScreenDimmer.Instance.FadeIn(0.55f, 0.3f);

        isOpen = true;
        justOpened = true;

        Debug.Log($"RoomSelectionUI opened — Canvas active: {gameObject.activeInHierarchy}");
    }

    public void Hide()
    {
        // Play the wipe-out animation first
        if (animator != null)
        {
            animator.Play("ToiletPaper_WipeOut", 0, 0);
            StartCoroutine(WaitAndDeactivate(0.6f)); // Match duration of your animation
        }
        else
        {
            // Fallback if no animator
            FinishHide();
        }

        Debug.Log("RoomSelectionUI hide animation started.");
    }

    private void GenerateChoices()
    {
        Debug.Log($"GenerateChoices called — prefab:{roomSlotPrefab}, container:{choicesContainer}, bg:{toiletPaperBG}");

        foreach (var slot in activeSlots)
            Destroy(slot.gameObject);
        activeSlots.Clear();

        List<string> excludedNames = new() { "Start Room", "End Room", "Key Room" };
        List<RoomDefinition> shuffled = new List<RoomDefinition>(
            roomDatabase.allRooms.FindAll(r => !excludedNames.Contains(r.roomName))
        );
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        int count = Mathf.Min(choiceCount, shuffled.Count);
        for (int i = 0; i < count; i++)
        {
            GameObject slotObj = Instantiate(roomSlotPrefab, choicesContainer);
            RoomSlotUI slot = slotObj.GetComponent<RoomSlotUI>();
            slot.Setup(shuffled[i]);
            activeSlots.Add(slot);
        }

        HighlightSlot(0);

        foreach (var slot in activeSlots)
        {
            Debug.Log($"Spawned slot: {slot.name}, position={slot.transform.position}, size={slot.GetComponent<RectTransform>().rect.size}");
        }
    }

    private void MoveSelection(int dir)
    {
        if (activeSlots.Count == 0) return;

        currentIndex = (currentIndex + dir + activeSlots.Count) % activeSlots.Count;
        HighlightSlot(currentIndex);
        Debug.Log($"Moved selection to index {currentIndex}");
    }

    private void HighlightSlot(int index)
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            activeSlots[i].SetHighlight(i == index);
        }

        Debug.Log($"Highlighting slot #{index}");
    }

    public void SelectRoom(RoomDefinition room)
    {
        FloorGenerator floorGen = FindFirstObjectByType<FloorGenerator>();

        if (room.uniquePerFloor && floorGen.shopBuiltThisFloor)
        {
            Debug.Log("You already built a shop this floor!");
            FindFirstObjectByType<CameraSystem>()?.Shake(); // optional feedback
            return;
        }

        if (currentPlayer != null)
        {
            var builder = FindFirstObjectByType<RoomBuilderSystem>();
            builder.BuildRoom(currentPlayer.currentPos, buildTarget, room.layout, room);

            if (room.uniquePerFloor)
                floorGen.shopBuiltThisFloor = true;

            Debug.Log($"Built room: {room.roomName} at {buildTarget}");
        }

        Hide();
    }

    private IEnumerator WaitAndDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        FinishHide();
    }

    private void FinishHide()
    {
        // Destroy the choice slots
        foreach (var slot in activeSlots)
            if (slot != null)
                Destroy(slot.gameObject);
        activeSlots.Clear();

        // Hide visuals
        if (toiletPaperBG != null)
            toiletPaperBG.SetActive(false);

        if (choicesContainer != null)
            choicesContainer.gameObject.SetActive(false);

        if (ScreenDimmer.Instance != null)
            ScreenDimmer.Instance.FadeOut(0.3f);

        isOpen = false;

        if (currentPlayer != null)
            currentPlayer.EndBuildMode();

        Debug.Log("RoomSelectionUI hidden and reset after animation.");
    }
}