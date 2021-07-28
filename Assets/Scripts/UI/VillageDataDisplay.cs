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
        if (detailsButton != null)
            detailsButton.onClick.AddListener(ShowDetails);
    }

    public void UpdateCount()
    {
        goldCount.text = "Gold : " + villageData.storage.GetItemCount(Placeable.Type.Gold) + " / " + villageData.reqResources[Placeable.Type.Gold];
        stoneCount.text = "Stone : " + villageData.storage.GetItemCount(Placeable.Type.Stone) + " / " + villageData.reqResources[Placeable.Type.Stone];
        woodCount.text = "Wood : " + villageData.storage.GetItemCount(Placeable.Type.Wood) + " / " + villageData.reqResources[Placeable.Type.Wood];
        totalCount.text = "Total : " + villageData.storage.totalItems + " / " + villageData.reqTotal;
    }


    public void ShowStats()
    {
        goldCount.gameObject.SetActive(true);
        stoneCount.gameObject.SetActive(true);
        woodCount.gameObject.SetActive(true);
        totalCount.gameObject.SetActive(true);
    }

    public void HideStats()
    {
        goldCount.gameObject.SetActive(false);
        stoneCount.gameObject.SetActive(false);
        woodCount.gameObject.SetActive(false);
        totalCount.gameObject.SetActive(false);
    }

    private void ShowDetails()
    {
        simulationVillageDisplay.ShowNpcs(villageData);
    }
}
