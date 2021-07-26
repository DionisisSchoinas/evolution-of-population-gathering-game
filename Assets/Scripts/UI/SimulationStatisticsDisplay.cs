using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SimulationStatisticsDisplay : MonoBehaviour
{
    public Button closeButton;

    private CanvasGroup canvasGroup;
    private Dictionary<string, SimulationStatistics.Statistics> statistics;
    public SimulationStatistics.Statistics bestStats;
    public GenomeValueDisplay[] longestLife;
    public GenomeValueDisplay[] bestGoldCarriers;
    public GenomeValueDisplay[] bestStoneCarriers;
    public GenomeValueDisplay[] bestWoodCarriers;

    public List<SimulationStatistics.GenomeIntPair> g;

    private void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        UIManager.SetCanvasState(canvasGroup, false);

        statistics = new Dictionary<string, SimulationStatistics.Statistics>();
        bestStats = new SimulationStatistics.Statistics();

        closeButton.onClick.AddListener(
            delegate
            {
                UIManager.SetCanvasState(canvasGroup, false);
            }
        );

        GenomeValueDisplay[] displays = gameObject.GetComponentsInChildren<GenomeValueDisplay>();

        longestLife = new GenomeValueDisplay[5];
        for (int i = 0; i < 5; i++)
            longestLife[i] = displays[i];

        bestGoldCarriers = new GenomeValueDisplay[5];
        for (int i = 0; i < 5; i++)
            bestGoldCarriers[i] = displays[i + 5];

        bestStoneCarriers = new GenomeValueDisplay[5];
        for (int i = 0; i < 5; i++)
            bestStoneCarriers[i] = displays[i + 10];

        bestWoodCarriers = new GenomeValueDisplay[5];
        for (int i = 0; i < 5; i++)
            bestWoodCarriers[i] = displays[i + 15];
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
        UIManager.SetCanvasState(canvasGroup, false);
    }

    public void LoadStatistics()
    {
        try
        {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath + "/Simulation Statistics");
            FileInfo[] filesInfo = info.GetFiles();

            ImportData(filesInfo);

            FindTopScores();

            DisplayData();

            Debug.Log("Loaded from : " + Application.persistentDataPath + "/Simulation Statistics");
        }
        catch
        {
            Debug.Log("Couldn't find : " + Application.persistentDataPath + "/Simulation Statistics");
        }
    }

    private void ImportData(FileInfo[] filesInfo)
    {
        string data;
        SimulationStatistics.Statistics stats;
        for (int i = 0; i < filesInfo.Length; i++)
        {
            if (statistics.ContainsKey(filesInfo[i].Name))
                continue;

            data = File.ReadAllText(filesInfo[i].FullName);
            stats = JsonUtility.FromJson<SimulationStatistics.Statistics>(data);

            if (stats == null)
                continue;

            statistics.Add(filesInfo[i].Name, stats);
        }
    }

    private void FindTopScores()
    {
        List<SimulationStatistics.GenomeIntPair> pairsLife = new List<SimulationStatistics.GenomeIntPair>();
        List<SimulationStatistics.GenomeIntPair> pairsGold = new List<SimulationStatistics.GenomeIntPair>();
        List<SimulationStatistics.GenomeIntPair> pairsStone = new List<SimulationStatistics.GenomeIntPair>();
        List<SimulationStatistics.GenomeIntPair> pairsWood = new List<SimulationStatistics.GenomeIntPair>();

        foreach (SimulationStatistics.Statistics stats in statistics.Values)
        {
            pairsLife.AddRange(stats.longestLife);
            pairsGold.AddRange(stats.bestGoldCarriers);
            pairsStone.AddRange(stats.bestStoneCarriers);
            pairsWood.AddRange(stats.bestWoodCarriers);
        }

        pairsLife.Sort((x, y) => x.value.CompareTo(y.value));
        pairsGold.Sort((x, y) => x.value.CompareTo(y.value));
        pairsStone.Sort((x, y) => x.value.CompareTo(y.value));
        pairsWood.Sort((x, y) => x.value.CompareTo(y.value));

        // Longest lifespan
        int indexTop = Mathf.Max(4, pairsLife.Count - 1);
        int j = 0;
        for (int i = indexTop; i >= indexTop - 4 ; i--)
            bestStats.longestLife[j++] = pairsLife[i];

        // Best gold carriers
        indexTop = Mathf.Max(4, pairsGold.Count - 1);
        j = 0;
        for (int i = indexTop; i >= indexTop - 4; i--)
            bestStats.bestGoldCarriers[j++] = pairsGold[i];

        // Best stone carriers
        indexTop = Mathf.Max(4, pairsStone.Count - 1);
        j = 0;
        for (int i = indexTop; i >= indexTop - 4; i--)
            bestStats.bestStoneCarriers[j++] = pairsStone[i];

        // Best wood carriers
        indexTop = Mathf.Max(4, pairsWood.Count - 1);
        j = 0;
        for (int i = indexTop; i >= indexTop - 4; i--)
            bestStats.bestWoodCarriers[j++] = pairsWood[i];
    }

    private void DisplayData()
    {
        for (int i = 0; i < 5; i++)
        {
            longestLife[i].gameObject.SetActive(true);
            bestGoldCarriers[i].gameObject.SetActive(true);
            bestStoneCarriers[i].gameObject.SetActive(true);
            bestWoodCarriers[i].gameObject.SetActive(true);

            longestLife[i].SetValue(bestStats.longestLife[i]);
            bestGoldCarriers[i].SetValue(bestStats.bestGoldCarriers[i]);
            bestStoneCarriers[i].SetValue(bestStats.bestStoneCarriers[i]);
            bestWoodCarriers[i].SetValue(bestStats.bestWoodCarriers[i]);
        }
    }
}
