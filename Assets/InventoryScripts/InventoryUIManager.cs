using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Dungeon-scene inventory panel.
// State machine: SelectHero → SelectSlot → SelectItem
public class InventoryUIManager : MonoBehaviour
{
    public enum InventoryState { Closed, SelectHero, SelectSlot, SelectItem }

    [Header("Root Panel")]
    public GameObject inventoryPanel;

    [Header("Hero List Panel")]
    public GameObject heroListPanel;
    public Button[] heroButtons;         // 4 buttons, one per hero
    public TextMeshProUGUI[] heroLabels;

    [Header("Slot Panel")]
    public GameObject slotPanel;
    public Button[] slotButtons;         // 6 slot buttons
    public TextMeshProUGUI[] slotLabels; // slot name + currently equipped item
    public TextMeshProUGUI selectedHeroLabel;

    [Header("Item Picker Panel")]
    public GameObject itemPickerPanel;
    public Transform itemListContent;    // parent for dynamically spawned buttons
    public GameObject itemButtonPrefab;  // Button + TextMeshProUGUI
    public Button unequipButton;
    public TextMeshProUGUI selectedSlotLabel;

    // Hero name → class mapping, must match prefab unitName / UnitClass values
    private static readonly string[] HeroNames   = { "Bear", "Fox", "Bunny", "Wolf" };
    private static readonly string[] HeroClasses = { "Paladin", "Ranger", "Fighter", "Rogue" };
    private static readonly SlotType[] SlotOrder  =
    {
        SlotType.Helmet, SlotType.BodyArmor, SlotType.Weapon,
        SlotType.Accessory1, SlotType.Accessory2, SlotType.ClassSpecific
    };
    private static readonly string[] SlotBaseLabels =
    {
        "Helmet", "Body Armor", "Weapon", "Accessory 1", "Accessory 2", ""  // last is class-specific
    };

    private InventoryState currentState = InventoryState.Closed;
    private int    selectedHeroIndex;
    private string SelectedHeroName  => HeroNames[selectedHeroIndex];
    private string SelectedHeroClass => HeroClasses[selectedHeroIndex];
    private SlotType selectedSlot;

    void Start()
    {
        inventoryPanel.SetActive(false);
        WireHeroButtons();
        WireSlotButtons();
        unequipButton.onClick.AddListener(OnUnequip);
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        TransitionTo(InventoryState.SelectHero);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        currentState = InventoryState.Closed;
    }

    // ─── Wiring ───────────────────────────────────────────────────────────────

    void WireHeroButtons()
    {
        for (int i = 0; i < heroButtons.Length && i < HeroNames.Length; i++)
        {
            int idx = i;  // capture for closure
            heroButtons[i].onClick.AddListener(() => OnHeroSelected(idx));
        }
    }

    void WireSlotButtons()
    {
        for (int i = 0; i < slotButtons.Length && i < SlotOrder.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(SlotOrder[idx]));
        }
    }

    // ─── State machine ────────────────────────────────────────────────────────

    void TransitionTo(InventoryState newState)
    {
        currentState = newState;
        heroListPanel.SetActive(newState == InventoryState.SelectHero);
        slotPanel.SetActive(newState == InventoryState.SelectSlot);
        itemPickerPanel.SetActive(newState == InventoryState.SelectItem);

        switch (newState)
        {
            case InventoryState.SelectHero: RefreshHeroList();   break;
            case InventoryState.SelectSlot: RefreshSlotPanel();  break;
            case InventoryState.SelectItem: RefreshItemPicker(); break;
        }
    }

    // ─── Hero list ────────────────────────────────────────────────────────────

    void RefreshHeroList()
    {
        for (int i = 0; i < heroLabels.Length && i < HeroNames.Length; i++)
            heroLabels[i].text = $"{HeroNames[i]}  ({HeroClasses[i]})";
    }

    void OnHeroSelected(int index)
    {
        selectedHeroIndex = index;
        TransitionTo(InventoryState.SelectSlot);
    }

    // ─── Slot panel ───────────────────────────────────────────────────────────

    void RefreshSlotPanel()
    {
        selectedHeroLabel.text = $"{SelectedHeroName}  —  {SelectedHeroClass}";
        var equipped = GameManager.Instance?.GetEquippedItems(SelectedHeroName) ?? new EquippedItems();

        for (int i = 0; i < slotLabels.Length && i < SlotOrder.Length; i++)
        {
            SlotType slot = SlotOrder[i];
            string slotName = i < SlotBaseLabels.Length - 1
                ? SlotBaseLabels[i]
                : ItemData.ClassSpecificSlotLabel(SelectedHeroClass);  // last slot

            string equippedId = equipped.GetSlot(slot);
            string equippedLabel = "(Empty)";
            if (!string.IsNullOrEmpty(equippedId))
            {
                var item = ItemDatabase.Instance?.Get(equippedId);
                equippedLabel = item?.displayName ?? equippedId;
            }

            slotLabels[i].text = $"{slotName}: {equippedLabel}";
        }
    }

    void OnSlotSelected(SlotType slot)
    {
        selectedSlot = slot;
        TransitionTo(InventoryState.SelectItem);
    }

    // ─── Item picker ──────────────────────────────────────────────────────────

    void RefreshItemPicker()
    {
        // Determine displayed slot name
        int slotIndex = System.Array.IndexOf(SlotOrder, selectedSlot);
        string slotName = (slotIndex >= 0 && slotIndex < SlotBaseLabels.Length - 1)
            ? SlotBaseLabels[slotIndex]
            : ItemData.ClassSpecificSlotLabel(SelectedHeroClass);
        selectedSlotLabel.text = slotName;

        // Show/hide unequip button based on whether something is equipped
        var equipped = GameManager.Instance?.GetEquippedItems(SelectedHeroName) ?? new EquippedItems();
        string currentId = equipped.GetSlot(selectedSlot);
        unequipButton.interactable = !string.IsNullOrEmpty(currentId);

        // Clear old item buttons
        foreach (Transform child in itemListContent)
            Destroy(child.gameObject);

        // Get stash items valid for this slot and class
        var stashIds = PartyStash.GetStash();
        var candidates = ItemDatabase.Instance != null
            ? ItemDatabase.Instance.GetForSlot(selectedSlot, SelectedHeroClass, stashIds)
            : new List<ItemData>();

        if (candidates.Count == 0)
        {
            var emptyGo = Instantiate(itemButtonPrefab, itemListContent);
            emptyGo.GetComponent<Button>().interactable = false;
            emptyGo.GetComponentInChildren<TextMeshProUGUI>().text = "(No items available)";
            return;
        }

        foreach (var item in candidates)
        {
            string capturedId = item.id;
            var go = Instantiate(itemButtonPrefab, itemListContent);
            go.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{item.displayName}  |  " +
                $"ATK:{FormatStat(item.bonusDamage)}  " +
                $"DEF:{FormatStat(item.bonusDefense)}  " +
                $"SPD:{FormatStat(item.bonusSpeed)}  " +
                $"HP:{FormatStat(item.bonusMaxHp)}";
            go.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(capturedId));
        }
    }

    void OnItemSelected(string itemId)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Swap: send currently equipped item back to stash
        string oldId = gm.GetEquippedItems(SelectedHeroName).GetSlot(selectedSlot);
        if (!string.IsNullOrEmpty(oldId))
            PartyStash.AddItem(oldId);

        // Equip new item, remove from stash
        PartyStash.RemoveItem(itemId);
        gm.SetEquippedItem(SelectedHeroName, selectedSlot, itemId);

        TransitionTo(InventoryState.SelectSlot);
    }

    void OnUnequip()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        string currentId = gm.GetEquippedItems(SelectedHeroName).GetSlot(selectedSlot);
        if (!string.IsNullOrEmpty(currentId))
        {
            PartyStash.AddItem(currentId);
            gm.SetEquippedItem(SelectedHeroName, selectedSlot, "");
        }

        TransitionTo(InventoryState.SelectSlot);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    // Back buttons in the UI call these directly via onClick in the Inspector
    public void BackToHeroList()  => TransitionTo(InventoryState.SelectHero);
    public void BackToSlotPanel() => TransitionTo(InventoryState.SelectSlot);

    private static string FormatStat(int value)
        => value >= 0 ? $"+{value}" : value.ToString();
}
