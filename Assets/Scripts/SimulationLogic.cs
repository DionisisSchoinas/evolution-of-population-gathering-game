using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLogic : MonoBehaviour
{
    public int ticks = 0;
    public bool simulationRunning;
    public bool autoRun;

    private SimulationData simulationData;

    public static SimulationLogic current;

    private void Awake()
    {
        simulationData = gameObject.GetComponent<SimulationData>();
        simulationRunning = false;
        autoRun = false;

        current = this;
    }

    private void Start()
    {
        current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        current.onSimulationRunning -= SimulationStatus;
    }

    public Action<int> onTick;
    public void Tick(int ticks)
    {
        if (onTick != null)
        {
            onTick(ticks);
        }
    }

    public Action<bool> onSimulationRunning;
    public void SimulationRunning(bool running)
    {
        if (onSimulationRunning != null)
        {
            onSimulationRunning(running);
        }
    }

    public Action<float> onSetMapOverlayAlpha;
    public void SetMapOverlayAlpha(float alpha)
    {
        if (onSetMapOverlayAlpha != null)
        {
            onSetMapOverlayAlpha(alpha);
        }
    }

    private void SimulationStatus(bool running)
    {
        if (running)
        {
            ticks = 0;
            simulationData.SpawnNpcs();
        }

        simulationRunning = running;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && simulationRunning)
        {
            autoRun = !autoRun;
        }

        if ((Input.GetKey(KeyCode.K) || Input.GetKeyDown(KeyCode.L) || autoRun) && simulationRunning)
        {
            ticks += 1;
            current.Tick(ticks);
        }
    }
}
