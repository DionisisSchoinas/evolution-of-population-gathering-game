using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageData : MonoBehaviour
{
    [Serializable]
    public class Storage
    {
        public Dictionary<Placeable.Type, int> storedItems = new Dictionary<Placeable.Type, int>();

        public int totalItems = 0;

        public void AddItem(Placeable.Type type, int count)
        {
            if (storedItems.ContainsKey(type))
                storedItems[type] += count;
            else
                storedItems.Add(type, count);
            
            totalItems++;
        }

        public int GetItemCount(Placeable.Type type)
        {
            if (storedItems.ContainsKey(type))
                return storedItems[type];
            return 0;
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
            ChangeColor();
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
            npcs[i].SetColor(SimulationData.villagesColors[number - 1]);
        }
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }
    public void createNpc(GameObject npcPrefab,string genome,Vector2Int position) {
        GameObject gm;
        gm = Instantiate(npcPrefab, gameObject.transform);
        npcs.Add(gm.GetComponent<NpcData>());
        npcs[npcs.Count-1].SetColor(SimulationData.villagesColors[number - 1]);
        npcs[npcs.Count - 1].npcBehaviour.mapPosition = position;
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }
    public void AddResource(Dictionary<Placeable.Type, int> resources)
    {
        foreach (KeyValuePair<Placeable.Type, int> resource in resources)
        {
            storage.AddItem(resource.Key, resource.Value);
        
        }
        if (villageDataDisplay != null)
            villageDataDisplay.UpdateCount();
    }

    public void UpdateView()
    {
        villageDataDisplay.villageName.text = "Village " + number;
        villageDataDisplay.UpdateCount();
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
        villageDataDisplay.villageColor.color = SimulationData.villagesColors[number - 1];
    }

    public void NpcRemoved(GameObject npc)
    {
        int index = npcs.FindIndex(n => n.gameObject.GetInstanceID() == npc.GetInstanceID());
        if (index == -1)
            return;

        npcs.RemoveAt(index);
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }

    public void ChangeColor() 
    {
        if (number <= 0 || number > 10)
            return;

        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", SimulationData.villagesColors[number - 1]);
    }
}
