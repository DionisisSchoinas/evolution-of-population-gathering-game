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
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
    }

    private void Tick(int ticks)
    {
        ticksDisplay.text = "Ticks : " + ticks;
    }
}
