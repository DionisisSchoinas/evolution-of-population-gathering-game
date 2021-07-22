using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public CanvasGroup buildingPanel;
    public CanvasGroup simSettingsPanel;
    public CanvasGroup simSettingsEditorPanel;

    private void Awake()
    {
        SetCanvasState(buildingPanel, true);
        SetCanvasState(simSettingsPanel, true);
        SetCanvasState(simSettingsEditorPanel, false);
    }

    public static void SetCanvasState(CanvasGroup canvasGroup, bool show)
    {
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
    }

    public void StartSimulation()
    {
        SetCanvasState(buildingPanel, false);
        SetCanvasState(simSettingsPanel, false);
        SetCanvasState(simSettingsEditorPanel, false);
    }
}
