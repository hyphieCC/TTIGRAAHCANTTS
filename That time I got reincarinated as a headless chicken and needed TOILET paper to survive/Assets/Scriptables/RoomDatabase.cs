using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/RoomDatabase")]
public class RoomDatabase : ScriptableObject
{
    public List<RoomDefinition> allRooms;

    // Optional categories for filtering later
    public List<RoomDefinition> basicRooms;
    public List<RoomDefinition> specialRooms;
    public List<RoomDefinition> trapRooms;

    public RoomDefinition GetRoomByName(string name)
    {
        return allRooms.Find(r => r.roomName == name);
    }

    public RoomDefinition GetRandomRoomFrom(List<RoomDefinition> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
