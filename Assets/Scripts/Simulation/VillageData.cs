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
        public int totalItems = 0;

        public void AddItem(Placeable.Type type)
        {
            foreach (StorageItem sItem in storedItems)
            {
                if (sItem.type == type)
                {
                    sItem.count++;
                    totalItems++;
                    return;
                }
            }
            storedItems.Add(new StorageItem(type, 1));
            totalItems++;
        }

        public int GetItemCount(Placeable.Type type)
        {
            foreach (StorageItem storageItem in storedItems)
            {
                if (storageItem.type == type)
                    return storageItem.count;
            }
            return 0;
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

    private int _number;
    public int number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
            if (villageDataDisplay != null)
                villageDataDisplay.UpdateCount(storage);
        }
    }

    public Vector2Int arrayPosition;
    public List<NpcData> npcs;

    public Storage storage;

    public VillageDataDisplay villageDataDisplay;

    private void Awake()
    {
        npcs = new List<NpcData>();
        storage = new Storage();
        number = 0;
    }

    private void OnDestroy()
    {
        if (villageDataDisplay != null)
            Destroy(villageDataDisplay.gameObject);
    }

    public void SpawnNpcs(GameObject npcPrefab)
    {
        GameObject gm;
        for (int i = 0; i < SimulationSettings.simSettings.agentsPerVillage; i++)
        {
            gm = Instantiate(npcPrefab, gameObject.transform);
            npcs.Add(gm.GetComponent<NpcData>());
        }
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }

    public void AddResource(List<string> resources)
    {
        foreach (string res in resources)
        {
            storage.AddItem(MapController.MapBuildingToEnum(res));
        }
        if (villageDataDisplay != null)
            villageDataDisplay.UpdateCount(storage);
    }

    public void UpdateView()
    {
        villageDataDisplay.villageName.text = "Village " + number;
        villageDataDisplay.UpdateCount(storage);
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }

    public void NpcRemoved(GameObject npc)
    {
        int index = npcs.FindIndex(n => n.gameObject.GetInstanceID() == npc.GetInstanceID());
        if (index == -1)
            return;

        npcs.RemoveAt(index);
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }
}
