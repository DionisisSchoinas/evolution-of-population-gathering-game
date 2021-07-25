using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationStatusDisplay : MonoBehaviour
{
    public Text ticksDisplay;

    private void Start()
    {
        SimulationLogic.current.onTick += Tick;
        SimulationLogic.current.onSimulationRunning += SimulationRunning;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
        SimulationLogic.current.onSimulationRunning -= SimulationRunning;
    }

    private void Tick(int ticks)
    {
        ticksDisplay.text = "Ticks : " + ticks;
    }

    private void SimulationRunning(bool running)
    {
        ticksDisplay.text = "Ticks : 0"; 
    }
}
