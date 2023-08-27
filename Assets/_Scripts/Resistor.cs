using System.Collections.Generic;
using UnityEngine;

/* Big shoutout to those at DigiKey who inspired me to make my own version of an interactive resistor calculator,
 * their version can be found atthe link below. 
 * 
 * https://www.digikey.com/en/resources/conversion-calculators/conversion-calculator-resistor-color-code
 */

public class Resistor : MonoBehaviour
{
    private int _bandCount;
    private Dictionary<int, Color> _bandColors;
    private Dictionary<int, float?> _bandValues;

    public float resistance { get; private set; }
    public string formattedResistance 
    {
        get 
        {
            if (resistance == 1f) { return $"{resistance}"; }
            else if (resistance < 1000f) { return $"{resistance}"; }
            else if (1000f <= resistance && resistance < 1000000f) { return $"{resistance / 1000f}k"; }
            else if (1000000f <= resistance && resistance < 1000000000f) { return $"{resistance / 1000000f}M"; }
            else { return $"{resistance / 1000000000f}G"; }
        }
    }

    private ResistorImageController _spriteController;

    private void Awake()
    {
        _spriteController = GetComponent<ResistorImageController>();
    }

    public string ValueFromBandNumber(int bandNumber)
    {
        if (0 < bandNumber && bandNumber < 4) { return _bandValues[bandNumber].ToString(); }
        else if (bandNumber == 4)
        {
            float multiplier = _bandValues[4].Value;
            if (multiplier < 1000f) { return $"{multiplier}"; }
            else if (1000f <= multiplier && multiplier < 100000f) { return $"{multiplier / 100000f}k"; }
            else if (100000f <= multiplier && multiplier < 100000000f) { return $"{multiplier / 100000000f}M"; }
            else { return $"{multiplier / 1000000000f}G"; }
        }
        else if (bandNumber == 5)
        {
            float tolerance = _bandValues[5].Value * 100.0f;
            if (tolerance >= 0f) { return tolerance.ToString(); }
            else if (tolerance < 0f && tolerance >= 0.1f) { return tolerance.ToString("F1"); }
            else if (tolerance < 0.1f && tolerance >= 0.01f) { return tolerance.ToString("F2"); }
            else if (tolerance < 0.01 && tolerance >= 0.001f) { return tolerance.ToString("F3"); }
            else { return tolerance.ToString("F4"); }
        }
        else { return _bandValues[6].ToString(); }
    }

    public void UpdateResistor(int bandCount, string[] bandColors)
    {
        // Update the band count, colors and their respective values.
        _bandCount = bandCount;
        UpdateBandColors(bandColors);
        UpdateBandValues(bandColors);

        // Tell the sprite controller to update visual colors, and then recalculate the bands resistance.
        _spriteController.UpdateBandColors(_bandColors);
        resistance = CalculateResistance(_bandValues);
    }

    float CalculateResistance(Dictionary<int, float?> bandValues)
    {
        string colorVal1 = bandValues[1].Value.ToString();
        string colorVal2 = bandValues[2].Value.ToString();
        string colorVal3 = bandValues[3].HasValue ? bandValues[3].ToString() : null;
        float multiplier = bandValues[4].Value;

        // Convert the three color values to a string, leaving out the third if null.
        string baseResistance = $"{colorVal1}{colorVal2}{colorVal3}";
        if (float.TryParse(baseResistance, out float resistance)) { return resistance * multiplier; }
        else
        {
            Debug.Log($"Unable to calculate resistance using values\n" +
                $"Color 1 = {colorVal1}\tColor 2 = {colorVal2}\tColor 3 = {colorVal3}\tMultiplier = {multiplier}");
            return -1f;
        }
    }

    void UpdateBandColors(string[] bandColors)
    {
        Dictionary<int, Color> bandColorDict = new Dictionary<int, Color>()
        { 
            { 1, Color.clear }, { 2, Color.clear }, { 3, Color.clear },
            { 4, Color.clear }, { 5, Color.clear }, { 6, Color.clear }
        };
        
        // First two color, the multiplier, and tolerance bands are always set.
        bandColorDict[1] = ResistanceColorFromString(bandColors[0]);
        bandColorDict[2] = ResistanceColorFromString(bandColors[1]);
        bandColorDict[4] = MultiplierColorFromString(bandColors[3]);
        bandColorDict[5] = ToleranceColorFromString(bandColors[4]);

        // Third color band is used in five AND six band resistors.
        if (_bandCount > 4) { bandColorDict[3] = ResistanceColorFromString(bandColors[2]); }

        // PPM band is used exclusively in six band resistors.
        if (_bandCount == 6) { bandColorDict[6] = PpmColorFromString(bandColors[5]); }

        // Set the new colors dictionary and tell the resistor image controller to update the band displayed band colors.
        _bandColors = bandColorDict;
    }

    void UpdateBandValues(string[] bandColors)
    {
        Dictionary<int, float?> bandValueDict = new Dictionary<int, float?>()
        {
            { 1, null }, { 2, null }, { 3, null }, 
            { 4, null }, { 5, null }, { 6, null },
        };

        // First two color, the multiplier, and tolerance bands are always set.
        bandValueDict[1] = ResistanceColorToValue(bandColors[0]);
        bandValueDict[2] = ResistanceColorToValue(bandColors[1]);
        bandValueDict[4] = MultiplierColorToValue(bandColors[3]);
        bandValueDict[5] = ToleranceColorToValue(bandColors[4]);

        // Third color band is used in five AND six band resistors.
        if (_bandCount > 4) { bandValueDict[3] = ResistanceColorToValue(bandColors[2]); }

        // PPM band is used exclusively in six band resistors.
        if (_bandCount == 6) { bandValueDict[6] = PpmColorToValue(bandColors[5]); }

        // Set the new values dictionary.
        _bandValues = bandValueDict;
    }

    float ResistanceColorToValue(string bandColor)
    {
        switch (bandColor.ToLower())
        {
            case "black":  return 0f;
            case "brown":  return 1f;
            case "red":    return 2f;
            case "orange": return 3f;
            case "yellow": return 4f;
            case "green":  return 5f;
            case "blue":   return 6f;
            case "violet": return 7f;
            case "grey":   return 8f;
            case "white":  return 9f;
            default: Debug.Log($"{bandColor} is not a valid resistance band color");  return -1f;
        }
    }

    Color ResistanceColorFromString(string bandColor)
    {
        float r, g, b;
        r = g = b = 0f;
        float a = 255f;

        switch (bandColor)
        {
            case "black":  break;
            case "brown":  r = 125f; g = 75f; break;
            case "red":    r = 255f; break;
            case "orange": r = 255f; g = 128f; break;
            case "yellow": r = 255f; g = 255f; break;
            case "green":  g = 255f; break;
            case "blue":   b = 255f; break;
            case "violet": r = b = 128f; break;
            case "grey":   r = g = b = 192f; break;
            case "white":  r = g = b = 255f; break;
            default: Debug.Log($"{bandColor} is not a valid resistance band color"); a = 0f; break;
        }

        // Normalize the RGBA values to fit the format of new colors and return the color.
        r /= 255f;
        g /= 255f;
        b /= 255f;
        a /= 255f;

        return new Color(r, g, b, a);
    }

    float MultiplierColorToValue(string bandColor)
    {
        switch (bandColor.ToLower())
        {
            case "black":  return 1f;
            case "brown":  return 10f;
            case "red":    return 100f;
            case "orange": return 1000f;
            case "yellow": return 10000f;
            case "green":  return 100000f;
            case "blue":   return 1000000f;
            case "violet": return 10000000f;
            case "grey":   return 100000000f;
            case "white":  return 1000000000f;
            case "gold":   return 0.1f;
            case "silver": return 0.01f;
            default: Debug.Log($"{bandColor} is not a valid multiplier band color"); return -1f;
        }
    }

    Color MultiplierColorFromString(string bandColor)
    {
        float r, g, b;
        r = g = b = 0f;
        float a = 255f;

        switch (bandColor)
        {
            case "black": break;
            case "brown": r = 125f; g = 75f; break;
            case "red": r = 255f; break;
            case "orange": r = 255f; g = 128f; break;
            case "yellow": r = g = 255f; break;
            case "green": g = 255f; break;
            case "blue": b = 255f; break;
            case "violet": r = b = 128f; break;
            case "grey": r = g = b = 192f; break;
            case "white": r = g = b = 255f; break;
            case "gold": r = 255f; g = 206f; b = 43f; break;
            case "silver": r = g = b = 128f; break;
            default: Debug.Log($"{bandColor} is not a valid multiplier band color"); a = 0f; break;
        }

        // Normalize the RGBA values to fit the format of new colors and return the color.
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    float ToleranceColorToValue(string bandColor)
    {
        switch (bandColor.ToLower())
        {
            case "grey":   return 0.0005f;
            case "blue":   return 0.0025f;
            case "violet": return 0.001f;
            case "green":  return 0.005f;
            case "brown":  return 0.01f;
            case "red":    return 0.02f;
            case "gold":   return 0.05f;
            case "silver": return 0.1f;
            default: Debug.Log($"{bandColor} is not a valid tolerance band color"); return -1f;
        }
    }

    Color ToleranceColorFromString(string bandColor)
    {
        float r, g, b;
        r = g = b = 0f;
        float a = 255f;

        switch (bandColor)
        {
            case "brown": r = 125f; g = 75f; break;
            case "red": r = 255f; break;
            case "green": g = 255f; break;
            case "blue": b = 255f; break;
            case "violet": r = b = 128f; break;
            case "grey": r = g = b = 192f; break;
            case "gold": r = 255f; g = 206f; b = 43f; break;
            case "silver": r = g = b = 128f; break;
            default: Debug.Log($"{bandColor} is not a valid tolerance band color"); a = 0f; break;
        }

        // Normalize the RGBA values to fit the format of new colors and return the color.
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    float PpmColorToValue(string bandColor)
    {
        switch (bandColor.ToLower())
        {
            case "violet": return 5f;
            case "blue":   return 10f;
            case "orange": return 15f;
            case "yellow": return 25f;
            case "red":    return 50f;
            case "brown":  return 100f;
            default: Debug.Log($"{bandColor} is not a valid ppm band color"); return -1f;
        }
    }

    Color PpmColorFromString(string bandColor)
    {
        float r, g, b;
        r = g = b = 0f;
        float a = 255f;

        switch (bandColor)
        {
            case "violet": r = b = 128f; break;
            case "blue": b = 255f; break;
            case "orange": r = 255f; g = 128f; break;
            case "yellow": r = g = 255f; break;
            case "red": r = 255f; break;
            case "brown": r = 125f; g = 75f; break;
            default: Debug.Log($"{bandColor} is not a valid ppm band color"); a = 0f; break;
        }

        // Normalize the RGBA values to fit the format of new colors and return the color.
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
