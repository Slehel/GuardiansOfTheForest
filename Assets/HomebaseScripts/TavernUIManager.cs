using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Tavern panel: Full Rest (restore all HP) and Calm (reduce wildness per hero).
public class TavernUIManager : MonoBehaviour
{
    private const int FullRestCost  = 20;
    private const int CalmCost      = 10;
    private const int CalmAmount    = 3;

    [Header("Full Rest")]
    public Button fullRestButton;
    public TextMeshProUGUI fullRestLabel;  // shows cost + party HP summary

    [Header("Hero Wildness List")]
    public Transform heroListContent;      // vertical layout, spawns hero rows
    public GameObject heroWildnessRowPrefab; // row with hero name, wildness value, Calm button

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private static readonly string[] HeroNames = { "Bear", "Fox", "Bunny", "Wolf" };

    void Start()
    {
        if (fullRestButton != null)
            fullRestButton.onClick.AddListener(OnFullRestClicked);
    }

    void OnPanelOpened() => Refresh();

    void Refresh()
    {
        if (feedbackText != null) feedbackText.text = "";

        var gm = GameManager.Instance;

        // Full Rest button label
        if (fullRestLabel != null)
        {
            bool canAfford = gm != null && gm.Gold >= FullRestCost;
            fullRestLabel.text = $"Full Rest — Restore all HP  ({FullRestCost}g)";
            if (fullRestButton != null) fullRestButton.interactable = canAfford;
        }

        // Hero wildness rows
        foreach (Transform child in heroListContent)
            Destroy(child.gameObject);

        foreach (var heroName in HeroNames)
        {
            int wildness = gm != null ? gm.GetHeroWildness(heroName) : 0;
            bool canCalm = gm != null && gm.Gold >= CalmCost && wildness > 0;

            string captured = heroName;
            var row = Instantiate(heroWildnessRowPrefab, heroListContent);
            var labels = row.GetComponentsInChildren<TextMeshProUGUI>();

            if (labels.Length >= 1)
                labels[0].text = $"{heroName}  —  Wildness: {wildness}";

            var btn = row.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.interactable = canCalm;
                if (labels.Length >= 2)
                    labels[1].text = $"Calm ({CalmCost}g)";
                else
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = $"Calm ({CalmCost}g)";

                btn.onClick.AddListener(() => OnCalmClicked(captured));
            }
        }
    }

    void OnFullRestClicked()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.Gold < FullRestCost) { Show("Not enough gold!"); return; }

        gm.Gold -= FullRestCost;

        // Max out all stored HP values (BattleSystem will clamp to TotalMaxHp on restore)
        foreach (var heroName in HeroNames)
        {
            // Store a very large value; BattleSystem clamps to TotalMaxHp in RestorePartyHp
            gm.PartyCurrentHp[heroName] = 9999;
        }

        Show("The party rests and recovers fully!");
        Refresh();
    }

    void OnCalmClicked(string heroName)
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.Gold < CalmCost) { Show("Not enough gold!"); return; }

        gm.Gold -= CalmCost;
        int current = gm.GetHeroWildness(heroName);
        gm.SetHeroWildness(heroName, Mathf.Max(0, current - CalmAmount));

        Show($"{heroName}'s wildness reduced by {CalmAmount}.");
        Refresh();
    }

    void Show(string msg) { if (feedbackText != null) feedbackText.text = msg; }
}
