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
                            
                            agenstToMateNumber += 1;
                            agentsToMate.Add(new NpcBehaviour[] { agentPrime, agent });
                        }
                    }
                }
            }
        }
        foreach (NpcBehaviour[] couple in agentsToMate){
            if (UnityEngine.Random.Range(0, 100)<20) {
                if (couple[0].myVillage.number == couple[1].myVillage.number) {
                    Debug.Log("Mating from the same village");
                    villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, crossGenome(couple[0].getGenome(), couple[1].getGenome()),couple[0].mapPosition);
                    villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, crossGenome(couple[0].getGenome(), couple[1].getGenome()), couple[1].mapPosition);

                }
                else
                {
                    Debug.Log("Mating from different village");
                    villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, crossGenome(couple[0].getGenome(), couple[1].getGenome()), couple[0].mapPosition);
                    villages[couple[1].myVillage.number - 1].createNpc(npcPrefab, crossGenome(couple[0].getGenome(), couple[1].getGenome()), couple[1].mapPosition);
                     
                }
                couple[0].destroyAgent();
                couple[1].destroyAgent();
            }

        }
    }
    //find a better solution
    private bool hasCouple(NpcBehaviour agent) {
        foreach (NpcBehaviour[] couple in agentsToMate) {
            if (couple[0] == agent || couple[1] == agent) {
                return true;
            }        
        }
        return false;
    }

    private string crossGenome(string genome1, string genome2)
    {
        int splitingIndex = UnityEngine.Random.Range(0, genome1.Length);
        return genome1.Substring(0, splitingIndex) + genome2.Substring(splitingIndex , genome2.Length-splitingIndex);
    }
}
