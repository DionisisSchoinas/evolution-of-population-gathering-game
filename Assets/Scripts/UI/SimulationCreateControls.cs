using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationCreateControls : MonoBehaviour
{
    public CanvasGroup simulationSettingsEditorPanel;
    public MapRandomizer mapRandomizer;

    private SimulationSettings simulationSettings;

    private void Awake()
    {
        simulationSettings = simulationSettingsEditorPanel.gameObject.GetComponent<SimulationSettings>();

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(SimSettngsClick);
        buttons[1].onClick.AddListener(RandomizeMapClick);
        buttons[2].onClick.AddListener(StartSimClick);
    }

    private void SimSettngsClick()
    {
        UIManager.SetCanvasState(simulationSettingsEditorPanel, true);
    }

    private void RandomizeMapClick()
    {
        mapRandomizer.RandomizeMap();
    }

    private void StartSimClick()
    {
        simulationSettings.CloseButtonClick();
        SimulationLogic.current.SimulationRunning(true);
    }
}
