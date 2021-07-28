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
        villagesColors = villageColors;
    }

    private void Start()
    {
        SimulationLogic.current.onTick += Tick;
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
    }

    private void Tick(int ticks)
    {
        mateAgents();
    }

    private void SimulationStatus(bool running)
    {
        if (running)
        {
            SpawnNpcs();
        }
        else
        {
            ClearVillages();
        }
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
        agents = new List<NpcBehaviour>();
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
            if (UnityEngine.Random.Range(0, 100) < SimulationSettings.simSettings.maxBirthChance) {

                NpcData child1;
                NpcData child2;
                if (couple[0].myVillage.number == couple[1].myVillage.number) 
                {
                    child1 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[0].mapPosition);
                    child2 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[1].mapPosition);
                }
                else
                {
                    child1 = villages[couple[0].myVillage.number - 1].createNpc(npcPrefab, couple[0].mapPosition);
                    child2 = villages[couple[1].myVillage.number - 1].createNpc(npcPrefab, couple[1].mapPosition);
                }
                //Swap
                int splitingIndex = UnityEngine.Random.Range(0, couple[0].getGenome().Length);
                child1.updateGenome(crossGenome(couple[0].getGenome(), couple[1].getGenome(), splitingIndex));
                child2.updateGenome(crossGenome(couple[1].getGenome(), couple[0].getGenome(), splitingIndex));
             
                //distribute energy pots
                int totalEnergyPots = couple[0].npcData._energyPots + couple[1].npcData._energyPots;
                child1.energyPots = totalEnergyPots/2;
                child2.energyPots = totalEnergyPots - child1.energyPots;
                
                //distribute gold
                int totalCoins = couple[0].npcData.gold + couple[1].npcData.gold;
                child1.gold = totalCoins / 2;
                child2.gold = totalCoins - child1.gold;
               
                //distribute resources
                int totalGold = couple[0].npcData.GetItem(Placeable.Type.Gold) + couple[1].npcData.GetItem(Placeable.Type.Gold);
                int totalWood = couple[0].npcData.GetItem(Placeable.Type.Wood) + couple[1].npcData.GetItem(Placeable.Type.Wood);
                int totalStone = couple[0].npcData.GetItem(Placeable.Type.Stone) + couple[1].npcData.GetItem(Placeable.Type.Stone); 

                child1.AddResource(Placeable.Type.Gold,totalGold/2);
                child2.AddResource(Placeable.Type.Gold, totalGold - (totalGold / 2));

                child1.AddResource(Placeable.Type.Wood, totalWood / 2);
                child2.AddResource(Placeable.Type.Wood, totalWood - (totalWood / 2));

                child1.AddResource(Placeable.Type.Stone, totalStone / 2);
                child2.AddResource(Placeable.Type.Stone, totalStone - (totalStone / 2));

                //trade knowledge 
                List<Vector2Int> knownWoodOres = new List<Vector2Int>();
                knownWoodOres.AddRange(couple[0].knownWoodOres);
                knownWoodOres.AddRange(couple[1].knownWoodOres);
               
                List<Vector2Int> knownGoldOres = new List<Vector2Int>();
                knownGoldOres.AddRange(couple[0].knownGoldOres);
                knownGoldOres.AddRange(couple[1].knownGoldOres);

                List<Vector2Int> knownStoneOres = new List<Vector2Int>();
                knownStoneOres.AddRange(couple[0].knownStoneOres);
                knownStoneOres.AddRange(couple[1].knownStoneOres);

                string[,] crossReferencedMap = crossRefereceMap(couple[0].localMapData, couple[1].localMapData);
                child1.npcBehaviour.localMapData = crossReferencedMap;
                child2.npcBehaviour.localMapData = crossReferencedMap;
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
        return genome;
    }

    private string[,] crossRefereceMap(string[,] map1, string[,] map2)
    {
        string[,] mapData = new string[SimulationSettings.simSettings.mapRows, SimulationSettings.simSettings.mapColumns];
        
        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                if (j == 0 || i == 0 || i == mapData.GetLength(0) - 1 || j == mapData.GetLength(1) - 1)
                {
                    mapData[i, j] = "e";
                }
                else
                {
                    if (map1[i, j] != "u")
                    {
                        mapData[i, j] = map1[i, j];
                    }
                    else {
                        mapData[i, j] = map2[i, j];
                    }
               
                }
            }
        }
        return mapData;


    }
}