using System.Collections.Generic;

// Static accessor for the party's unequipped item pool, backed by GameManager.
public static class PartyStash
{
    public static List<string> GetStash()
        => GameManager.Instance?.PartyStash ?? new List<string>();

    public static void AddItem(string itemId)
        => GameManager.Instance?.PartyStash.Add(itemId);

    public static void RemoveItem(string itemId)
        => GameManager.Instance?.PartyStash.Remove(itemId);  // removes first occurrence

    public static bool Contains(string itemId)
        => GameManager.Instance?.PartyStash.Contains(itemId) ?? false;

    public static List<ItemData> GetStashItems()
    {
        var result = new List<ItemData>();
        foreach (var id in GetStash())
        {
            var item = ItemDatabase.Instance?.Get(id);
            if (item != null) result.Add(item);
        }
        return result;
    }
}
