using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationData : MonoBehaviour
{
    public GameObject npcPrefab;
    public List<NpcBehaviour> agents;
    public List<Vector2Int> agentPositions;
    public GameObject villageDisplayList;
    public VillageDataDisplay villageDataDisplayPrefab;
    public Color[] villageColors = new Color[SimulationSettings.maxVillages];
    public static Color[] villagesColors;
    public List<VillageData> villages;
    private List<NpcBehaviour[]> agentsToMate = new List<NpcBehaviour[]>();
    public int agenstToMateNumber=0;
    private void Awake()
    {
        ClearVillages();
        agents = new List<NpcBehaviour>();
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
        dataDisplay.villageData = villageData;
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

    public void updateAgent(NpcBehaviour agent) {
        if (!agents.Contains(agent))
        {
            agents.Add(agent);
        }
      
    }
    public void mateAgents()
    {
        agentsToMate.Clear();
        agenstToMateNumber = 0;
        foreach (NpcBehaviour agentPrime in agents)
        {
            foreach (NpcBehaviour agent in agents)
            {
                if (agentPrime != agent)
                {
                    Vector2Int distance = new Vector2Int(Math.Abs(agentPrime.mapPosition.x - agent.mapPosition.x), Math.Abs(agentPrime.mapPosition.y - agent.mapPosition.y));
                    if (distance.x <= 1 && distance.y <= 1) {
                        if (!hasCouple(agentPrime))
                        {
                            Debug.Log("Agents are close");
                            agenstToMateNumber += 1;
                            agentsToMate.Add(new NpcBehaviour[] { agentPrime, agent });
                        }
                        else {
                            Debug.Log("Agents have mates");

                        }
                    }
                }
            }
        }
    }
    private bool hasCouple(NpcBehaviour agent) {
        foreach (NpcBehaviour[] couple in agentsToMate) {
            if (couple[0] == agent || couple[1] == agent) {
                return true;
            }        
        }
        return false;
    }
}
