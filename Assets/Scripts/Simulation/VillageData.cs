using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageData : MonoBehaviour
{
    public int number;
    public Vector2Int arrayPosition;
    public List<NpcData> npcs;
    public List<NpcBehaviour> npcsBehaviour;

    private void Awake()
    {
        number = 0;
        npcs = new List<NpcData>();
        npcsBehaviour = new List<NpcBehaviour>();
    }

    public void SpawnNpcs(GameObject npcPrefab)
    {
        GameObject gm;
        for (int i = 0; i < SimulationSettings.simSettings.agentsPerVillage; i++)
        {
            gm = Instantiate(npcPrefab, gameObject.transform);
            npcs.Add(gm.GetComponent<NpcData>());
        }
    }
}
