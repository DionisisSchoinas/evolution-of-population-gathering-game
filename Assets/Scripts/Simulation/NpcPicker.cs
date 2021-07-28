using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPicker : MonoBehaviour
{
    private GridSnapping gridSnapping;
    private SimulationVillageDisplay simulationVillageDisplay;

    private void Awake()
    {
        gridSnapping = FindObjectOfType<GridSnapping>();
        simulationVillageDisplay = FindObjectOfType<SimulationVillageDisplay>();
    }

    void Update()
    {
        if (SimulationLogic.current.simulationRunning && Input.GetKeyDown(KeyCode.Mouse0))
        {
            CheckForNpc();
        }
    }

    private void CheckForNpc()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            NpcData npcData = hit.transform.gameObject.GetComponent<NpcData>();
            if (npcData != null)
            {
                if (npcData.npcBehaviour.myVillage.number != simulationVillageDisplay.activeVillage)
                    simulationVillageDisplay.ShowNpcs(npcData.npcBehaviour.myVillage);


                SimulationLogic.current.ShowMapOverlay(npcData);

                //gridSnapping.ShowMapOverlay(npcData);
            }
        }
    }
}
