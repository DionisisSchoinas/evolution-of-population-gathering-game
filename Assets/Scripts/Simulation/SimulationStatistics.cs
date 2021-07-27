using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationStatistics : MonoBehaviour
{
    [Serializable]
    public struct GenomeIntPair
    {
        public string genome;
        public int value;

        public GenomeIntPair(string genome, int value)
        {
            this.genome = genome;
            this.value = value;
        }
    }

    [Serializable]
    public class Statistics
    {
        private static int bestOf = 5;

        public GenomeIntPair[] longestLife = new GenomeIntPair[bestOf];
        public GenomeIntPair[] bestGoldCarriers = new GenomeIntPair[bestOf];
        public GenomeIntPair[] bestStoneCarriers = new GenomeIntPair[bestOf];
        public GenomeIntPair[] bestWoodCarriers = new GenomeIntPair[bestOf];

        public SimulationSettings.Settings settings = new SimulationSettings.Settings();
    }

    public static SimulationStatistics current;
    private Statistics simStatistics;

    private void Awake()
    {
        simStatistics = new Statistics();
        current = this;
    }

    private void Start()
    {
        SimulationLogic.current.onSimulationRunning += SimulationStatus;
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onSimulationRunning -= SimulationStatus;
    }

    private void SimulationStatus(bool running)
    {
        if (!running)
            SaveStats();
    }

    private void SaveStats()
    {
        Statistics statistics = simStatistics;
        statistics.settings = SimulationSettings.simSettings;

        string data = JsonUtility.ToJson(statistics);
        int saveIndex = 0;
        if (Directory.Exists(Application.persistentDataPath + "/Simulation Statistics"))
        {
            var info = new DirectoryInfo(Application.persistentDataPath + "/Simulation Statistics");
            var fileInfo = info.GetFiles();
            saveIndex = fileInfo.Length;
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Simulation Statistics");
        }

        File.WriteAllText(Application.persistentDataPath + "/Simulation Statistics/Simulation_" + saveIndex + ".json", data);

    }

    public void NewData(NpcData npcData)
    {
        SortStat(simStatistics.longestLife, 0, simStatistics.longestLife.Length - 1, new GenomeIntPair(npcData.genome, npcData.totalLife));
        SortStat(simStatistics.bestGoldCarriers, 0, simStatistics.bestGoldCarriers.Length - 1, new GenomeIntPair(npcData.genome, LookForType(npcData.resourcesCarried, Placeable.Type.Gold)));
        SortStat(simStatistics.bestStoneCarriers, 0, simStatistics.bestStoneCarriers.Length - 1, new GenomeIntPair(npcData.genome, LookForType(npcData.resourcesCarried, Placeable.Type.Stone)));
        SortStat(simStatistics.bestWoodCarriers, 0, simStatistics.bestWoodCarriers.Length - 1, new GenomeIntPair(npcData.genome, LookForType(npcData.resourcesCarried, Placeable.Type.Wood)));
    }

    // stats, wil be sorted in descending order based on stats[].value
    private void SortStat(GenomeIntPair[] stats, int minIndex, int maxIndex, GenomeIntPair newStat)
    {
        if (minIndex < maxIndex)
        {
            int mid = Mathf.FloorToInt((minIndex + maxIndex) / 2f);

            if (stats[mid].value < newStat.value)
            {
                SortStat(stats, minIndex, mid - 1, newStat);
                return;
            }
            else if (stats[mid].value > newStat.value)
            {
                SortStat(stats, mid + 1, maxIndex, newStat);
                return;
            }

            PlaceNewElem(stats, newStat, mid);
            return;
        }

        if (stats[0].value < newStat.value)
            PlaceNewElem(stats, newStat, 0);

        return;
    }

    private void PlaceNewElem(GenomeIntPair[] stats, GenomeIntPair newStat, int index)
    {
        GenomeIntPair[] newStats = new GenomeIntPair[stats.Length];
        stats.CopyTo(newStats, 0);

        newStats[index] = newStat;
        for (int i = index + 1; i < stats.Length; i++)
            newStats[i] = stats[i - 1];

        newStats.CopyTo(stats, 0);
    }

    private int LookForType(Dictionary<Placeable.Type, int> items, Placeable.Type type)
    {
        if (items.ContainsKey(type))
            return items[type];
        return 0;
    }
}
