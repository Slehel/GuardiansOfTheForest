using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Hero data loaded from heroes.json
[System.Serializable]
public class RecruitableHero
{
    public string id;
    public string displayName;
    public string unitClass;
    public int damage;
    public int defense;
    public int speed;
    public int maxHp;
    public int recruitCost;
}

[System.Serializable]
public class HeroList
{
    public RecruitableHero[] heroes;
}

// Barracks panel: shows active party, bench, and recruit pool.
public class BarracksUIManager : MonoBehaviour
{
    [Header("Row Prefab")]
    public GameObject heroRowPrefab;     // Button + TMP label

    [Header("Section Containers")]
    public Transform activePartyContent;
    public Transform benchContent;
    public Transform recruitPoolContent;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private List<RecruitableHero> allRecruitableHeroes = new List<RecruitableHero>();

    // Fixed base heroes always available in the party
    private static readonly string[] BaseHeroNames = { "Bear", "Fox", "Bunny", "Wolf" };

    void Start() => LoadHeroesJson();

    void OnPanelOpened() => Refresh();

    void LoadHeroesJson()
    {
        TextAsset asset = Resources.Load<TextAsset>("heroes");
        if (asset == null) { Debug.LogError("heroes.json not found in Resources/"); return; }
        allRecruitableHeroes = new List<RecruitableHero>(
            JsonUtility.FromJson<HeroList>(asset.text).heroes);
    }

    void Refresh()
    {
        if (feedbackText != null) feedbackText.text = "";

        ClearAll();

        var gm = GameManager.Instance;
        if (gm == null) return;

        // ── Active party ──────────────────────────────────────────────────────
        foreach (var heroName in gm.ActiveParty)
        {
            string captured = heroName;
            bool canBench = gm.ActiveParty.Count > 1; // keep at least 1 active
            SpawnRow(activePartyContent, heroName, $"{heroName}  [Active]",
                canBench, "Move to Bench", () => MoveToGroup(captured, gm.ActiveParty, gm.HeroBench));
        }

        // ── Bench ─────────────────────────────────────────────────────────────
        if (gm.HeroBench.Count == 0)
        {
            SpawnDisabled(benchContent, "No heroes on the bench.");
        }
        else
        {
            foreach (var heroName in gm.HeroBench)
            {
                string captured = heroName;
                bool canActivate = gm.ActiveParty.Count < 4;
                SpawnRow(benchContent, heroName, $"{heroName}  [Bench]",
                    canActivate, "Activate", () => MoveToGroup(captured, gm.HeroBench, gm.ActiveParty));
            }
        }

        // ── Recruit pool ──────────────────────────────────────────────────────
        bool anyRecruits = false;
        foreach (var hero in allRecruitableHeroes)
        {
            if (gm.RecruitedHeroes.Contains(hero.id)) continue; // already recruited

            anyRecruits = true;
            string capturedId = hero.id;
            bool canAfford = gm.Gold >= hero.recruitCost;

            string label = $"{hero.displayName}  ({hero.unitClass})\n" +
                           $"ATK:{hero.damage}  DEF:{hero.defense}  SPD:{hero.speed}  HP:{hero.maxHp}" +
                           $"  |  Cost: {hero.recruitCost}g";

            SpawnRow(recruitPoolContent, hero.id, label, canAfford,
                $"Recruit ({hero.recruitCost}g)", () => OnRecruitClicked(capturedId));
        }
        if (!anyRecruits)
            SpawnDisabled(recruitPoolContent, "No more heroes available to recruit.");
    }

    void MoveToGroup(string heroName, List<string> from, List<string> to)
    {
        from.Remove(heroName);
        to.Add(heroName);
        Refresh();
    }

    void OnRecruitClicked(string heroId)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        var hero = allRecruitableHeroes.Find(h => h.id == heroId);
        if (hero == null) return;

        if (gm.Gold < hero.recruitCost)
        {
            if (feedbackText != null) feedbackText.text = "Not enough gold!";
            return;
        }

        gm.Gold -= hero.recruitCost;
        gm.RecruitedHeroes.Add(heroId);

        // New hero goes to bench (active party may already be full)
        if (gm.ActiveParty.Count < 4)
            gm.ActiveParty.Add(hero.displayName);
        else
            gm.HeroBench.Add(hero.displayName);

        if (feedbackText != null)
            feedbackText.text = $"{hero.displayName} joined the party!";

        Refresh();
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    void SpawnRow(Transform parent, string _, string labelText, bool interactable,
                  string buttonText, System.Action onClick)
    {
        var go = Instantiate(heroRowPrefab, parent);
        var labels = go.GetComponentsInChildren<TextMeshProUGUI>();
        if (labels.Length > 0) labels[0].text = labelText;

        var btn = go.GetComponent<Button>();
        btn.interactable = interactable;
        if (labels.Length > 1) labels[1].text = buttonText;
        else btn.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        btn.onClick.AddListener(() => onClick());
    }

    void SpawnDisabled(Transform parent, string text)
    {
        var go = Instantiate(heroRowPrefab, parent);
        go.GetComponentInChildren<TextMeshProUGUI>().text = text;
        go.GetComponent<Button>().interactable = false;
    }

    void ClearAll()
    {
        foreach (Transform c in activePartyContent) Destroy(c.gameObject);
        foreach (Transform c in benchContent)       Destroy(c.gameObject);
        foreach (Transform c in recruitPoolContent) Destroy(c.gameObject);
    }
}
