using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageDataDisplay : MonoBehaviour
{
    public Text villageName;
    public Image villageColor;
    public Text npcCount;
    public Text goldCount;
    public Text stoneCount;
    public Text woodCount;
    public Text totalCount;

    public VillageData villageData;

    private Button detailsButton;
    private SimulationVillageDisplay simulationVillageDisplay;

    private void Awake()
    {
        simulationVillageDisplay = FindObjectOfType<SimulationVillageDisplay>();
        detailsButton = gameObject.GetComponentInChildren<Button>();
        detailsButton.onClick.AddListener(ShowDetails);
    }

    public void UpdateCount()
    {
        goldCount.text = "Gold : " + villageData.storage.GetItemCount(Placeable.Type.Gold);
        stoneCount.text = "Stone : " + villageData.storage.GetItemCount(Placeable.Type.Stone);
        woodCount.text = "Wood : " + villageData.storage.GetItemCount(Placeable.Type.Wood);
        totalCount.text = "Total : " + villageData.storage.totalItems + " / 0";
    }

    private void ShowDetails()
    {
        simulationVillageDisplay.ShowNpcs(villageData);
    }
}
