using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationStatisticsDisplay : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public Button closeButton;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        UIManager.SetCanvasState(canvasGroup, false);

        closeButton.onClick.AddListener(
            delegate
            {
                UIManager.SetCanvasState(canvasGroup, false);
            }
        );
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
        UIManager.SetCanvasState(canvasGroup, false);
    }
}
