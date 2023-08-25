using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResistorCalculator
{
    public static float CalculateResistance(int[] colorVals, float multiplier)
    {
        // Convert int array to single string comprised of array elements.
        string baseResText = string.Join("", colorVals);

        // Parse the string into a float and multiply it by the multiplier to get total resistance.
        float baseResVal = float.Parse(baseResText);
        return baseResVal * multiplier;
    }
}
