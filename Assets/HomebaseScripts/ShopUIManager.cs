using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Market panel: two tabs — Buy (shop stock) and Sell (party stash).
public class ShopUIManager : MonoBehaviour
{
    public enum ShopTab { Buy, Sell }

    [Header("Tab Buttons")]
    public Button buyTabButton;
    public Button sellTabButton;

    [Header("Item List")]
    public Transform itemListContent;   // ScrollView Content (vertical layout)
    public GameObject itemButtonPrefab; // Button + TMP label

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI goldDisplay;

    private ShopTab currentTab = ShopTab.Buy;

    void Start()
    {
        buyTabButton?.onClick.AddListener(() => SwitchTab(ShopTab.Buy));
        sellTabButton?.onClick.AddListener(() => SwitchTab(ShopTab.Sell));
    }

    // Called by HomebaseManager.OpenPanel via SendMessage
    void OnPanelOpened()
    {
        currentTab = ShopTab.Buy;
        RefreshAll();
    }

    void SwitchTab(ShopTab tab)
    {
        currentTab = tab;
        RefreshAll();
    }

    void RefreshAll()
    {
        if (feedbackText != null) feedbackText.text = "";
        RefreshGold();
        ClearList();

        if (currentTab == ShopTab.Buy) RefreshBuy();
        else RefreshSell();
    }

    // ─── Buy tab ──────────────────────────────────────────────────────────────

    void RefreshBuy()
    {
        var gm = GameManager.Instance;
        var db = ItemDatabase.Instance;
        if (gm == null || db == null) return;

        if (gm.ShopStock.Count == 0)
        {
            SpawnDisabledRow("The market is empty. Embark on a dungeon to restock.");
            return;
        }

        foreach (var itemId in gm.ShopStock)
        {
            var item = db.Get(itemId);
            if (item == null) continue;

            int price = item.goldValue;
            bool canAfford = gm.Gold >= price;

            string captured = itemId;
            SpawnRow(
                $"{item.displayName}  —  {item.description}\n" +
                $"ATK:{FormatStat(item.bonusDamage)}  DEF:{FormatStat(item.bonusDefense)}  " +
                $"SPD:{FormatStat(item.bonusSpeed)}  HP:{FormatStat(item.bonusMaxHp)}  " +
                $"| {price}g",
                canAfford,
                () => OnBuyClicked(captured)
            );
        }
    }

    void OnBuyClicked(string itemId)
    {
        var gm = GameManager.Instance;
        var db = ItemDatabase.Instance;
        if (gm == null || db == null) return;

        var item = db.Get(itemId);
        if (item == null) return;

        if (gm.Gold < item.goldValue)
        {
            Show("Not enough gold!");
            return;
        }

        gm.Gold -= item.goldValue;
        gm.ShopStock.Remove(itemId);
        PartyStash.AddItem(itemId);
        Show($"Bought {item.displayName}!");
        RefreshAll();
    }

    // ─── Sell tab ─────────────────────────────────────────────────────────────

    void RefreshSell()
    {
        var stashItems = PartyStash.GetStashItems();
        if (stashItems.Count == 0)
        {
            SpawnDisabledRow("Your stash is empty.");
            return;
        }

        foreach (var item in stashItems)
        {
            int sellPrice = Mathf.CeilToInt(item.goldValue / 2f);
            string captured = item.id;
            SpawnRow(
                $"{item.displayName}  →  {sellPrice}g",
                true,
                () => OnSellClicked(captured)
            );
        }
    }

    void OnSellClicked(string itemId)
    {
        var gm = GameManager.Instance;
        var db = ItemDatabase.Instance;
        if (gm == null || db == null) return;

        var item = db.Get(itemId);
        if (item == null) return;

        PartyStash.RemoveItem(itemId);
        int sellPrice = Mathf.CeilToInt(item.goldValue / 2f);
        gm.Gold += sellPrice;
        Show($"Sold {item.displayName} for {sellPrice}g.");
        RefreshAll();
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    void SpawnRow(string labelText, bool interactable, System.Action onClick)
    {
        var go = Instantiate(itemButtonPrefab, itemListContent);
        go.GetComponentInChildren<TextMeshProUGUI>().text = labelText;
        var btn = go.GetComponent<Button>();
        btn.interactable = interactable;
        btn.onClick.AddListener(() => onClick());
    }

    void SpawnDisabledRow(string text)
    {
        var go = Instantiate(itemButtonPrefab, itemListContent);
        go.GetComponentInChildren<TextMeshProUGUI>().text = text;
        go.GetComponent<Button>().interactable = false;
    }

    void ClearList()
    {
        foreach (Transform child in itemListContent)
            Destroy(child.gameObject);
    }

    void RefreshGold()
    {
        if (goldDisplay != null && GameManager.Instance != null)
            goldDisplay.text = $"Gold: {GameManager.Instance.Gold}";
    }

    void Show(string msg) { if (feedbackText != null) feedbackText.text = msg; }

    static string FormatStat(int v) => v >= 0 ? $"+{v}" : v.ToString();
}
