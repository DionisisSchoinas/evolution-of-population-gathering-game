using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationVillageDisplay : MonoBehaviour
{
    public Text villageName;
    public NpcDataDisplay npcDataDisplayPrefab;
    public Transform npcDisplayPanel;

    private void Start()
    {
        villageName.text = "";
    }

    public void ShowNpcs(VillageData villageData)
    {
        ClearNpcs();
        villageName.text = "Village " + villageData.number.ToString();
        NpcDataDisplay npcDataDisplay;
        foreach (NpcData npc in villageData.npcs)
        {
            npcDataDisplay = Instantiate(npcDataDisplayPrefab, npcDisplayPanel);
            npcDataDisplay.npcData = npc;
            npc.dataDisplay = npcDataDisplay;
            npcDataDisplay.UpdateView();
        }
    }

    public void ClearNpcs()
    {
        NpcDataDisplay[] children = gameObject.GetComponentsInChildren<NpcDataDisplay>();
        foreach (NpcDataDisplay child in children)
            Destroy(child.gameObject);
    }
}
