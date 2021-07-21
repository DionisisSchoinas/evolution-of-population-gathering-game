using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSettings : MonoBehaviour
{
    [Serializable]
    public class Settings
    {
        // Map and population settings
        public int mapRows = 100;
        public int mapColumns = 100;
        public int agentsPerVillage = 10;
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
    private Scrollbar master;
    private Scrollbar music;
    private Scrollbar effects;
    private Button[] buttons;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        /*
        Scrollbar[] scrollbars = gameObject.GetComponentsInChildren<Scrollbar>();
        master = scrollbars[0];
        music = scrollbars[1];
        effects = scrollbars[2];
        */
        /*
        master.onValueChanged.AddListener(ChangeMaster);
        music.onValueChanged.AddListener(ChangeMusic);
        effects.onValueChanged.AddListener(ChangeEffects);
        */

        buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(CloseButtonClick);
        buttons[1].onClick.AddListener(SaveButtonClick);

        LoadSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        Settings settings = new Settings();

        string data = JsonUtility.ToJson(settings);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/SimulationSettings.json", data);
    }

    private void LoadSettings()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SimulationSettings.json");
            simSettings = JsonUtility.FromJson<Settings>(data);
        }
        catch
        {
            simSettings = new Settings();
        }

        Debug.Log("Loaded from : " + Application.persistentDataPath);
    }

    private void SaveButtonClick()
    {
        SaveSettings();
        UIManager.SetCanvasState(canvasGroup, false);
    }

    private void CloseButtonClick()
    {
        UIManager.SetCanvasState(canvasGroup, false);
    }
}