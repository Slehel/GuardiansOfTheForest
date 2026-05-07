using System;
using UnityEngine;

// Attach to each hero prefab (Player, Fox, Bunny, Wolf).
// Computes stat bonuses from equipped items stored in GameManager.
public class HeroInventory : MonoBehaviour
{
    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private EquippedItems GetEquipped()
    {
        if (GameManager.Instance == null || unit == null) return new EquippedItems();
        return GameManager.Instance.GetEquippedItems(unit.unitName);
    }

    private ItemData GetItem(SlotType slot)
    {
        string id = GetEquipped().GetSlot(slot);
        return ItemDatabase.Instance?.Get(id);
    }

    public int GetBonusDamage()
    {
        int total = 0;
        foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
            total += GetItem(slot)?.bonusDamage ?? 0;
        return total;
    }

    public int GetBonusDefense()
    {
        int total = 0;
        foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
            total += GetItem(slot)?.bonusDefense ?? 0;
        return total;
    }

    public int GetBonusSpeed()
    {
        int total = 0;
        foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
            total += GetItem(slot)?.bonusSpeed ?? 0;
        return total;
    }

    public int GetBonusMaxHp()
    {
        int total = 0;
        foreach (SlotType slot in Enum.GetValues(typeof(SlotType)))
            total += GetItem(slot)?.bonusMaxHp ?? 0;
        return total;
    }
}
