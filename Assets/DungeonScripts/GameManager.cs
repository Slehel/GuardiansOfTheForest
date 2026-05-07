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

    // ── Homebase state ────────────────────────────────────────────────────────

    // Ability upgrades: abilityName → upgrade level (0–3, persists across dungeons)
    public Dictionary<string, int> AbilityUpgradeLevels { get; private set; } = new Dictionary<string, int>();

    // Hero wildness values (heroName → wildness, persists)
    public Dictionary<string, int> HeroWildness { get; private set; } = new Dictionary<string, int>();

    // Active party: up to 4 hero names used in battle
    public List<string> ActiveParty { get; private set; } = new List<string> { "Bear", "Fox", "Bunny", "Wolf" };

    // Bench: recruited heroes not in active party
    public List<string> HeroBench { get; private set; } = new List<string>();

    // IDs of heroes recruited from heroes.json pool
    public List<string> RecruitedHeroes { get; private set; } = new List<string>();

    // Shop stock: item IDs currently for sale (4 items, refreshed each homebase visit)
    public List<string> ShopStock { get; private set; } = new List<string>();

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
        RefreshShopStock();
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

    // ─── Ability upgrades ─────────────────────────────────────────────────────

    public int GetAbilityUpgradeLevel(string abilityName)
    {
        AbilityUpgradeLevels.TryGetValue(abilityName, out int level);
        return level;
    }

    public void SetAbilityUpgradeLevel(string abilityName, int level)
        => AbilityUpgradeLevels[abilityName] = level;

    // ─── Hero wildness ────────────────────────────────────────────────────────

    public int GetHeroWildness(string heroName)
    {
        HeroWildness.TryGetValue(heroName, out int w);
        return w;
    }

    public void SetHeroWildness(string heroName, int value)
        => HeroWildness[heroName] = Mathf.Max(0, value);

    // ─── Shop stock ───────────────────────────────────────────────────────────

    public void RefreshShopStock()
    {
        ShopStock.Clear();
        if (ItemDatabase.Instance == null) return;

        var pool = ItemDatabase.Instance.GetUniversalItems();
        // Pick up to 4 unique random items
        var shuffled = new List<ItemData>(pool);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        int count = Mathf.Min(4, shuffled.Count);
        for (int i = 0; i < count; i++)
            ShopStock.Add(shuffled[i].id);
    }
}
