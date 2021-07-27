using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLogic : MonoBehaviour
{
    public int ticks = 0;
    public bool simulationRunning;
    public bool autoRun;
    public bool villageWon;

    public static SimulationLogic current;

    private void Awake()
    {
        simulationRunning = false;
        autoRun = false;
        villageWon = false;

        current = this;
    }

    private void Start()
    {
        current.onSimulationRunning += SimulationStatus;
        current.onVillageWon += VillageVictory;
    }

    private void OnDestroy()
    {
        current.onSimulationRunning -= SimulationStatus;
        current.onVillageWon -= VillageVictory;
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

    public Action<VillageData> onVillageWon;
    public void VillageWon(VillageData villageData)
    {
        if (onVillageWon != null)
        {
            onVillageWon(villageData);
        }
    }

    private void SimulationStatus(bool running)
    {
        if (running)
        {
            ticks = 0;
            autoRun = false;
            villageWon = false;
        }

        simulationRunning = running;
    }

    private void VillageVictory(VillageData villageData)
    {
        villageWon = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (villageWon)
            return;

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
