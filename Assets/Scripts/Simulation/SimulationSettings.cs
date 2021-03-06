using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSettings : MonoBehaviour
{
    public static int maxVillages = 5;

    private MapController mapController;

    [Serializable]
    public class Settings
    {
        // Map and population settings
        public int mapRows = 100;
        public int mapColumns = 100;
        public int agentsPerVillage = 6;
        public int totalResourcesRequired = 30;
        public float maxBirthChance = 25;
        public float maxDnaMutationChance = 1;
        // Resources settings
        public int amountOfVillages = 2;
        public int amountOfGold = 50;
        public int amountOfEnergyPots = 30;
        public int amountOfStone = 50;
        public int amountOfWood = 50;
        // Misc settings
        public int energyPotCost = 5;
        public int mapCost = 2;
    }

    public static Settings simSettings;
    // Set at MapController.RedrawMap()
    public static Vector3Int indexNormalizeVector;
    public static Vector2Int gridRrowsIndexLimits; // [-sth, sth_else]
    public static Vector2Int gridColumnsIndexLimits;

    // Map and population settings
    public InputField mapRows;
    public InputField mapColumns;
    public InputField agentsPerVillage;
    public InputField totalResourcesRequired;
    public InputField maxBirthChance;
    public InputField maxDnaMutationChance;
    // Resources settings
    public InputField amountOfVillages;
    public InputField amountOfGold;
    public InputField amountOfEnergyPots;
    public InputField amountOfStone;
    public InputField amountOfWood;
    // Misc settings
    public InputField energyPotCost;
    public InputField mapCost;

    private CanvasGroup canvasGroup;
    private Button[] buttons;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        mapController = FindObjectOfType<MapController>();

        buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(CloseButtonClick);
        buttons[1].onClick.AddListener(SaveButtonClick);
    }

    private void Start()
    {
        LoadSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        Settings settings = simSettings;

        string data = JsonUtility.ToJson(settings);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SimulationSettings.json", data);
    }

    private void LoadSettings()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SimulationSettings.json");
            simSettings = JsonUtility.FromJson<Settings>(data);

            if (simSettings == null)
                simSettings = new Settings();
        }
        catch
        {
            simSettings = new Settings();
        }
        SetSettingsToDisplay();

        mapController.RedrawMap(true);

        Debug.Log("Loaded from : " + Application.persistentDataPath + "/SimulationSettings.json");
    }

    private void SetSettingsToDisplay()
    {
        Settings settings = simSettings;

        mapRows.text = settings.mapRows.ToString();
        mapColumns.text = settings.mapColumns.ToString();
        agentsPerVillage.text = settings.agentsPerVillage.ToString();
        totalResourcesRequired.text = settings.totalResourcesRequired.ToString();
        maxBirthChance.text = settings.maxBirthChance.ToString();
        maxDnaMutationChance.text = settings.maxDnaMutationChance.ToString();
        amountOfVillages.text = settings.amountOfVillages.ToString();
        amountOfGold.text = settings.amountOfGold.ToString();
        amountOfEnergyPots.text = settings.amountOfEnergyPots.ToString();
        amountOfStone.text = settings.amountOfStone.ToString();
        amountOfWood.text = settings.amountOfWood.ToString();
        energyPotCost.text = settings.energyPotCost.ToString();
        mapCost.text = settings.mapCost.ToString();
    }

    private bool GetSettingsFromDisplay()
    {
        int mapRs = simSettings.mapRows;
        int mapCs = simSettings.mapColumns;
        Settings new_settings = simSettings;

        int i_val;
        float f_val;
        // Map rows [100,500] and integer and even
        if (int.TryParse(mapRows.text, out i_val))
        {
            new_settings.mapRows = Mathf.Clamp(i_val, 100, 500);
            // If value is odd remove 1
            new_settings.mapRows -= new_settings.mapRows % 2;
        }

        // Map columns [100,500] and integer and even
        if (int.TryParse(mapColumns.text, out i_val))
        {
            new_settings.mapColumns = Mathf.Clamp(i_val, 100, 500);
            // If value is odd remove 1
            new_settings.mapColumns -= new_settings.mapColumns % 2;
        }

        // Agents per village [4,10] and integer
        if (int.TryParse(agentsPerVillage.text, out i_val))
            new_settings.agentsPerVillage = Mathf.Clamp(i_val, 4, 10);

        // Total resources [1, 1/100 of max space] and integer
        if (int.TryParse(totalResourcesRequired.text, out i_val))
            new_settings.totalResourcesRequired = Mathf.Clamp(i_val, 1, Mathf.FloorToInt((new_settings.mapRows * new_settings.mapColumns) / 100f));

        // Max birth chance [0,100] and float
        if (float.TryParse(maxBirthChance.text, out f_val))
            new_settings.maxBirthChance = Mathf.Clamp(f_val, 0, 100);

        // Max mutation chance [0,100] and float
        if (float.TryParse(maxDnaMutationChance.text, out f_val))
            new_settings.maxDnaMutationChance = Mathf.Clamp(f_val, 0, 100);

        // Amount of villages [2, maxVillages] and integer
        if (int.TryParse(amountOfVillages.text, out i_val))
            new_settings.amountOfVillages = Mathf.Clamp(i_val, 2, maxVillages);

        // Amount of gold [30, 1/50 of max space] and integer
        if (int.TryParse(amountOfGold.text, out i_val))
            new_settings.amountOfGold = Mathf.Clamp(i_val, 30, Mathf.FloorToInt((new_settings.mapRows * new_settings.mapColumns) / 50f));

        // Amount of energy pots [30, 1/50 of max space] and integer
        if (int.TryParse(amountOfEnergyPots.text, out i_val))
            new_settings.amountOfEnergyPots = Mathf.Clamp(i_val, 30, Mathf.FloorToInt((new_settings.mapRows * new_settings.mapColumns) / 50f));

        // Amount of stone [30, 1/50 of max space] and integer
        if (int.TryParse(amountOfStone.text, out i_val))
            new_settings.amountOfStone = Mathf.Clamp(i_val, 30, Mathf.FloorToInt((new_settings.mapRows * new_settings.mapColumns) / 50f));

        // Amount of wood [30, 1/50 of max space] and integer
        if (int.TryParse(amountOfWood.text, out i_val))
            new_settings.amountOfWood = Mathf.Clamp(i_val, 30, Mathf.FloorToInt((new_settings.mapRows * new_settings.mapColumns) / 50f));

        // Cost of energy pots [0, 1/10 of gold on map] and integer
        if (int.TryParse(energyPotCost.text, out i_val))
            new_settings.energyPotCost = Mathf.Clamp(i_val, 0, Mathf.FloorToInt(new_settings.amountOfGold / 10f));

        // Cost of map [0, 1/10 of gold on map] and integer
        if (int.TryParse(mapCost.text, out i_val))
            new_settings.mapCost = Mathf.Clamp(i_val, 0, Mathf.FloorToInt(new_settings.amountOfGold / 10f));

        bool redraw = true;
        if (mapRs == new_settings.mapRows && mapCs == new_settings.mapColumns)
            redraw = false;

        simSettings = new_settings;

        return redraw;
    }

    private void SaveButtonClick()
    {
        bool redraw = GetSettingsFromDisplay();

        // Gets settings and updates settings display with possible clamped values
        SetSettingsToDisplay();

        // Saves new settings
        SaveSettings();

        // Redraw map
        mapController.RedrawMap(false, redraw);

        UIManager.SetCanvasState(canvasGroup, false);
    }

    public void CloseButtonClick()
    {
        // Reset display to current settings
        SetSettingsToDisplay();
        UIManager.SetCanvasState(canvasGroup, false);
    }
}
