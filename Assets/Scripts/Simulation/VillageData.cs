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

        public bool AddItem(Placeable.Type type, int count, int maxType, int maxTotal)
        {
            int totalAdd = Mathf.Clamp(count, 0, Mathf.Max(0, maxType - GetItemCount(type)));
            //Debug.Log("Type : " + type.ToString() + " count : " + count + " -> " + Mathf.Clamp(count, 0, Mathf.Max(0, maxType - GetItemCount(type))) + " with " + maxType + " - " + GetItemCount(type));

            if (storedItems.ContainsKey(type))
                storedItems[type] += count;
            else
                storedItems.Add(type, count);

            totalItems += totalAdd;

            if (totalItems >= maxTotal)
                return true;

            return false;
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

    private System.Random rand;

    public Dictionary<Placeable.Type, int> reqResources;
    public int reqTotal;

    private void Awake()
    {
        npcs = new List<NpcData>();
        number = 0;

        rand = new System.Random();
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;

        if (villageDataDisplay != null)
            Destroy(villageDataDisplay.gameObject);
    }

    private void SimulationStatus(bool running)
    {
        if (running)
        {
            storage = new Storage();
            CreateResourceRequirements();
        }
    }

    private void CreateResourceRequirements()
    {
        reqResources = new Dictionary<Placeable.Type, int>();
        reqResources.Add(Placeable.Type.Gold, 0);
        reqResources.Add(Placeable.Type.Stone, 0);
        reqResources.Add(Placeable.Type.Wood, 0);
        reqTotal = 0;

        reqTotal = SimulationSettings.simSettings.totalResourcesRequired;

        reqResources[Placeable.Type.Gold] = rand.Next(1, Mathf.CeilToInt(reqTotal * 0.4f) + 1);
        reqResources[Placeable.Type.Stone] = rand.Next(1, Mathf.CeilToInt(reqTotal * 0.4f) + 1);
        reqResources[Placeable.Type.Wood] = reqTotal - reqResources[Placeable.Type.Gold] - reqResources[Placeable.Type.Stone];

        villageDataDisplay.UpdateCount();
    }

    public void SpawnNpcs(GameObject npcPrefab)
    {
        npcs.Clear();
        GameObject gm;
        for (int i = 0; i < SimulationSettings.simSettings.agentsPerVillage; i++)
        {
            gm = Instantiate(npcPrefab, gameObject.transform);
            npcs.Add(gm.GetComponent<NpcData>());
            npcs[i].SetColor(SimulationData.GetVillageColor(number));
        }
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
    }

    public NpcData createNpc(GameObject npcPrefab, Vector2Int position) 
    {
        GameObject gm;
        gm = Instantiate(npcPrefab, gameObject.transform);
        npcs.Add(gm.GetComponent<NpcData>());
        npcs[npcs.Count - 1].SetColor(SimulationData.GetVillageColor(number));
        npcs[npcs.Count - 1].npcBehaviour.mapPosition = position;
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();

        return npcs[npcs.Count - 1];
    }

    public void AddResource(Dictionary<Placeable.Type, int> resources)
    {
        foreach (KeyValuePair<Placeable.Type, int> resource in resources)
        {
            if (storage.AddItem(resource.Key, resource.Value, reqResources[resource.Key], reqTotal))
            {
                SimulationLogic.current.VillageWon(this);
            }
        }
        if (villageDataDisplay != null)
            villageDataDisplay.UpdateCount();
    }

    public void UpdateView()
    {
        villageDataDisplay.villageName.text = "Village " + number;
        //villageDataDisplay.UpdateCount();
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();
        villageDataDisplay.villageColor.color = SimulationData.GetVillageColor(number);
    }

    public void NpcRemoved(GameObject npc)
    {
        int index = npcs.FindIndex(n => n.gameObject.GetInstanceID() == npc.GetInstanceID());
        if (index == -1)
            return;

        npcs.RemoveAt(index);
        villageDataDisplay.npcCount.text = "Npcs : " + npcs.Count.ToString();

        if (npcs.Count == 0)
            SimulationLogic.current.VillageEmpty();
    }

    public void ChangeColor() 
    {
        if (number <= 0 || number > 10)
            return;

        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", SimulationData.GetVillageColor(number));
    }
}
