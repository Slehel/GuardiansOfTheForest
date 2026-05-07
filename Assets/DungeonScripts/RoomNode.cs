using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Attached to each room button in the dungeon map Canvas.
public class RoomNode : MonoBehaviour
{
    public DungeonRoom RoomData { get; private set; }

    private Button button;
    private Image icon;
    private TextMeshProUGUI label;
    private DungeonMapManager manager;

    public void Setup(DungeonRoom room, DungeonMapManager mapManager)
    {
        RoomData = room;
        manager = mapManager;

        button = GetComponent<Button>();
        icon = GetComponent<Image>();
        label = GetComponentInChildren<TextMeshProUGUI>();

        ApplyVisuals();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.OnRoomClicked(RoomData));
    }

    public void Refresh()
    {
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (label != null)
            label.text = RoomLabel();

        if (icon != null)
            icon.color = RoomColor();
    }

    private string RoomLabel() => RoomData.type switch
    {
        RoomType.Start   => "Start",
        RoomType.Combat  => "⚔",   // ⚔
        RoomType.Treasure=> "★",   // ★
        RoomType.Rest    => "☽",   // ☽
        RoomType.Quest   => "?",
        RoomType.Boss    => "☠",   // ☠
        _                => "?"
    };

    private Color RoomColor() => RoomData.type switch
    {
        RoomType.Start   => new Color(0.2f, 0.8f, 0.2f),
        RoomType.Combat  => new Color(0.8f, 0.2f, 0.2f),
        RoomType.Treasure=> new Color(1.0f, 0.8f, 0.0f),
        RoomType.Rest    => new Color(0.3f, 0.5f, 0.9f),
        RoomType.Quest   => new Color(0.6f, 0.2f, 0.9f),
        RoomType.Boss    => new Color(0.4f, 0.0f, 0.0f),
        _                => Color.grey
    };
}
