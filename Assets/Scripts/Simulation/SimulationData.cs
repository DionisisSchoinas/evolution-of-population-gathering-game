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
    public int agenstToMateNumber = 0;

    private void Awake()
    {
        ClearVillages();
        agents = new List<NpcBehaviour>();
        villagesColors = villageColors;
    }

    private void Start()
    {
        SimulationLogic.current.onTick += Tick;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
    }

    private void Tick(int ticks)
    {
        mateAgents();
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

    public void updateAgent(NpcBehaviour agent)
    {
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
                        if (!agentPrime.hasMate && !agent.hasMate)
                        {
                            agenstToMateNumber += 1;
                            agentsToMate.Add(new NpcBehaviour[] { agentPrime, agent });
                            agentPrime.hasMate = true;
                            agent.hasMate = true;
                        }
                    }
                }
            }
        }
        foreach (NpcBehaviour[] couple in agentsToMate) {
            if (UnityEngine.Random.Range(0, 100) < 20) {

                NpcData child1;
                NpcData child2;
                if (couple[0].myVillage.number == couple[1].myVillage.number) {
                    Debug.Log("Mating from the same village");
                    child1 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[0].mapPosition);
                    child2 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[1].mapPosition);
                }
                else
                {
                    Debug.Log("Mating from different village");
                    child1 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[0].mapPosition);
                    child2 = villages[couple[1].myVillage.number - 1].createNpc(npcPrefab, couple[1].mapPosition);
                }

                //Swap

                couple[0].destroyAgent();
                couple[1].destroyAgent();
            }
        }
    }

    private string crossGenome(string genome1, string genome2, int splitingIndex)
    {
        string genome = genome1.Substring(0, splitingIndex) + genome2.Substring(splitingIndex, genome2.Length - splitingIndex);
        if (UnityEngine.Random.Range(0, 100) < 20)
        {
            int randomIndex = UnityEngine.Random.Range(0, genome.Length);
            if (genome[randomIndex] == '0')
            {
                genome = genome.Substring(0, randomIndex) + '1' + genome.Substring(randomIndex, genome.Length - randomIndex);
            }
            else {
                genome = genome.Substring(0, randomIndex) + '0' + genome.Substring(randomIndex, genome.Length - randomIndex);
            }
        }
        Debug.Log(genome1 + "," + genome2 + "," + genome);

        return genome;
    }
}