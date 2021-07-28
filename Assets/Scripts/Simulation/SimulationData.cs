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
    private List<NpcBehaviour[]> agentsToInteract = new List<NpcBehaviour[]>();
    public int agentsToInteractNumber = 0;
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
        agentInteraction();
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
    public void agentInteraction()
    {
        agentsToInteract.Clear();
        agentsToInteractNumber = 0;
        foreach (NpcBehaviour agentPrime in agents)
        {
            foreach (NpcBehaviour agent in agents)
            {
                if (agentPrime != agent)
                {
                    Vector2Int distance = new Vector2Int(Math.Abs(agentPrime.mapPosition.x - agent.mapPosition.x), Math.Abs(agentPrime.mapPosition.y - agent.mapPosition.y));
                    if (distance.x <= 1 && distance.y <= 1)
                    {
                        if (!agentPrime.isInteracting && !agent.isInteracting)
                        {
                            agentsToInteractNumber += 1;
                            agentsToInteract.Add(new NpcBehaviour[] { agentPrime, agent });
                            agentPrime.isInteracting = true;
                            agent.isInteracting = true;
                        }
                    }
                }
            }
        }
        foreach (NpcBehaviour[] couple in agentsToInteract)
        {
            if (UnityEngine.Random.Range(0, 100) < SimulationSettings.simSettings.maxBirthChance)
            {
                mateAgents(couple[0], couple[1]);
            }
            else
            {
                tradeAgentsEnergyPots(couple[0], couple[1]);
                tradeAgentsMapKnowledge(couple[0], couple[1]);
            }
        }
    }
    public void mateAgents(NpcBehaviour agent1,NpcBehaviour agent2) {
        NpcData child1;
        NpcData child2;
        if (agent1.myVillage.number == agent2.myVillage.number)
        {
            child1 = villages[agent1.myVillage.number - 1].createNpc(npcPrefab, agent1.mapPosition);
            child2 = villages[agent1.myVillage.number - 1].createNpc(npcPrefab, agent2.mapPosition);
        }
        else
        {
            child1 = villages[agent1.myVillage.number - 1].createNpc(npcPrefab, agent1.mapPosition);
            child2 = villages[agent2.myVillage.number - 1].createNpc(npcPrefab, agent2.mapPosition);
        }
        //Swap
        int splitingIndex = UnityEngine.Random.Range(0, agent1.getGenome().Length);
        child1.updateGenome(crossGenome(agent1.getGenome(), agent2.getGenome(), splitingIndex));
        child2.updateGenome(crossGenome(agent2.getGenome(), agent1.getGenome(), splitingIndex));

        //distribute energy pots
        int totalEnergyPots = agent1.npcData._energyPots + agent2.npcData._energyPots;
        child1.energyPots = totalEnergyPots / 2;
        child2.energyPots = totalEnergyPots - child1.energyPots;

        //distribute gold
        int totalCoins = agent1.npcData.gold + agent2.npcData.gold;
        child1.gold = totalCoins / 2;
        child2.gold = totalCoins - child1.gold;

        //distribute resources
        int totalGold = agent1.npcData.GetItem(Placeable.Type.Gold) + agent2.npcData.GetItem(Placeable.Type.Gold);
        int totalWood = agent1.npcData.GetItem(Placeable.Type.Wood) + agent2.npcData.GetItem(Placeable.Type.Wood);
        int totalStone = agent1.npcData.GetItem(Placeable.Type.Stone) + agent2.npcData.GetItem(Placeable.Type.Stone);

        child1.AddResource(Placeable.Type.Gold, totalGold / 2);
        child2.AddResource(Placeable.Type.Gold, totalGold - (totalGold / 2));

        child1.AddResource(Placeable.Type.Wood, totalWood / 2);
        child2.AddResource(Placeable.Type.Wood, totalWood - (totalWood / 2));

        child1.AddResource(Placeable.Type.Stone, totalStone / 2);
        child2.AddResource(Placeable.Type.Stone, totalStone - (totalStone / 2));

        //trade knowledge 
        List<Vector2Int> knownWoodOres = new List<Vector2Int>();
        knownWoodOres.AddRange(agent1.knownWoodOres);
        knownWoodOres.AddRange(agent2.knownWoodOres);

        List<Vector2Int> knownGoldOres = new List<Vector2Int>();
        knownGoldOres.AddRange(agent1.knownGoldOres);
        knownGoldOres.AddRange(agent2.knownGoldOres);

        List<Vector2Int> knownStoneOres = new List<Vector2Int>();
        knownStoneOres.AddRange(agent1.knownStoneOres);
        knownStoneOres.AddRange(agent2.knownStoneOres);

        string[,] crossReferencedMap = crossRefereceMap(agent1.localMapData, agent2.localMapData);
        child1.npcBehaviour.localMapData = crossReferencedMap;
        child2.npcBehaviour.localMapData = crossReferencedMap;
        agent1.destroyAgent();
        agent2.destroyAgent();
    }
    public bool tradeAgentsEnergyPots(NpcBehaviour agent1, NpcBehaviour agent2)
    {
<<<<<<< Updated upstream
       // Debug.Log("Asking to trade for energy pots");
=======
        agent1.isInteracting = false;
        agent2.isInteracting = false;
        Debug.Log("Asking to trade for energy pots");
>>>>>>> Stashed changes
        int energyPotAskingPrice = 1;
     
        //agent one asked agent two to trade
        if (agent1.npcData._energyPots <= 1 && agent1.npcData.gold >= energyPotAskingPrice) {
            //ask for trade
            if (agent2.npcData._energyPots > 1)
            {
                //Debug.Log("Accepted trade of energy pots");
                agent2.npcData.gold += energyPotAskingPrice;
                agent1.npcData.gold -= energyPotAskingPrice;

                agent2.npcData._energyPots -= 1;
                agent1.npcData._energyPots += 1;
                return true;
            }
            else {
                //Debug.Log("Refused trade of energy pots");
            }
        }
        //agent two asked agent one to trade
        else if (agent2.npcData._energyPots <= 1 && agent2.npcData.gold >= energyPotAskingPrice)
        {
            //ask for trade
            if (agent1.npcData._energyPots > 1)
            {
                //Debug.Log("Accepted trade of energy pots");
                agent1.npcData.gold += energyPotAskingPrice;
                agent2.npcData.gold -= energyPotAskingPrice;

                agent1.npcData._energyPots -= 1;
                agent2.npcData._energyPots += 1;
                return true;

            }
            else
            {
                //Debug.Log("Refused trade of energy pots");
            }
        }
        return false;
    }
    public bool tradeAgentsMapKnowledge(NpcBehaviour agent1, NpcBehaviour agent2){
        agent1.isInteracting = false;
        agent2.isInteracting = false;
        int mapExcangeAskingPrice = 1;
        //Debug.Log("Asking to trade for ores");
        if (agent1.npcData.gold >= mapExcangeAskingPrice)
        {
            if (agent1.npcData.carryType == 0 && agent2.knownWoodOres.Count > 0)
            {
                //Debug.Log("Trade for wood knowledge");
                agent1.knownWoodOres.AddRange(agent2.knownWoodOres);
                agent1.knownStoneOres.AddRange(agent2.knownStoneOres);
                agent1.knownGoldOres.AddRange(agent2.knownGoldOres);
            }
            else if (agent1.npcData.carryType == 1 && agent2.knownStoneOres.Count > 0)
            {
                //Debug.Log("Trade for stone knowledge");
                agent1.knownWoodOres.AddRange(agent2.knownWoodOres);
                agent1.knownStoneOres.AddRange(agent2.knownStoneOres);
                agent1.knownGoldOres.AddRange(agent2.knownGoldOres);
            }
            else if (agent1.npcData.carryType == 2 && agent2.knownGoldOres.Count > 0)
            {
                //Debug.Log("Trade for gold knowledge");
                agent1.knownWoodOres.AddRange(agent2.knownWoodOres);
                agent1.knownStoneOres.AddRange(agent2.knownStoneOres);
                agent1.knownGoldOres.AddRange(agent2.knownGoldOres);
            }
            else if (agent1.npcData.carryType == 3 && agent2.knownWoodOres.Count + agent2.knownStoneOres.Count + agent2.knownGoldOres.Count > 0)
            {
                //Debug.Log("Trade for any knowledge");
                agent1.knownWoodOres.AddRange(agent2.knownWoodOres);
                agent1.knownStoneOres.AddRange(agent2.knownStoneOres);
                agent1.knownGoldOres.AddRange(agent2.knownGoldOres);
            }
        }
        return false;
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