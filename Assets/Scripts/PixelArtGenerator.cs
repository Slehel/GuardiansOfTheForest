using UnityEngine;

// Generates simple 16x16 pixel art sprites in code — no external assets needed.
// Call PixelArtGenerator.Apply(unit) in Unit.Start() to assign the right sprite.
public static class PixelArtGenerator
{
    // 16x16 pixel maps: 0 = transparent, 1 = body color, 2 = accent color, 3 = dark outline
    // Row 0 = bottom, Row 15 = top

    private static readonly int[,] BearPixels = new int[16, 16]
    {
        { 0,0,0,0,0,3,3,3,3,3,3,0,0,0,0,0 }, // 0
        { 0,0,0,0,3,1,1,1,1,1,1,3,0,0,0,0 }, // 1
        { 0,0,0,3,1,1,1,1,1,1,1,1,3,0,0,0 }, // 2
        { 0,0,3,1,1,2,1,1,1,2,1,1,1,3,0,0 }, // 3 eyes
        { 0,0,3,1,1,1,1,1,1,1,1,1,1,3,0,0 }, // 4
        { 0,0,3,1,1,1,2,2,2,1,1,1,1,3,0,0 }, // 5 nose
        { 0,0,0,3,1,1,1,1,1,1,1,1,3,0,0,0 }, // 6
        { 0,0,0,3,1,1,1,1,1,1,1,1,3,0,0,0 }, // 7
        { 0,0,3,1,1,1,1,1,1,1,1,1,1,3,0,0 }, // 8 body
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 9
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 10
        { 0,3,1,1,2,2,1,1,1,1,2,2,1,1,3,0 }, // 11 paws
        { 0,3,1,1,2,2,1,1,1,1,2,2,1,1,3,0 }, // 12
        { 0,0,3,3,3,3,1,1,1,1,3,3,3,3,0,0 }, // 13 feet
        { 0,0,0,0,0,3,1,1,1,1,3,0,0,0,0,0 }, // 14
        { 0,0,0,0,3,3,0,0,0,0,3,3,0,0,0,0 }, // 15 ears top
    };

    private static readonly int[,] BunnyPixels = new int[16, 16]
    {
        { 0,0,0,0,0,3,3,3,3,3,0,0,0,0,0,0 }, // 0
        { 0,0,0,0,3,1,1,1,1,1,3,0,0,0,0,0 }, // 1 head
        { 0,0,0,3,1,1,2,1,2,1,1,3,0,0,0,0 }, // 2 eyes
        { 0,0,0,3,1,1,1,2,1,1,1,3,0,0,0,0 }, // 3 nose
        { 0,0,0,0,3,1,1,1,1,1,3,0,0,0,0,0 }, // 4
        { 0,0,0,3,1,1,1,1,1,1,1,3,0,0,0,0 }, // 5 body
        { 0,0,3,1,1,1,1,1,1,1,1,1,3,0,0,0 }, // 6
        { 0,0,3,1,1,2,1,1,1,2,1,1,3,0,0,0 }, // 7 buttons
        { 0,0,3,1,1,1,1,1,1,1,1,1,3,0,0,0 }, // 8
        { 0,0,0,3,1,1,1,1,1,1,1,3,0,0,0,0 }, // 9
        { 0,0,3,1,1,1,1,1,1,1,1,1,3,0,0,0 }, // 10
        { 0,3,1,1,2,2,1,0,0,1,2,2,1,1,3,0 }, // 11 legs
        { 0,3,1,1,2,2,3,0,0,3,2,2,1,1,3,0 }, // 12
        { 0,0,3,3,3,3,0,0,0,0,3,3,3,3,0,0 }, // 13 feet
        { 0,0,3,1,3,0,0,0,0,0,0,3,1,3,0,0 }, // 14 ears
        { 0,3,1,3,0,0,0,0,0,0,0,0,3,1,3,0 }, // 15 long ears
    };

    private static readonly int[,] CorruptedWolfPixels = new int[16, 16]
    {
        { 0,0,3,3,0,0,0,0,0,0,0,0,3,3,0,0 }, // 0 pointy ears
        { 0,3,1,1,3,0,0,0,0,0,0,3,1,1,3,0 }, // 1
        { 3,1,1,1,1,3,3,3,3,3,3,1,1,1,1,3 }, // 2 head wide
        { 3,1,1,2,1,1,1,1,1,1,1,1,2,1,1,3 }, // 3 red eyes
        { 3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3 }, // 4
        { 3,1,1,1,2,2,2,2,2,2,2,2,1,1,1,3 }, // 5 teeth
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 6
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 7
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 8 body
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 9
        { 0,3,1,2,1,1,1,1,1,1,1,1,2,1,3,0 }, // 10 claws
        { 0,3,1,2,2,1,1,1,1,1,1,2,2,1,3,0 }, // 11
        { 0,0,3,1,1,3,1,1,1,1,3,1,1,3,0,0 }, // 12 legs
        { 0,0,3,1,1,3,0,0,0,0,3,1,1,3,0,0 }, // 13
        { 0,0,0,3,3,0,0,0,0,0,0,3,3,0,0,0 }, // 14 paws
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 15
    };

    private static readonly int[,] CorruptedEngineerPixels = new int[16, 16]
    {
        { 0,0,0,0,3,3,3,3,3,3,3,3,0,0,0,0 }, // 0 helmet
        { 0,0,0,3,2,2,2,2,2,2,2,2,3,0,0,0 }, // 1 yellow trim
        { 0,0,3,1,1,1,1,1,1,1,1,1,1,3,0,0 }, // 2 face
        { 0,0,3,1,2,1,1,1,1,1,2,1,1,3,0,0 }, // 3 eyes
        { 0,0,3,1,1,1,1,1,1,1,1,1,1,3,0,0 }, // 4
        { 0,0,0,3,1,1,2,2,2,2,1,1,3,0,0,0 }, // 5 mouth
        { 0,0,0,3,3,3,3,3,3,3,3,3,3,0,0,0 }, // 6
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 7 torso
        { 0,3,1,2,1,1,1,1,1,1,1,1,2,1,3,0 }, // 8 shoulder pads
        { 0,3,1,1,1,1,1,1,1,1,1,1,1,1,3,0 }, // 9
        { 0,3,1,1,1,2,2,2,2,2,2,1,1,1,3,0 }, // 10 belt
        { 0,0,3,1,3,0,1,1,1,1,0,3,1,3,0,0 }, // 11 arms out
        { 0,0,3,1,3,0,3,1,1,3,0,3,1,3,0,0 }, // 12 legs
        { 0,0,0,3,0,0,3,1,1,3,0,0,3,0,0,0 }, // 13
        { 0,0,0,0,0,0,3,2,2,3,0,0,0,0,0,0 }, // 14 boots
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }, // 15
    };

    // Applies the correct procedural sprite to a unit based on their name
    public static void Apply(Unit unit)
    {
        string n = unit.unitName.ToLower();
        Color body, accent;
        int[,] pixels;

        if (n.Contains("bear") || n.Contains("bearimus"))
        {
            body = new Color(0.55f, 0.35f, 0.15f);   // brown
            accent = new Color(0.3f, 0.18f, 0.05f);  // dark brown
            pixels = BearPixels;
        }
        else if (n.Contains("bunny") || n.Contains("lotus"))
        {
            body = new Color(0.92f, 0.88f, 0.85f);   // off-white
            accent = new Color(0.75f, 0.55f, 0.55f); // pink
            pixels = BunnyPixels;
        }
        else if (n.Contains("wolf") || n.Contains("policeman") || n.Contains("police"))
        {
            body = new Color(0.3f, 0.2f, 0.35f);     // dark purple-grey
            accent = new Color(0.8f, 0.1f, 0.1f);    // red
            pixels = CorruptedWolfPixels;
        }
        else
        {
            body = new Color(0.2f, 0.2f, 0.25f);     // dark uniform
            accent = new Color(0.85f, 0.75f, 0.1f);  // yellow trim
            pixels = CorruptedEngineerPixels;
        }

        Sprite sprite = BuildSprite(pixels, body, accent);
        if (unit.unitSpriteRenderer != null)
            unit.unitSpriteRenderer.sprite = sprite;
    }

    static Sprite BuildSprite(int[,] pixels, Color body, Color accent)
    {
        Color outline = new Color(0.08f, 0.08f, 0.08f, 1f);
        Color transparent = new Color(0, 0, 0, 0);

        int size = 16;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point; // crisp pixel art — no blur

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int v = pixels[size - 1 - y, x]; // flip Y so row 0 = bottom
                Color c = v == 0 ? transparent
                        : v == 1 ? body
                        : v == 2 ? accent
                        :          outline;
                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
