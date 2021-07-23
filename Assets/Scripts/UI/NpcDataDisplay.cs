using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDataDisplay : MonoBehaviour
{
    public Text genome;
    public Text energyCount;
    public Text coinsCount;
    public Text goldCount;
    public Text stoneCount;
    public Text woodCount;

    public NpcData npcData;

    private Button showMapButton;
    private SimulationVillageDisplay simulationVillageDisplay;

    private void Awake()
    {
        simulationVillageDisplay = FindObjectOfType<SimulationVillageDisplay>();
        showMapButton = gameObject.GetComponentInChildren<Button>();
        showMapButton.onClick.AddListener(ShowMap);
    }

    public void UpdateView()
    {
        genome.text = npcData.genome;
        energyCount.text = "Energy : " + npcData.energy.ToString() + " / 0";
        coinsCount.text = "Coins : " + npcData.gold.ToString() + " / 0";
        goldCount.text = "Gold : " + GetItem(Placeable.Type.Gold);
        stoneCount.text = "Stone : " + GetItem(Placeable.Type.Stone);
        woodCount.text = "Wood : " + GetItem(Placeable.Type.Wood);
    }

    private int GetItem(Placeable.Type type)
    {
        int count = 0;
        if (npcData.carryingResources.TryGetValue(type, out count))
            return count;
        return 0;
    }

    private void ShowMap()
    {

    }
}
