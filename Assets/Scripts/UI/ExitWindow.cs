using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitWindow : MonoBehaviour
{
    public Button exitSimulationButton;
    public Button detailsButton;
    public CanvasGroup detailsPanel;
    public Button statsButton;
    public CanvasGroup statsPanel;
    public Button exitApplicationButton;

    private CanvasGroup canvasGroup;
    private SimulationStatisticsDisplay statisticsDisplay;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        UIManager.SetCanvasState(canvasGroup, false);

        statisticsDisplay = statsPanel.gameObject.GetComponent<SimulationStatisticsDisplay>();

        exitSimulationButton.onClick.AddListener(ExitSimulation);
        exitSimulationButton.gameObject.SetActive(false);
        detailsButton.onClick.AddListener(Details);
        statsButton.onClick.AddListener(Statistics);
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
        if (canvasGroup.alpha == 0f && Input.GetKeyDown(KeyCode.Escape)) // If hidden show menu
        {
            UIManager.SetCanvasState(canvasGroup, true);
        }
        else if (canvasGroup.alpha == 1f && Input.GetKeyDown(KeyCode.Escape)) // If showing hide menu
        {
            UIManager.SetCanvasState(canvasGroup, false);
        }
    }

    private void SimulationStatus(bool running)
    {
        exitSimulationButton.gameObject.SetActive(running);
        statsButton.gameObject.SetActive(!running);
        UIManager.SetCanvasState(canvasGroup, false);
    }

    private void ExitSimulation()
    {
        SimulationLogic.current.SimulationRunning(false);
    }

    private void Details()
    {
        UIManager.SetCanvasState(detailsPanel, true);
    }

    private void Statistics()
    {
        statisticsDisplay.LoadStatistics();
        UIManager.SetCanvasState(statsPanel, true);
    }

    private void ExitApplication()
    {
        Application.Quit();
    }
}
