using System.Collections.Generic;
using UnityEngine;

public static class DungeonGenerator
{
    // Tier sizes: Start(1) → T1(3) → T2(3) → T3(2) → Boss(1)
    private static readonly int[] TierSizes = { 1, 3, 3, 2, 1 };

    // Room type pools per tier (index 0 = Start tier, 4 = Boss tier)
    private static readonly RoomType[][] TierRoomTypes =
    {
        new[] { RoomType.Start },
        new[] { RoomType.Combat, RoomType.Combat, RoomType.Treasure },
        new[] { RoomType.Combat, RoomType.Rest, RoomType.Quest },
        new[] { RoomType.Combat, RoomType.Treasure },
        new[] { RoomType.Boss }
    };

    // Canvas layout constants (UI units)
    private const float CanvasWidth = 1920f;
    private const float CanvasHeight = 1080f;
    private const float MarginX = 200f;
    private const float MarginY = 100f;

    public static List<DungeonRoom> Generate()
    {
        var rooms = new List<DungeonRoom>();
        int idCounter = 0;
        var tierRooms = new List<List<DungeonRoom>>();

        // --- Create rooms per tier ---
        for (int tier = 0; tier < TierSizes.Length; tier++)
        {
            var thisGroup = new List<DungeonRoom>();
            var typePool = new List<RoomType>(TierRoomTypes[tier]);
            Shuffle(typePool);

            int count = TierSizes[tier];
            float yPos = MarginY + (CanvasHeight - 2 * MarginY) * tier / (TierSizes.Length - 1);

            for (int slot = 0; slot < count; slot++)
            {
                float xPos = count == 1
                    ? CanvasWidth / 2f
                    : MarginX + (CanvasWidth - 2 * MarginX) * slot / (count - 1);

                var room = new DungeonRoom
                {
                    id = idCounter++,
                    tier = tier,
                    type = typePool[slot % typePool.Count],
                    mapPosition = new Vector2(xPos, yPos)
                };
                thisGroup.Add(room);
                rooms.Add(room);
            }
            tierRooms.Add(thisGroup);
        }

        // --- Build forward connections ---
        for (int tier = 0; tier < tierRooms.Count - 1; tier++)
        {
            var current = tierRooms[tier];
            var next = tierRooms[tier + 1];

            // Guarantee every next-tier room has at least one incoming connection
            var unconnectedNext = new List<DungeonRoom>(next);
            Shuffle(unconnectedNext);

            foreach (var src in current)
            {
                // Each room connects to 1–2 rooms in the next tier
                int connectionCount = Random.Range(1, Mathf.Min(3, next.Count + 1));
                var candidates = new List<DungeonRoom>(next);
                Shuffle(candidates);

                int added = 0;
                foreach (var dst in candidates)
                {
                    if (added >= connectionCount) break;
                    AddConnection(src, dst);
                    unconnectedNext.Remove(dst);
                    added++;
                }
            }

            // Connect any still-unconnected next-tier rooms to a random source
            foreach (var orphan in unconnectedNext)
            {
                var src = current[Random.Range(0, current.Count)];
                AddConnection(src, orphan);
            }
        }

        return rooms;
    }

    private static void AddConnection(DungeonRoom src, DungeonRoom dst)
    {
        if (src.nextRoomIds.Contains(dst.id)) return;

        src.nextRoomIds.Add(dst.id);

        // Generate corridor event
        var evt = new CorridorEvent { type = RollCorridorEvent(), isTriggered = false };
        src.corridorTargetIds.Add(dst.id);
        src.corridorEvents.Add(evt);
    }

    private static CorridorEventType RollCorridorEvent()
    {
        // 40% chance of having any event
        if (Random.value > 0.4f) return CorridorEventType.Empty;

        float roll = Random.value;
        if (roll < 0.50f) return CorridorEventType.Curio;
        if (roll < 0.80f) return CorridorEventType.Ambush;
        return CorridorEventType.Friendly;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
