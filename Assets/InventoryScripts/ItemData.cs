using UnityEngine;

public enum SlotType { Helmet, BodyArmor, Weapon, Accessory1, Accessory2, ClassSpecific }

public enum ItemType { Weapon, Helmet, BodyArmor, Accessory, Shield, Quiver, OffHand }

[System.Serializable]
public class ItemData
{
    public string id;
    public string displayName;
    public string description;
    public ItemType itemType;
    public string requiredClass;  // "" = any class

    public int bonusDamage;
    public int bonusDefense;
    public int bonusSpeed;
    public int bonusMaxHp;
    public int goldValue;   // buy price in the Market; sell price = ceil(goldValue / 2)

    public static SlotType SlotTypeForItemType(ItemType type) => type switch
    {
        ItemType.Weapon    => SlotType.Weapon,
        ItemType.Helmet    => SlotType.Helmet,
        ItemType.BodyArmor => SlotType.BodyArmor,
        ItemType.Accessory => SlotType.Accessory1,  // UI resolves Accessory1 vs 2
        ItemType.Shield    => SlotType.ClassSpecific,
        ItemType.Quiver    => SlotType.ClassSpecific,
        ItemType.OffHand   => SlotType.ClassSpecific,
        _                  => SlotType.Weapon
    };

    public static string ClassSpecificSlotLabel(string unitClass) => unitClass switch
    {
        "Paladin" => "Shield",
        "Ranger"  => "Quiver",
        "Fighter" => "Off-hand",
        "Rogue"   => "Off-hand",
        _         => "Extra"
    };
}

[System.Serializable]
public class EquippedItems
{
    public string helmetId        = "";
    public string bodyArmorId     = "";
    public string weaponId        = "";
    public string accessory1Id    = "";
    public string accessory2Id    = "";
    public string classSpecificId = "";

    public string GetSlot(SlotType slot) => slot switch
    {
        SlotType.Helmet        => helmetId,
        SlotType.BodyArmor     => bodyArmorId,
        SlotType.Weapon        => weaponId,
        SlotType.Accessory1    => accessory1Id,
        SlotType.Accessory2    => accessory2Id,
        SlotType.ClassSpecific => classSpecificId,
        _                      => ""
    };

    public void SetSlot(SlotType slot, string itemId)
    {
        switch (slot)
        {
            case SlotType.Helmet:         helmetId        = itemId; break;
            case SlotType.BodyArmor:      bodyArmorId     = itemId; break;
            case SlotType.Weapon:         weaponId        = itemId; break;
            case SlotType.Accessory1:     accessory1Id    = itemId; break;
            case SlotType.Accessory2:     accessory2Id    = itemId; break;
            case SlotType.ClassSpecific:  classSpecificId = itemId; break;
        }
    }
}

// JsonUtility wrapper — same pattern as AbilityList
[System.Serializable]
public class ItemList
{
    public ItemData[] items;
}
