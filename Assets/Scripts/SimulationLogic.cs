using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLogic : MonoBehaviour
{
    public int ticks = 0;

    public static SimulationLogic current;

    private void Awake()
    {
        current = this;
    }

    public Action onTick;
    public void Tick()
    {
        if (onTick != null)
        {
            onTick();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            ticks += 1;
            current.Tick();
        }
    }
}
