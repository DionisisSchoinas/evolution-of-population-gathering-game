using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public VillageDataDisplay villageDataDisplay;
    public Button exitSimButton;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        UIManager.SetCanvasState(canvasGroup, false);

        exitSimButton.onClick.AddListener(ExitSim);
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
        SimulationLogic.current.onVillageWon += VillageWon;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
        SimulationLogic.current.onVillageWon -= VillageWon;
    }

    private void SimulationStatus(bool running)
    {
        UIManager.SetCanvasState(canvasGroup, false);
    }

    private void VillageWon(VillageData villageData)
    {
        UIManager.SetCanvasState(canvasGroup, true);
        villageDataDisplay.villageData = villageData;
        villageDataDisplay.UpdateCount();
        villageDataDisplay.villageName.text = "Village " + villageData.number + " Won";
    }

    private void ExitSim()
    {
        SimulationLogic.current.SimulationRunning(false);
    }
}
