using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _bandCountDropdown;
    [SerializeField] private TMP_Dropdown _colorDropdown1;
    [SerializeField] private TMP_Dropdown _colorDropdown2;
    [SerializeField] private TMP_Dropdown _colorDropdown3;
    [SerializeField] private TMP_Dropdown _multiplierDropdown;
    [SerializeField] private TMP_Dropdown _toleranceDropdown;
    [SerializeField] private TMP_Dropdown _ppmDropdown;

    [SerializeField] private TMP_Text _bandOneLabel;
    [SerializeField] private TMP_Text _bandTwoLabel;
    [SerializeField] private TMP_Text _bandThreeLabel;
    [SerializeField] private TMP_Text _bandFourLabel;
    [SerializeField] private TMP_Text _bandFiveLabel;
    [SerializeField] private TMP_Text _bandSixLabel;

    [SerializeField] private Transform _resistorDisplayPoint;
    private GameObject _currentResistorDisplay;
    [SerializeField] private GameObject _fourBandResistor;
    [SerializeField] private GameObject _fiveBandResistor;
    [SerializeField] private GameObject _sixBandResistor;

    [SerializeField] private TMP_Text _resistanceValuesLabel;

    private void Awake()
    {
        // Initialize the dropdown values states and band texts.
        InitializeDropdownValues();
        SetDropdownStates(_bandCountDropdown.value);
        UpdateBandText();
        UpdateResistorImage();
        CalculateResistorValues();
    }

    private void InitializeDropdownValues()
    {
        // Set the initial value of each dropdown to the first element (index 0).
        _bandCountDropdown.value = 0;
        _bandCountDropdown.RefreshShownValue();

        _colorDropdown1.value = 0;
        _colorDropdown1.RefreshShownValue();

        _colorDropdown2.value = 0;
        _colorDropdown2.RefreshShownValue();

        _colorDropdown3.value = 0;
        _colorDropdown3.RefreshShownValue();

        _multiplierDropdown.value = 0;
        _multiplierDropdown.RefreshShownValue();

        _toleranceDropdown.value = 0;
        _toleranceDropdown.RefreshShownValue();

        _ppmDropdown.value = 0;
        _ppmDropdown.RefreshShownValue();
    }

    public void SetDropdownStates(int value)
    {
        // 0 = Four Band Resistor, 1 = Five Band Resistor, 2 = Six Band Resistor
        if (value.Equals(0))
        {
            // Disable third color band and PPM dropdowns. All others are enabled.
            _colorDropdown3.value = 0;
            _colorDropdown3.RefreshShownValue();
            _colorDropdown3.interactable = false;

            _ppmDropdown.value = 0;
            _ppmDropdown.RefreshShownValue();
            _ppmDropdown.interactable = false;
        }
        else if (value.Equals(1))
        {
            // Disable PPM dropdown. All others are enabled.
            _colorDropdown3.interactable = true;
            _colorDropdown3.RefreshShownValue();

            _ppmDropdown.value = 0;
            _ppmDropdown.RefreshShownValue();
            _ppmDropdown.interactable = false;
        }
        else if (value.Equals(2))
        {
            // Size band resistor selected. Enable all dropdowns.
            _colorDropdown3.interactable = true;
            _colorDropdown3.RefreshShownValue();

            _ppmDropdown.interactable = true;
            _ppmDropdown.RefreshShownValue();
        }
    }

    public void UpdateBandText()
    {
        /* Bands 1 & 2 (required color bands), 4 (multiplier), and 5 (tolerance) always have numerical values
         * displaying while bands 3 (optional color band) and 6 (ppm band) do not. When bands 3 and 6 do not have a
         * numerical value displaying - indicating they are not in use - they will replace the numerical value with
         * a hyphen succeeded by the unit whose value they would display. */
        
        // Format the text of the color bands using the color values.
        int[] colorVals = GetColorVals();
        string bandOneText = colorVals[0].ToString();
        string bandTwoText = colorVals[1].ToString();

        // Handle the third value that is there sometimes, not others.
        string bandThreeText = colorVals.Length == 3 ? colorVals[2].ToString() : "-";

        // Format the multiplier and tolerance texts.
        string bandFourText = $"{CalculateMultiplier()} ohm(s)";
        string bandFiveText = $"±{ToleranceAsPercentage()}%";

        // Handle the PPM value that may or may not be used.
        float? ppmVal = CalculatePpm();
        string bandSixText = ppmVal.HasValue ? $"{ppmVal.Value} ppm" : "- ppm";

        // Set all of the texts.
        _bandOneLabel.SetText(bandOneText);
        _bandTwoLabel.SetText(bandTwoText);
        _bandThreeLabel.SetText(bandThreeText);
        _bandFourLabel.SetText(bandFourText);
        _bandFiveLabel.SetText(bandFiveText);
        _bandSixLabel.SetText(bandSixText);
    }

    private int[] GetColorVals()
    {
        // Grab the first two color indices within the dropdowns.
        int colorVal1 = _colorDropdown1.value;
        int colorVal2 = _colorDropdown2.value;

        // Check if the third color band is being used. -1 indicates it is not.
        int? colorVal3 = _colorDropdown3.interactable ? _colorDropdown3.value : null;

        // Create the array, including colorVal3 only if it is not null.
        return colorVal3.HasValue ?
            new int[3] { colorVal1, colorVal2, colorVal3.Value } : new int[2] { colorVal1, colorVal2 };
    }

    private float CalculateMultiplier()
    {
        switch (_multiplierDropdown.value)
        {
            case 0:  return 1f;          // 1 ohm
            case 1:  return 10f;         // 10 ohms
            case 2:  return 100f;        // 100 ohms
            case 3:  return 1000f;       // 1,000 ohms
            case 4:  return 10000f;      // 10,000 ohms
            case 5:  return 100000f;     // 100,000 ohms
            case 6:  return 1000000f;    // 1,000,000 ohms
            case 7:  return 10000000f;   // 10,000,000 ohms
            case 8:  return 100000000f;  // 100,000,000 ohms
            case 9:  return 1000000000f; // 1,000,000,000 ohms
            case 10: return 0.1f;        // 0.1 ohms
            case 11: return 0.01f;       // 0.01 ohms
            default: return -1f;         // Invalid value.
        }
    }

    float CalculateTolerance()
    {
        switch (_toleranceDropdown.value)
        {
            case 0: return 0.01f;   // +/- 1%
            case 1: return 0.02f;   // +/- 2%
            case 2: return 0.005f;  // +/- 0.5%
            case 3: return 0.0025f; // +/- 0.25%
            case 4: return 0.001f;  // +/- 0.1%
            case 5: return 0.0005f; // +/- 0.05%
            case 6: return 0.05f;   // +/- 5%
            case 7: return 0.1f;    // +/- 10%
            default: return -1f;    // Invalid value.
        }
    }

    string ToleranceAsPercentage()
    {
        switch (_toleranceDropdown.value)
        {
            case 0: return "1";
            case 1: return "2";
            case 2: return "0.5";
            case 3: return "0.25";
            case 4: return "0.1";
            case 5: return "0.05";
            case 6: return "5";
            case 7: return "10";
            default: return "invalid";
        }
    }

    float? CalculatePpm()
    {
        // Determine if the PPM dropdown value should be used based on its active in hierarchy state.
        if (!_ppmDropdown.interactable)
        {
            // Dropdown is not active, ignore PPM value.
            return null;
        }

        switch (_ppmDropdown.value)
        {
            case 0: return 5f;   // 5 ppm
            case 1: return 10f;  // 10 ppm
            case 2: return 15f;  // 15 ppm
            case 3: return 25f;  // 25 ppm
            case 4: return 50f;  // 50 ppm
            case 5: return 100f; // 100 ppm
            default: return -1f; // Invalid value.
        }
    }

    public void CalculateResistorValues()
    {
        // Calculate the total resistance value using the parameters obtained above.
        float resistanceVal = ResistorCalculator.CalculateResistance(GetColorVals(), CalculateMultiplier());
        //float toleranceVal = CalculateTolerance();
        float? ppmVal = CalculatePpm();

        Debug.Log($"Resistor Values\nResistance = {resistanceVal}\tTolerance = {ToleranceAsPercentage()}\tPPM = {ppmVal}");

        // Format the values into their text values.
        string formattedResistanceValue;
        if (1000f <= resistanceVal && resistanceVal < 1000000f)
        {
            // Format string to contain a k for kilo ohms
            formattedResistanceValue = $"{resistanceVal / 1000f}k";
        }
        else if (1000000f <= resistanceVal && resistanceVal < 1000000000f)
        {
            // Format to contain an M for mega ohms.
            formattedResistanceValue = $"{resistanceVal / 1000000f}M";
        }
        else if (1000000000f <= resistanceVal)
        {
            // Format to contain a G for giga ohms.
            formattedResistanceValue = $"{resistanceVal / 1000000000f}G";
        }
        else
        {
            // No formatting required.
            formattedResistanceValue = resistanceVal.ToString();
        }

        string resistanceText = resistanceVal != 1f ? $"{formattedResistanceValue} ohms" : $"{resistanceVal} ohm";
        string toleranceText = $"±{ToleranceAsPercentage()}%";
        string ppmText = ppmVal.HasValue ? $"{ppmVal.Value} ppm" : "";
        string resistorValues = $"{resistanceText} {toleranceText} {ppmText}";

        // Set the resistance values text.
        _resistanceValuesLabel.SetText(resistorValues);
    }

    public void UpdateResistorImage()
    {
        UpdateResistorImageBandCount();
        UpdateResistorImageColors();
    }

    private void UpdateResistorImageBandCount()
    {
        Destroy(_currentResistorDisplay);

        // Get the number of bands selected and set the respective image.
        int bandCount = _bandCountDropdown.value + 4;
        if (bandCount.Equals(4))
        {
            _currentResistorDisplay = Instantiate(_fourBandResistor, _resistorDisplayPoint.position, Quaternion.identity);
        }
        else if (bandCount.Equals(5))
        {
            _currentResistorDisplay = Instantiate(_fiveBandResistor, _resistorDisplayPoint.position, Quaternion.identity);

        }
        else if (bandCount.Equals(6))
        {
            _currentResistorDisplay = Instantiate(_sixBandResistor, _resistorDisplayPoint.position, Quaternion.identity);
        }
    }

    public void UpdateResistorImageColors()
    {
        Dictionary<string, float> keyValuePairs = new Dictionary<string, float>();

        // Get the values of each band.
        int[] colorVals = GetColorVals();
        float? ppm = CalculatePpm();

        // The first two color bands, multiplier band, and tolerance band are always added.
        keyValuePairs.Add("ColorBandOne", colorVals[0]);
        keyValuePairs.Add("ColorBandTwo", colorVals[1]);
        keyValuePairs.Add("MultiplierBand", _multiplierDropdown.value);
        keyValuePairs.Add("ToleranceBand", _toleranceDropdown.value);

        // Add the third color band and ppm band only if necessary.
        if (colorVals.Length.Equals(3)) { keyValuePairs.Add("ColorBandThree", colorVals[2]); }
        if (ppm.HasValue) { keyValuePairs.Add("PpmBand", _ppmDropdown.value); }

        _currentResistorDisplay.GetComponent<ResistorImageController>().SetBandColors(keyValuePairs);
    }
}
