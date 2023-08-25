using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResistorImageController : MonoBehaviour
{
    private int _bandCount;

    Dictionary<string, SpriteRenderer> _bandSprites;

    private void Awake()
    {
        // Bands are rectangular sprites with this gameobjects transform set as their parent.
        _bandCount = transform.childCount;
        Debug.Log($"Found {_bandCount} children");

        SetBandSprites(_bandCount);
    }

    private void SetBandSprites(int bandCount)
    {
        _bandSprites = new Dictionary<string, SpriteRenderer>
        {
            { "ColorBandOne", transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>() },
            { "ColorBandTwo", transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>() }
        };

        if (_bandCount.Equals(4))
        {
            _bandSprites.Add("MultiplierBand", transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("ToleranceBand", transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>());
        }
        else if (bandCount.Equals(5))
        {
            _bandSprites.Add("ColorBandThree", transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("MultiplierBand", transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("ToleranceBand", transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>());
        }
        else if (bandCount.Equals(6))
        {
            _bandSprites.Add("ColorBandThree", transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("MultiplierBand", transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("ToleranceBand", transform.GetChild(4).gameObject.GetComponent<SpriteRenderer>());
            _bandSprites.Add("PpmBand", transform.GetChild(5).gameObject.GetComponent<SpriteRenderer>());
        }
    }

    private Color ColorFromColorValue(float colorValue)
    {
        float r, g, b, a;
        switch (colorValue)
        {
            case 0:  r = 0f; g = 0f; b = 0f; a = 255f; break; // Black
            case 1:  r = 125; g = 75f; b = 0f; a = 255f; break; // Brown        
            case 2:  r = 255f; g = 0f; b = 0f; a = 255f; break; // Red
            case 3:  r = 255f; g = 128f; b = 0f; a = 255f; break; // Orange
            case 4:  r = 255f; g = 255f; b = 0f; a = 255f; break; // Yellow
            case 5:  r = 0f; g = 155f; b = 0f; a = 255f; break; // Green
            case 6:  r = 0f; g = 0f; b = 255f; a = 255f; break; // Blue
            case 7:  r = 128f; g = 0f; b = 128f; a = 255f; break; // Violet
            case 8:  r = 192; g = 192f; b = 192f; a = 255f; break; // Grey
            case 9:  r = 255f; g = 255f; b = 255f; a = 255f; break; // White
            default: r = 0f; g = 0f; b = 0f; a = 0f; break; // Transparent due to invalid color.
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    private Color ColorFromMultiplierValue(float multiplierValue)
    {
        float r, g, b, a;
        switch (multiplierValue)
        {
            case 0: r = 0f; g = 0f; b = 0f; a = 255f; break;
            case 1: r = 125; g = 75f; b = 0f; a = 255f; break; // Brown       
            case 2: r = 255f; g = 0f; b = 0f; a = 255f; break; // Red
            case 3: r = 255f; g = 128f; b = 0f; a = 255f; break; // Orange
            case 4: r = 255f; g = 255f; b = 0f; a = 255f; break; // Yellow
            case 5: r = 0f; g = 155f; b = 0f; a = 255f; break; // Green
            case 6: r = 0f; g = 0f; b = 255f; a = 255f; break; // Blue
            case 7: r = 128f; g = 0f; b = 128f; a = 255f; break; // Violet
            case 8: r = 192f; g = 192f; b = 192f; a = 255f; break; // Grey
            case 9: r = 255f; g = 255f; b = 255f; a = 255f; break; // White
            case 10: r = 255f; g = 206f; b = 43f; a = 255f; break; // Gold
            case 11: r = 128f; g = 128f; b = 128f; a = 255f; break; // Silver
            default: r = 0f; g = 0f; b = 0f; a = 0f; break; // Transparent due to invalid color.
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    private Color ColorFromToleranceValue(float toleranceValue)
    {
        float r, g, b, a;
        switch (toleranceValue)
        {
            case 0: r = 125; g = 75f; b = 0f; a = 255f; break; // Brown
            case 1: r = 255f; g = 0f; b = 0f; a = 255f; break; // Red
            case 2: r = 0f; g = 155f; b = 0f; a = 255f; break; // Green
            case 3: r = 0f; g = 0f; b = 255f; a = 255f; break; // Blue
            case 4: r = 128f; g = 0f; b = 128f; a = 255f; break; // Violet
            case 5: r = 192f; g = 192f; b = 192f; a = 255f; break; // Grey
            case 6: r = 255f; g = 206f; b = 43f; a = 255f; break; // Gold
            case 7: r = 128f; g = 128f; b = 128f; a = 255f; break; // Silver
            default: r = 0f; g = 0f; b = 0f; a = 0f; break; // Transparent due to invalid color.
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    private Color ColorFromPpmValue(float ppmValue)
    {
        float r, g, b, a;
        switch (ppmValue)
        {
            case 0: r = 128f; g = 0f; b = 128f; a = 255f; break; // Violet
            case 1: r = 0f; g = 0f; b = 255f; a = 255f; break; // Blue
            case 2: r = 255f; g = 128f; b = 0f; a = 255f; break; // Orange
            case 3: r = 255f; g = 255f; b = 0f; a = 255f; break; // Yellow
            case 4: r = 255f; g = 0f; b = 0f; a = 255f; break; // Red
            case 5: r = 125; g = 75f; b = 0f; a = 255f; break; // Brown
            default: r = 0f; g = 0f; b = 0f; a = 0f; break; // Transparent due to invalid color.
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    public void SetBandColors(Dictionary<string, float> keyValuePairs)
    {
        // The first four colors will always get set.
        _bandSprites["ColorBandOne"].color = ColorFromColorValue(keyValuePairs["ColorBandOne"]);
        _bandSprites["ColorBandTwo"].color = ColorFromColorValue(keyValuePairs["ColorBandTwo"]);
        _bandSprites["MultiplierBand"].color = ColorFromMultiplierValue(keyValuePairs["MultiplierBand"]);
        _bandSprites["ToleranceBand"].color = ColorFromToleranceValue(keyValuePairs["ToleranceBand"]);

        if (_bandCount.Equals(5))
        {
            // There will be only five colors in the array.
            _bandSprites["ColorBandThree"].color = ColorFromColorValue(keyValuePairs["ColorBandThree"]);
        }
        else if (_bandCount.Equals(6))
        {
            // There will be only six colors in the array.
            _bandSprites["ColorBandThree"].color = ColorFromColorValue(keyValuePairs["ColorBandThree"]);
            _bandSprites["PpmBand"].color = ColorFromPpmValue(keyValuePairs["PpmBand"]);
        }
    }
}
