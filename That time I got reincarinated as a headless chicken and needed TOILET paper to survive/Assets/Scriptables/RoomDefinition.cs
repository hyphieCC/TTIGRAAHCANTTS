using UnityEngine;

[CreateAssetMenu(menuName = "Data/RoomDefinition")]
public class RoomDefinition : ScriptableObject
{
    [Header("Room Info")]
    public string roomName;
    public RoomType layout;

    [Header("Sprites (based on entry direction)")]
    public Sprite spriteFromUp;
    public Sprite spriteFromDown;
    public Sprite spriteFromLeft;
    public Sprite spriteFromRight;

    [Header("Build Costs")]
    public int gemCost;

    [Header("On Enter Rewards")]
    public int coinReward;
    public int gemReward;
    public int staminaGain;
    public int staminaLoss;

    [Header("Special")]
    public bool requiresEndKey; // for End Rooms
    public bool grantsEndKey;

    [Header("Reward Rules")]
    public bool oneTimeReward; // true = collectible only once
    public bool repeatableStamina; // true = stamina effects apply every entry

    [Header("Special Room Rules")]
    public bool uniquePerFloor = false;

    [Header("Room Characters")]
    public GameObject shopkeeperPrefab;
    public GameObject doorkeeperPrefab;
    public GameObject eggPrefab;
}
