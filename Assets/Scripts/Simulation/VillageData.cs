using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageData : MonoBehaviour
{
    [Serializable]
    public class Storage
    {
        public List<StorageItem> storedItems = new List<StorageItem>();

        public void AddItem(Placeable.Type type)
        {
            foreach (StorageItem sItem in storedItems)
            {
                if (sItem.type == type)
                {
                    sItem.count++;
                    return;
                }
            }
            storedItems.Add(new StorageItem(type, 1));
        }
    }

    [Serializable]
    public class StorageItem
    {
        public Placeable.Type type;
        public int count = 0;

        public StorageItem(Placeable.Type type, int count)
        {
            this.type = type;
            this.count = count;
        }
    }

    public int number;
    public Vector2Int arrayPosition;
    public List<NpcData> npcs;
    public List<NpcBehaviour> npcsBehaviour;

    public Storage storage;

    private void Awake()
    {
        number = 0;
        npcs = new List<NpcData>();
        npcsBehaviour = new List<NpcBehaviour>();
        storage = new Storage();
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

    public void AddResource(List<string> resources)
    {
        foreach (string res in resources)
        {
            storage.AddItem(MapController.MapBuildingToEnum(res));
        }
    }
}
