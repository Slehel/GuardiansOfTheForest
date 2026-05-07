using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Dungeon state
    public List<DungeonRoom> Dungeon { get; private set; }
    public int CurrentRoomId { get; set; }
    public bool LastBattleWon { get; set; }
    public bool IsBossBattle { get; set; }

    // Party resources
    public int Gold { get; set; }

    // Party HP persisted across scene loads (unitName → currentHp)
    public Dictionary<string, int> PartyCurrentHp { get; private set; } = new Dictionary<string, int>();

    // Inventory: shared item stash (list of item IDs)
    public List<string> PartyStash { get; private set; } = new List<string>();

    // Per-hero equipment (unitName → equipped slots)
    private Dictionary<string, EquippedItems> partyEquipment = new Dictionary<string, EquippedItems>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateNewDungeon()
    {
        Dungeon = DungeonGenerator.Generate();
        CurrentRoomId = 0;
        Gold = 0;
        PartyCurrentHp.Clear();
        PartyStash.Clear();
        partyEquipment.Clear();
        LastBattleWon = false;
        IsBossBattle = false;
    }

    public DungeonRoom GetCurrentRoom() => GetRoom(CurrentRoomId);

    public DungeonRoom GetRoom(int id)
    {
        if (Dungeon == null) return null;
        return Dungeon.Find(r => r.id == id);
    }

    public void MarkCurrentRoomCleared()
    {
        var room = GetCurrentRoom();
        if (room != null) room.isCleared = true;
    }

    // Called by BattleSystem before leaving to FightSceneTest
    public void SavePartyHp(List<Unit> units)
    {
        PartyCurrentHp.Clear();
        foreach (var unit in units)
        {
            if (unit.isPlayerCharacter)
                PartyCurrentHp[unit.unitName] = unit.currentHp;
        }
    }

    // Called by BattleSystem when returning to DungeonScene
    public void RestorePartyHp(List<Unit> units)
    {
        foreach (var unit in units)
        {
            if (unit.isPlayerCharacter && PartyCurrentHp.TryGetValue(unit.unitName, out int hp))
                unit.currentHp = Mathf.Max(hp, 0);
        }
    }

    public bool HasDungeon() => Dungeon != null && Dungeon.Count > 0;

    // ─── Equipment ───────────────────────────────────────────────────────────

    public EquippedItems GetEquippedItems(string heroName)
    {
        if (!partyEquipment.TryGetValue(heroName, out var eq))
        {
            eq = new EquippedItems();
            partyEquipment[heroName] = eq;
        }
        return eq;
    }

    public void SetEquippedItem(string heroName, SlotType slot, string itemId)
        => GetEquippedItems(heroName).SetSlot(slot, itemId);
}
