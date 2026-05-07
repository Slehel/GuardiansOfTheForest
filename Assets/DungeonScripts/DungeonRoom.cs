using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Start, Combat, Treasure, Rest, Quest, Boss }
public enum CorridorEventType { Empty, Ambush, Curio, Friendly }

[System.Serializable]
public class CorridorEvent
{
    public CorridorEventType type;
    public bool isTriggered;
}

[System.Serializable]
public class DungeonRoom
{
    public int id;
    public int tier;
    public RoomType type;
    public bool isCleared;
    public Vector2 mapPosition;

    // Ids of rooms reachable from this one (forward only)
    public List<int> nextRoomIds = new List<int>();

    // Corridor event keyed by the destination room id
    public List<int> corridorTargetIds = new List<int>();
    public List<CorridorEvent> corridorEvents = new List<CorridorEvent>();

    public CorridorEvent GetCorridorTo(int targetRoomId)
    {
        int index = corridorTargetIds.IndexOf(targetRoomId);
        return index >= 0 ? corridorEvents[index] : null;
    }
}
