using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    private Dictionary<string, ItemData> lookup = new Dictionary<string, ItemData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadItems();
    }

    void LoadItems()
    {
        TextAsset asset = Resources.Load<TextAsset>("items");
        if (asset == null)
        {
            Debug.LogError("ItemDatabase: items.json not found in Resources/");
            return;
        }
        ItemList list = JsonUtility.FromJson<ItemList>(asset.text);
        foreach (var item in list.items)
            lookup[item.id] = item;

        Debug.Log($"ItemDatabase: loaded {lookup.Count} items.");
    }

    public ItemData Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        lookup.TryGetValue(id, out var item);
        return item;
    }

    public List<ItemData> GetAll() => new List<ItemData>(lookup.Values);

    // Returns stash items that can go into the given slot for the given hero class.
    public List<ItemData> GetForSlot(SlotType slot, string unitClass, List<string> stashIds)
    {
        var result = new List<ItemData>();
        foreach (var id in stashIds)
        {
            var item = Get(id);
            if (item == null) continue;

            SlotType itemSlot = ItemData.SlotTypeForItemType(item.itemType);

            // Accessories can fill either Accessory1 or Accessory2
            bool slotMatch = itemSlot == slot ||
                             (item.itemType == ItemType.Accessory &&
                              (slot == SlotType.Accessory1 || slot == SlotType.Accessory2));

            if (!slotMatch) continue;
            if (!string.IsNullOrEmpty(item.requiredClass) && item.requiredClass != unitClass) continue;

            result.Add(item);
        }
        return result;
    }

    public List<ItemData> GetUniversalItems()
        => GetAll().FindAll(i => string.IsNullOrEmpty(i.requiredClass));

    public List<ItemData> GetClassSpecificItems()
        => GetAll().FindAll(i => !string.IsNullOrEmpty(i.requiredClass));
}
