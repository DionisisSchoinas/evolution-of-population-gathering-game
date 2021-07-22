using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationData : MonoBehaviour
{
    public GameObject npcPrefab;
    public List<VillageData> villages;

    private void Awake()
    {
        ClearVillages();
    }

    public void SpawnNpcs()
    {
        foreach (VillageData villageData in villages)
        {
            villageData.SpawnNpcs(npcPrefab);
        }
    }

    public void AddVillage(VillageData villageData)
    {
        villages.Add(villageData);
    }

    public void RemoveVillage(VillageData villageData)
    {
        villages.Remove(villageData);
        for (int i = 0; i < villages.Count; i++)
        {
            villages[i].number = i + 1;
        }
    }

    public void ClearVillages()
    {
        villages = new List<VillageData>();
    }
}