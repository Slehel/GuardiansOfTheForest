using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum BuildingType { SkillForge, Market, Barracks, Tavern, DungeonGate, Decorative }

// Attach to each building zone GameObject in the homebase Canvas.
// Handles hover glow animation and routes clicks to HomebaseManager.
[RequireComponent(typeof(Image))]
public class HomebaseBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Building Config")]
    public BuildingType buildingType;
    public bool isInteractable = true;

    [Header("UI References")]
    public Image glowImage;               // child Image — warm gold glow, alpha=0 at rest
    public TextMeshProUGUI nameLabel;     // child TMP — building name

    private HomebaseManager manager;
    private Coroutine pulseCoroutine;
    private Coroutine fadeCoroutine;

    void Start()
    {
        manager = FindObjectOfType<HomebaseManager>();

        if (glowImage != null)
            glowImage.color = new Color(1f, 0.85f, 0.2f, 0f);

        if (nameLabel != null)
            nameLabel.color = new Color(nameLabel.color.r, nameLabel.color.g, nameLabel.color.b, 0f);

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClicked);
            btn.interactable = isInteractable;
        }
    }

    public void OnPointerEnter(PointerEventData _)
    {
        if (!isInteractable) return;
        StopFade();
        if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);

        // Fade glow and label in, then start pulsing
        fadeCoroutine = StartCoroutine(FadeGlow(0.6f, 0.2f));
        if (nameLabel != null) StartCoroutine(FadeLabel(1f, 0.2f));
        pulseCoroutine = StartCoroutine(PulseGlow());
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (!isInteractable) return;
        if (pulseCoroutine != null) { StopCoroutine(pulseCoroutine); pulseCoroutine = null; }
        StopFade();
        fadeCoroutine = StartCoroutine(FadeGlow(0f, 0.25f));
        if (nameLabel != null) StartCoroutine(FadeLabel(0f, 0.25f));
    }

    void OnClicked()
    {
        if (!isInteractable || manager == null) return;
        manager.OpenBuilding(buildingType);
    }

    // ─── Glow coroutines ──────────────────────────────────────────────────────

    IEnumerator PulseGlow()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * 1.5f, 1f);
            float a = Mathf.Lerp(0.35f, 0.85f, t);
            if (glowImage != null)
                glowImage.color = new Color(1f, 0.85f, 0.2f, a);
            yield return null;
        }
    }

    IEnumerator FadeGlow(float target, float duration)
    {
        if (glowImage == null) yield break;
        float start = glowImage.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(start, target, elapsed / duration);
            glowImage.color = new Color(1f, 0.85f, 0.2f, a);
            yield return null;
        }
        glowImage.color = new Color(1f, 0.85f, 0.2f, target);
    }

    IEnumerator FadeLabel(float target, float duration)
    {
        if (nameLabel == null) yield break;
        float start = nameLabel.color.a;
        float elapsed = 0f;
        Color c = nameLabel.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            nameLabel.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, target, elapsed / duration));
            yield return null;
        }
        nameLabel.color = new Color(c.r, c.g, c.b, target);
    }

    void StopFade()
    {
        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); fadeCoroutine = null; }
    }
}
