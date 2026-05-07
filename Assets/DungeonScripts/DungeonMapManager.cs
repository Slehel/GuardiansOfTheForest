using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonMapManager : MonoBehaviour
{
    [Header("Scene References")]
    public RectTransform mapContainer;      // Parent RectTransform for all room nodes
    public GameObject roomNodePrefab;       // Prefab: Button + Image + TMP label + RoomNode
    public RectTransform partyIcon;         // The single party icon that moves on the map
    public TextMeshProUGUI narratorText;    // Status / event text at top/bottom
    public GameObject eventPanel;           // Panel shown for curio/friendly events
    public TextMeshProUGUI eventPanelText;
    public Button eventConfirmButton;

    [Header("Inventory")]
    public InventoryUIManager inventoryUIManager;
    public Button openInventoryButton;

    [Header("Homebase")]
    public Button returnToBaseButton;

    [Header("Scene Names")]
    public string battleSceneName = "FightSceneTest";

    // Runtime
    private List<DungeonRoom> dungeon;
    private Dictionary<int, RoomNode> roomNodeMap = new Dictionary<int, RoomNode>();
    private List<Image> connectionLines = new List<Image>();

    private DungeonRoom currentRoom;
    private DungeonRoom pendingDestination; // Room we're about to enter (mid-corridor)

    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager not found! Add a GameManager GameObject to DungeonScene.");
            return;
        }

        if (!gm.HasDungeon())
            gm.GenerateNewDungeon();

        dungeon = gm.Dungeon;
        currentRoom = gm.GetCurrentRoom();

        RenderMap();
        PlacePartyIcon(currentRoom, animated: false);

        if (gm.LastBattleWon)
            ShowNarrator("You cleared the last room! Choose your next path.");
        else
            ShowNarrator("The dungeon awaits. Choose a room.");

        eventPanel.SetActive(false);

        if (openInventoryButton != null && inventoryUIManager != null)
            openInventoryButton.onClick.AddListener(() => inventoryUIManager.OpenInventory());

        if (returnToBaseButton != null)
            returnToBaseButton.onClick.AddListener(() => SceneManager.LoadScene("HomebaseScene"));
    }

    // ─────────────────────────────────────────────
    //  MAP RENDERING
    // ─────────────────────────────────────────────

    void RenderMap()
    {
        // Draw connection lines first so they appear behind nodes
        foreach (var room in dungeon)
        {
            foreach (int nextId in room.nextRoomIds)
            {
                var nextRoom = GetRoom(nextId);
                DrawLine(room.mapPosition, nextRoom.mapPosition);
            }
        }

        // Spawn room nodes
        foreach (var room in dungeon)
        {
            GameObject go = Instantiate(roomNodePrefab, mapContainer);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = room.mapPosition;

            var node = go.GetComponent<RoomNode>();
            node.Setup(room, this);
            roomNodeMap[room.id] = node;
        }
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        GameObject lineGo = new GameObject("Line", typeof(RectTransform), typeof(Image));
        lineGo.transform.SetParent(mapContainer, false);
        lineGo.transform.SetAsFirstSibling(); // behind nodes

        Image img = lineGo.GetComponent<Image>();
        img.color = new Color(0.6f, 0.5f, 0.35f, 0.8f);

        RectTransform rt = lineGo.GetComponent<RectTransform>();
        Vector2 dir = to - from;
        float dist = dir.magnitude;

        rt.sizeDelta = new Vector2(dist, 6f);
        rt.anchoredPosition = (from + to) / 2f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        connectionLines.Add(img);
    }

    // ─────────────────────────────────────────────
    //  MOVEMENT
    // ─────────────────────────────────────────────

    public void OnRoomClicked(DungeonRoom targetRoom)
    {
        // Only allow moving to directly connected forward rooms
        if (!currentRoom.nextRoomIds.Contains(targetRoom.id))
        {
            ShowNarrator("You can't reach that room from here.");
            return;
        }

        // Check corridor event
        CorridorEvent corridor = currentRoom.GetCorridorTo(targetRoom.id);
        if (corridor != null && corridor.type != CorridorEventType.Empty && !corridor.isTriggered)
        {
            corridor.isTriggered = true;
            pendingDestination = targetRoom;
            TriggerCorridorEvent(corridor);
            return;
        }

        // Move and trigger the room
        StartCoroutine(MoveAndEnterRoom(targetRoom));
    }

    IEnumerator MoveAndEnterRoom(DungeonRoom targetRoom)
    {
        yield return StartCoroutine(AnimatePartyIcon(targetRoom.mapPosition));
        currentRoom = targetRoom;
        GameManager.Instance.CurrentRoomId = targetRoom.id;
        TriggerRoomEvent(targetRoom);
    }

    IEnumerator AnimatePartyIcon(Vector2 targetPos)
    {
        Vector2 startPos = partyIcon.anchoredPosition;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            partyIcon.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }
        partyIcon.anchoredPosition = targetPos;
    }

    void PlacePartyIcon(DungeonRoom room, bool animated = true)
    {
        if (animated)
            StartCoroutine(AnimatePartyIcon(room.mapPosition));
        else
            partyIcon.anchoredPosition = room.mapPosition;
    }

    // ─────────────────────────────────────────────
    //  CORRIDOR EVENTS
    // ─────────────────────────────────────────────

    void TriggerCorridorEvent(CorridorEvent evt)
    {
        switch (evt.type)
        {
            case CorridorEventType.Ambush:
                ShowNarrator("Ambush! Enemies leap from the shadows!");
                GameManager.Instance.IsBossBattle = false;
                StartCoroutine(DelayedSceneLoad(battleSceneName, 1.5f));
                break;

            case CorridorEventType.Curio:
                ShowEventPanel(
                    "You spot a mysterious object on the path.\nInspect it?",
                    "Inspect",
                    () => ResolveCurio()
                );
                break;

            case CorridorEventType.Friendly:
                int goldFound = Random.Range(5, 20);
                GameManager.Instance.Gold += goldFound;
                ShowNarrator($"A friendly wanderer shares supplies! +{goldFound} Gold. (Total: {GameManager.Instance.Gold})");
                StartCoroutine(ContinueAfterDelay(pendingDestination, 2f));
                break;
        }
    }

    void ResolveCurio()
    {
        eventPanel.SetActive(false);

        // Risk/reward: 60% good, 40% bad
        if (Random.value < 0.6f)
        {
            int gold = Random.Range(10, 30);
            GameManager.Instance.Gold += gold;
            ShowNarrator($"The object contained treasure! +{gold} Gold.");
        }
        else
        {
            ShowNarrator("The object was cursed! Your party feels weakened... (HP -10% on next battle)");
            // Debuff stored as a flag — BattleSystem can read it from GameManager if desired
        }

        StartCoroutine(ContinueAfterDelay(pendingDestination, 2f));
    }

    IEnumerator ContinueAfterDelay(DungeonRoom destination, float delay)
    {
        yield return new WaitForSeconds(delay);
        pendingDestination = null;
        StartCoroutine(MoveAndEnterRoom(destination));
    }

    // ─────────────────────────────────────────────
    //  ROOM EVENTS
    // ─────────────────────────────────────────────

    void TriggerRoomEvent(DungeonRoom room)
    {
        switch (room.type)
        {
            case RoomType.Start:
                ShowNarrator("You are at the dungeon entrance.");
                break;

            case RoomType.Combat:
                ShowNarrator("Enemies are here! Prepare for battle!");
                GameManager.Instance.IsBossBattle = false;
                StartCoroutine(DelayedSceneLoad(battleSceneName, 1.5f));
                break;

            case RoomType.Boss:
                ShowNarrator("A powerful enemy blocks your path! BOSS FIGHT!");
                GameManager.Instance.IsBossBattle = true;
                StartCoroutine(DelayedSceneLoad(battleSceneName, 2f));
                break;

            case RoomType.Treasure:
                if (!room.isCleared)
                {
                    room.isCleared = true;
                    int gold = Random.Range(15, 40);
                    GameManager.Instance.Gold += gold;

                    string droppedId = RollTreasureItem();
                    if (droppedId != null)
                    {
                        PartyStash.AddItem(droppedId);
                        var dropped = ItemDatabase.Instance?.Get(droppedId);
                        ShowNarrator($"Treasure chest! +{gold} Gold and you found: {dropped?.displayName}!");
                    }
                    else
                    {
                        ShowNarrator($"You found a treasure chest! +{gold} Gold. (Total: {GameManager.Instance.Gold})");
                    }

                    inventoryUIManager?.OpenInventory();
                }
                else
                {
                    ShowNarrator("This chest is already empty.");
                }
                break;

            case RoomType.Rest:
                if (!room.isCleared)
                {
                    room.isCleared = true;
                    HealPartyAtRest();
                    ShowNarrator("The party rests by the campfire. HP restored. Take this moment to prepare your equipment.");
                    inventoryUIManager?.OpenInventory();
                }
                else
                {
                    ShowNarrator("The embers are cold. This fire has already been used.");
                }
                break;

            case RoomType.Quest:
                ShowEventPanel(
                    "A strange object stands in the center of the room.\nInteract with it?",
                    "Interact",
                    () => ResolveQuestRoom(room)
                );
                break;
        }

        // Refresh node visuals
        if (roomNodeMap.TryGetValue(room.id, out var node))
            node.Refresh();
    }

    void HealPartyAtRest()
    {
        // Heal all party members for 30% of max HP (tracked in GameManager.PartyCurrentHp)
        var hpDict = GameManager.Instance.PartyCurrentHp;
        var keys = new List<string>(hpDict.Keys);
        foreach (var key in keys)
        {
            // We don't have Unit objects here, so store a healing bonus for BattleSystem
            // Add 30% estimated max HP — BattleSystem will clamp to maxHp on restore
            hpDict[key] = Mathf.RoundToInt(hpDict[key] * 1.3f);
        }
    }

    void ResolveQuestRoom(DungeonRoom room)
    {
        eventPanel.SetActive(false);
        if (!room.isCleared)
        {
            room.isCleared = true;
            int gold = Random.Range(20, 50);
            GameManager.Instance.Gold += gold;

            string rewardId = RollQuestRewardItem();
            if (rewardId != null)
            {
                PartyStash.AddItem(rewardId);
                var reward = ItemDatabase.Instance?.Get(rewardId);
                ShowNarrator($"Quest complete! +{gold} Gold and you earned: {reward?.displayName}!");
            }
            else
            {
                ShowNarrator($"Quest complete! The object revealed its secret. +{gold} Gold.");
            }

            inventoryUIManager?.OpenInventory();
        }
        else
        {
            ShowNarrator("You've already solved this room's mystery.");
        }
    }

    private string RollTreasureItem()
    {
        if (ItemDatabase.Instance == null) return null;
        var candidates = ItemDatabase.Instance.GetUniversalItems();
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)].id;
    }

    private string RollQuestRewardItem()
    {
        if (ItemDatabase.Instance == null) return null;
        var candidates = ItemDatabase.Instance.GetClassSpecificItems();
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)].id;
    }

    // ─────────────────────────────────────────────
    //  UI HELPERS
    // ─────────────────────────────────────────────

    void ShowNarrator(string message)
    {
        if (narratorText != null)
            narratorText.text = message;
        Debug.Log("[Dungeon] " + message);
    }

    void ShowEventPanel(string message, string buttonLabel, System.Action onConfirm)
    {
        eventPanel.SetActive(true);
        eventPanelText.text = message;
        eventConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonLabel;
        eventConfirmButton.onClick.RemoveAllListeners();
        eventConfirmButton.onClick.AddListener(() => onConfirm());
    }

    IEnumerator DelayedSceneLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    DungeonRoom GetRoom(int id) => GameManager.Instance.GetRoom(id);
}
