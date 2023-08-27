using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private bool _rescaleCanvas = false;

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
    [SerializeField] private TMP_Text _resistanceValuesLabel;

    [SerializeField] private Transform _resistorDisplayPoint;
    [SerializeField] private GameObject _resistorPrefab;
    private Resistor _resistor;

    private void Awake()
    {
        if (_rescaleCanvas)
        {
            SetCanvasScale(Screen.currentResolution);
        }

        SetDropdownStates(_bandCountDropdown.value);
        InitializeResistor();
    }

    void InitializeResistor()
    {
        _resistor = Instantiate(_resistorPrefab, _resistorDisplayPoint.position, Quaternion.identity).GetComponent<Resistor>();
        UpdateResistorValues();
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
        else
        {
            // Handle dropdown states in the cases of five and six band resistors.
            _colorDropdown3.interactable = true;

            if (value.Equals(2)) { _ppmDropdown.interactable = true; }
            else
            {
                _ppmDropdown.value = 0;
                _ppmDropdown.RefreshShownValue();
                _ppmDropdown.interactable = false;
            }
        }
    }

    void UpdateResistorValues()
    {
        // Get the values currently set within the dropdowns.
        int bandCount = _bandCountDropdown.value + 4;
        string colorOne = _colorDropdown1.options[_colorDropdown1.value].text.ToLower();
        string colorTwo = _colorDropdown2.options[_colorDropdown2.value].text.ToLower();
        string colorThree = _colorDropdown3.options[_colorDropdown3.value].text.ToLower();
        string multiplier = _multiplierDropdown.options[_multiplierDropdown.value].text.ToLower();
        string tolerance = _toleranceDropdown.options[_toleranceDropdown.value].text.ToLower();
        string ppm = _ppmDropdown.options[_ppmDropdown.value].text.ToLower();

        string[] bandColors = new string[6] { colorOne, colorTwo, colorThree, multiplier, tolerance, ppm };

        // Pass these values along to the resistor controller.
        _resistor.UpdateResistor(bandCount, bandColors);

        // Update the band and result texts with these values.
        UpdateBandText();
        UpdateResultText();
    }

    void UpdateBandText()
    {
        /* Bands 1 & 2 (required color bands), 4 (multiplier), and 5 (tolerance) always have numerical values
         * displaying while bands 3 (optional color band) and 6 (ppm band) do not. When bands 3 and 6 do not have a
         * numerical value displaying - indicating they are not in use - they will replace the numerical value with
         * a hyphen succeeded by the unit whose value they would display. */
        
        string bandOneText = _resistor.ValueFromBandNumber(1);
        string bandTwoText = _resistor.ValueFromBandNumber(2);
        string bandFourText = $"{_resistor.ValueFromBandNumber(4)} Ω";
        string bandFiveText = $"±{_resistor.ValueFromBandNumber(5)}%";

        // Handle the third value that is there sometimes, not others.
        string bandThreeText = _resistor.ValueFromBandNumber(3);
        bandThreeText = !string.IsNullOrWhiteSpace(bandThreeText) ? $"{bandThreeText} Ω" : "- Ω";

        string bandSixText = _resistor.ValueFromBandNumber(6);
        bandSixText = !string.IsNullOrWhiteSpace(bandSixText) ? $"{bandSixText} ppm" : "- ppm";

        // Set all of the texts.
        _bandOneLabel.SetText(bandOneText);
        _bandTwoLabel.SetText(bandTwoText);
        _bandThreeLabel.SetText(bandThreeText);
        _bandFourLabel.SetText(bandFourText);
        _bandFiveLabel.SetText(bandFiveText);
        _bandSixLabel.SetText(bandSixText);
    }

    void UpdateResultText()
    {
        string resistanceText = $"{_resistor.formattedResistance} Ω";
        string toleranceText = $"±{_resistor.ValueFromBandNumber(5)}%";
        string ppmText = !string.IsNullOrEmpty(_resistor.ValueFromBandNumber(6)) ? $"{_resistor.ValueFromBandNumber(6)} ppm" : "";
        string resistorValues = $"{resistanceText} {toleranceText} {ppmText}";

        // Set the resistance values text.
        _resistanceValuesLabel.SetText(resistorValues);
    }

    public void ResetResistorValues()
    {
        // Reset all dropdowns to initial values.
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

        Debug.Log("Reset resistor values");
    }

    private void SetCanvasScale(Resolution resolution)
    {
        if (resolution.height == 1280 && resolution.width == 720)
        {
            // Scale UI Canvas by [1.15] for HD (1280 x 720)
            canvasScaler.scaleFactor = 1.15f;
        }
        else if (resolution.width == 1920 && resolution.height == 1080)
        {
            // Scale UI Canvas by [1.75] for Full HD (1920 x 1080)
            canvasScaler.scaleFactor = 1.75f;
        }
        else if (resolution.width == 2560 && resolution.height == 1440)
        {
            // Scale UI Canvas by [2.25] for QHD (2560 x 1440)
            canvasScaler.scaleFactor = 2.25f;
        }
        else if (resolution.width == 3840 && resolution.height == 2160)
        {
            // Scale UI Canvas by [3.5] for 4K UHD (3840 x 2160)
            canvasScaler.scaleFactor = 3.5f;
        }
        else
        {
            // Default to canvas of scale 1.
            canvasScaler.scaleFactor = 1f;
        }

        Debug.Log($"Canvas scale factor set to {canvasScaler.scaleFactor} for resolution {resolution.width} x {resolution.height}");
    }
}
