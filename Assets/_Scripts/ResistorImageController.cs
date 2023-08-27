using System.Collections.Generic;
using UnityEngine;

public class ResistorImageController : MonoBehaviour
{
    Dictionary<int, SpriteRenderer> _bandSprites;

    private void Awake()
    {
        // Bands are rectangular sprites with this gameobjects transform set as their parent.
        _bandSprites = new Dictionary<int, SpriteRenderer>()
        {
            { 1, transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>() },
            { 2, transform.GetChild(3).GetChild(1).GetComponent<SpriteRenderer>() },
            { 3, transform.GetChild(3).GetChild(2).GetComponent<SpriteRenderer>() },
            { 4, transform.GetChild(3).GetChild(3).GetComponent<SpriteRenderer>() },
            { 5, transform.GetChild(3).GetChild(4).GetComponent<SpriteRenderer>() },
            { 6, transform.GetChild(3).GetChild(5).GetComponent<SpriteRenderer>() },
        };
    }

    public void UpdateBandColors(Dictionary<int, Color> bandColors)
    {
        for (int i = 1; i <= bandColors.Count; i++) { _bandSprites[i].color = bandColors[i]; }
    }
}
