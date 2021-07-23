using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLogic : MonoBehaviour
{
    public int ticks = 0;
    public bool simulationRunning;

    private SimulationData simulationData;

    public static SimulationLogic current;

    private void Awake()
    {
        simulationData = gameObject.GetComponent<SimulationData>();
        simulationRunning = false;

        current = this;
    }

    public Action<int> onTick;
    public void Tick(int ticks)
    {
        if (onTick != null)
        {
            onTick(ticks);
        }
    }

    public void StartSimulation()
    {
        simulationData.SpawnNpcs();
        simulationRunning = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.L) && simulationRunning)
        {
            ticks += 1;
            current.Tick(ticks);
        }
    }
}
