using UnityEngine;

public class PaletteSwapController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Array of Palette Textures
    public Texture2D[] palettes;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Optional: Initialize with the first palette
        if (palettes.Length > 0)
        {
            SetPalette(0);  // Set the first palette initially
        }
    }

    // Set the active palette by index
    public void SetPalette(int paletteIndex)
    {
        if (palettes.Length == 0)
        {
            Debug.LogError("No palettes assigned.");
            return;
        }

        // Ensure the palette index is within range
        if (paletteIndex < 0 || paletteIndex >= palettes.Length)
        {
            Debug.LogError("Invalid palette index.");
            return;
        }

        // Create a new MaterialPropertyBlock to modify the material's properties
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);

        // Set the texture for _SwapTex to the selected palette
        mpb.SetTexture("_SwapTex", palettes[paletteIndex]);
        
        // Set the total number of palettes
        mpb.SetFloat("_TotalPalettes", palettes.Length);

        // Apply the material property block to the sprite renderer
        spriteRenderer.SetPropertyBlock(mpb);
    }

    // Example: Change palette based on user input
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))  // Press 1 to switch to the first palette
        {
            SetPalette(0);  // Switch to the first palette
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  // Press 2 to switch to the second palette
        {
            SetPalette(1);  // Switch to the second palette
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  // Press 3 to switch to the third palette
        {
            SetPalette(2);  // Switch to the third palette
        }
    }
}
