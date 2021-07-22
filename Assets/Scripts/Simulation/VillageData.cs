using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageData : MonoBehaviour
{
    public int number;
    public List<NpcData> npcs;
    public List<NpcBehaviour> npcsBehaviour;

    private void Awake()
    {
        number = 0;
        npcs = new List<NpcData>();
        npcsBehaviour = new List<NpcBehaviour>();
    }

    private void SpawnNpcs()
    {

    }
}
