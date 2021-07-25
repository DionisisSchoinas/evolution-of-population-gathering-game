using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOverlay : MonoBehaviour
{
    Material material;

    private void Awake()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
        material.SetFloat("_Alpha", 0f);
        material.SetColor("_Color", Color.red);
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
        SimulationLogic.current.onSetMapOverlayAlpha += SetAlpha;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
        SimulationLogic.current.onSetMapOverlayAlpha -= SetAlpha;
    }

    private void SimulationStatus(bool running)
    {
        SetAlpha(0f);
    }

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", Mathf.Clamp(alpha, 0f, 1f));
    }
}
