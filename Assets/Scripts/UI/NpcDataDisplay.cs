using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDataDisplay : MonoBehaviour
{
    public Text genome;
    public Text energyCount;
    public Text coinsCount;
    public Text potsCount;
    public Text goldCount;
    public Text stoneCount;
    public Text woodCount;

    public NpcData npcData;

    private Button showMapButton;
    private GridSnapping gridSnapping;

    private void Awake()
    {
        showMapButton = gameObject.GetComponentInChildren<Button>();
        showMapButton.onClick.AddListener(ShowMap);
        gridSnapping = FindObjectOfType<GridSnapping>();
    }

    public void UpdateView()
    {
        genome.text = npcData.genome;
        energyCount.text = "Energy : " + npcData.energy.ToString();
        coinsCount.text = "Coins : " + npcData.gold.ToString();
        potsCount.text = "Potions : " + npcData.energyPots;
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
        gridSnapping.ShowMapOverlay(npcData);
    }
}
