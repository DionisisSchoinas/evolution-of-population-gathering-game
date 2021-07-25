using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CanvasGroup buildingPanel;
    public CanvasGroup simSettingsPanel;
    public CanvasGroup simSettingsEditorPanel;
    public CanvasGroup simulationStatusDisplay;
    public CanvasGroup simulationVillageDetailsDisplay;

    private void Awake()
    {
        SetCanvasState(buildingPanel, true);
        SetCanvasState(simSettingsPanel, true);
        SetCanvasState(simSettingsEditorPanel, false);
        SetCanvasState(simulationStatusDisplay, false);
        SetCanvasState(simulationVillageDetailsDisplay, false);
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
    }

    private void SimulationStatus(bool running)
    {
        SetCanvasState(buildingPanel, !running);
        SetCanvasState(simSettingsPanel, !running);
        SetCanvasState(simSettingsEditorPanel, false);

        SetCanvasState(simulationStatusDisplay, running);
        SetCanvasState(simulationVillageDetailsDisplay, running);
    }

    public static void SetCanvasState(CanvasGroup canvasGroup, bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }
}
