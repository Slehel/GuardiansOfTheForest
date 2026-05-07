using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Ability upgrade panel.
// Shows each of the 4 base abilities with their current stats and an Upgrade button.
public class SkillForgeUIManager : MonoBehaviour
{
    [Header("Ability Card Prefab")]
    public GameObject abilityCardPrefab;  // Button with child TMP label
    public Transform cardContainer;       // Vertical layout group parent

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private const int MaxUpgradeLevel = 3;
    private const int CostPerLevel = 30;  // multiplied by (currentLevel + 1)

    // Ability names matching abilities.json exactly
    private static readonly string[] AbilityNames = { "Fireball", "Ice Blast", "Lightning Strike", "Heal" };

    // Called by HomebaseManager.OpenPanel via SendMessage
    void OnPanelOpened() => Refresh();

    void Refresh()
    {
        if (feedbackText != null) feedbackText.text = "";

        // Clear old cards
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        var gm = GameManager.Instance;

        foreach (var abilityName in AbilityNames)
        {
            int currentLevel = gm != null ? gm.GetAbilityUpgradeLevel(abilityName) : 0;
            int cost = CostPerLevel * (currentLevel + 1);
            bool maxed = currentLevel >= MaxUpgradeLevel;
            bool canAfford = gm != null && gm.Gold >= cost;

            var card = Instantiate(abilityCardPrefab, cardContainer);

            // Main label: ability name + level + stat preview
            var label = card.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = BuildCardText(abilityName, currentLevel);

            // Upgrade button
            var btn = card.GetComponentInChildren<Button>();
            if (btn != null)
            {
                string captured = abilityName;
                btn.interactable = !maxed && canAfford;
                btn.GetComponentInChildren<TextMeshProUGUI>().text =
                    maxed ? "MAX" : $"Upgrade ({cost}g)";
                btn.onClick.AddListener(() => OnUpgradeClicked(captured));
            }
        }
    }

    void OnUpgradeClicked(string abilityName)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        int currentLevel = gm.GetAbilityUpgradeLevel(abilityName);
        if (currentLevel >= MaxUpgradeLevel) return;

        int cost = CostPerLevel * (currentLevel + 1);
        if (gm.Gold < cost)
        {
            if (feedbackText != null) feedbackText.text = "Not enough gold!";
            return;
        }

        gm.Gold -= cost;
        gm.SetAbilityUpgradeLevel(abilityName, currentLevel + 1);

        if (feedbackText != null)
            feedbackText.text = $"{abilityName} upgraded to level {currentLevel + 1}!";

        Refresh();
    }

    static string BuildCardText(string abilityName, int level)
    {
        // Show relative bonuses so the player understands what they're buying
        int nextLevel = level + 1;
        bool maxed = level >= MaxUpgradeLevel;

        string levelStr = maxed ? "MAX" : $"Lv {level}/{MaxUpgradeLevel}";
        string bonusStr = level > 0
            ? $"+{level * 25}% dmg  |  -{level * 0.5f:0.#} cd  |  +{level * 10}% stun/poison"
            : "No upgrades yet";

        string nextStr = maxed ? "" :
            $"\nNext: +25% dmg | -0.5 cd | +10% stun/poison";

        return $"{abilityName}  [{levelStr}]\n{bonusStr}{nextStr}";
    }
}
