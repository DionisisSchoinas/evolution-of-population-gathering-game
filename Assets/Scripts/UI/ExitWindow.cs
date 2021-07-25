using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitWindow : MonoBehaviour
{
    public Button exitSimulationButton;
    public Button detailsButton;
    public Button exitApplicationButton;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        UIManager.SetCanvasState(canvasGroup, false);

        exitSimulationButton.onClick.AddListener(ExitSimulation);
        exitSimulationButton.gameObject.SetActive(false);
        detailsButton.onClick.AddListener(Details);
        exitApplicationButton.onClick.AddListener(ExitApplication);
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
    }

    private void Update()
    {
        if (canvasGroup.alpha == 0f && Input.GetKeyDown(KeyCode.Q)) // If hidden show menu
        {
            UIManager.SetCanvasState(canvasGroup, true);
        }
        else if (canvasGroup.alpha == 1f && Input.GetKeyDown(KeyCode.Q)) // If showing hide menu
        {
            UIManager.SetCanvasState(canvasGroup, false);
        }
    }

    private void SimulationStatus(bool running)
    {
        exitSimulationButton.gameObject.SetActive(running);
        UIManager.SetCanvasState(canvasGroup, false);
    }

    private void ExitSimulation()
    {
        SimulationLogic.current.SimulationRunning(false);
    }

    private void Details()
    {

    }

    private void ExitApplication()
    {
        Application.Quit();
    }
}
