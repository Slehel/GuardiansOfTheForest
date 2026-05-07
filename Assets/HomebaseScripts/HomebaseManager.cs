using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Scene controller for HomebaseScene.
// Manages which building panel is open and the Dungeon Gate embark flow.
public class HomebaseManager : MonoBehaviour
{
    [Header("Building Panels")]
    public GameObject skillForgePanel;
    public GameObject marketPanel;
    public GameObject barracksPanel;
    public GameObject tavernPanel;

    [Header("Dungeon Gate Confirmation")]
    public GameObject embarkConfirmPanel;
    public Button embarkConfirmButton;
    public Button embarkCancelButton;

    [Header("HUD")]
    public TextMeshProUGUI goldDisplay;

    [Header("Scene Names")]
    public string dungeonSceneName = "DungeonScene";

    void Start()
    {
        // Ensure all panels are closed at scene start
        CloseAll();

        if (embarkConfirmPanel != null) embarkConfirmPanel.SetActive(false);

        if (embarkConfirmButton != null) embarkConfirmButton.onClick.AddListener(OnEmbarkConfirmed);
        if (embarkCancelButton  != null) embarkCancelButton.onClick.AddListener(CloseEmbarkPanel);

        RefreshGoldDisplay();

        // Refresh shop stock on homebase entry
        GameManager.Instance?.RefreshShopStock();
    }

    void Update()
    {
        // Keep gold display current (gold may change inside panels)
        RefreshGoldDisplay();
    }

    // ─── Called by HomebaseBuilding ───────────────────────────────────────────

    public void OpenBuilding(BuildingType type)
    {
        CloseAll();
        switch (type)
        {
            case BuildingType.SkillForge:  OpenPanel(skillForgePanel);  break;
            case BuildingType.Market:      OpenPanel(marketPanel);      break;
            case BuildingType.Barracks:    OpenPanel(barracksPanel);    break;
            case BuildingType.Tavern:      OpenPanel(tavernPanel);      break;
            case BuildingType.DungeonGate: ShowEmbarkConfirm();         break;
        }
    }

    // Close button wired in Inspector on each panel
    public void CloseCurrentPanel() => CloseAll();

    // ─── Panel helpers ────────────────────────────────────────────────────────

    void OpenPanel(GameObject panel)
    {
        if (panel == null) return;
        panel.SetActive(true);

        // Notify the UIManager so it can refresh its content
        panel.SendMessage("OnPanelOpened", SendMessageOptions.DontRequireReceiver);
    }

    void CloseAll()
    {
        if (skillForgePanel != null) skillForgePanel.SetActive(false);
        if (marketPanel     != null) marketPanel.SetActive(false);
        if (barracksPanel   != null) barracksPanel.SetActive(false);
        if (tavernPanel     != null) tavernPanel.SetActive(false);
    }

    // ─── Dungeon Gate ─────────────────────────────────────────────────────────

    void ShowEmbarkConfirm()
    {
        if (embarkConfirmPanel != null) embarkConfirmPanel.SetActive(true);
    }

    void CloseEmbarkPanel()
    {
        if (embarkConfirmPanel != null) embarkConfirmPanel.SetActive(false);
    }

    void OnEmbarkConfirmed()
    {
        CloseEmbarkPanel();
        GameManager.Instance?.GenerateNewDungeon();
        SceneManager.LoadScene(dungeonSceneName);
    }

    // ─── Gold HUD ─────────────────────────────────────────────────────────────

    void RefreshGoldDisplay()
    {
        if (goldDisplay != null && GameManager.Instance != null)
            goldDisplay.text = $"Gold: {GameManager.Instance.Gold}";
    }
}
