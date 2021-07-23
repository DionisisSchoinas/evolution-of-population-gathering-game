using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationData : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject villageDisplayList;
    public VillageDataDisplay villageDataDisplayPrefab;
    public Color[] villageColors = new Color[SimulationSettings.maxVillages];
    public static Color[] villagesColors;
    public List<VillageData> villages;

    private void Awake()
    {
        ClearVillages();
        villagesColors = villageColors;
    }

    private void OnValidate()
    {
        if (villageColors.Length != SimulationSettings.maxVillages)
        {
            Debug.LogWarning("Don't change the 'ints' field's array size!");
            Array.Resize(ref villageColors, SimulationSettings.maxVillages);
        }
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
        VillageDataDisplay dataDisplay = Instantiate(villageDataDisplayPrefab, villageDisplayList.transform);
        villageData.villageDataDisplay = dataDisplay;
        villageData.UpdateView();

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
