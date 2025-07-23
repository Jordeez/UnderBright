using UnityEngine;
using UnityEngine.UI;

public class MapFogRevealer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mapCamera;
    public Image fogOverlay;

    [Header("Mask Settings")]
    public int maskSize = 256;
    public float revealRadius = 8f;

    private Texture2D revealMask;
    private Material fogMaterial;

    void Start()
    {
        // Create a blank mask
        revealMask = new Texture2D(maskSize, maskSize, TextureFormat.R8, false);
        revealMask.filterMode = FilterMode.Bilinear;
        ClearMask();

        // Duplicate and assign the fog material (so we don't modify the asset)
        fogMaterial = Instantiate(fogOverlay.material);
        fogOverlay.material = fogMaterial;

        // Assign the generated mask texture to the shader
        fogMaterial.SetTexture("_MaskTex", revealMask);
    }

    void Update()
    {
        UpdateRevealMask();
    }

    void ClearMask()
    {
        Color[] fill = new Color[maskSize * maskSize];
        for (int i = 0; i < fill.Length; i++) fill[i] = Color.black;
        revealMask.SetPixels(fill);
        revealMask.Apply();
    }

    void UpdateRevealMask()
    {
        // Get player's position in map camera's viewport (0–1)
        Vector3 viewportPos = mapCamera.WorldToViewportPoint(player.position);
        int texX = Mathf.FloorToInt(viewportPos.x * maskSize);
        int texY = Mathf.FloorToInt(viewportPos.y * maskSize);

        DrawRevealCircle(texX, texY, revealRadius);
        revealMask.Apply();
    }

    void DrawRevealCircle(int centerX, int centerY, float radius)
    {
        int r = Mathf.CeilToInt(radius);
        int rSquared = Mathf.RoundToInt(radius * radius);

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                int px = centerX + x;
                int py = centerY + y;

                if (px >= 0 && px < maskSize && py >= 0 && py < maskSize)
                {
                    if (x * x + y * y <= rSquared)
                    {
                        revealMask.SetPixel(px, py, Color.white);
                    }
                }
            }
        }
    }
}
