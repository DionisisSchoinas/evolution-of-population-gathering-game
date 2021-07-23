using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageDataDisplay : MonoBehaviour
{
    public Text villageName;
    public Text npcCount;
    public Text goldCount;
    public Text stoneCount;
    public Text woodCount;
    public Text totalCount;

    public void UpdateCount(VillageData.Storage storage)
    {
        goldCount.text = "Gold : " + storage.GetItemCount(Placeable.Type.Gold);
        stoneCount.text = "Stone : " + storage.GetItemCount(Placeable.Type.Stone);
        woodCount.text = "Wood : " + storage.GetItemCount(Placeable.Type.Wood);
        totalCount.text = "Total : " + storage.totalItems + " / 0";
    }
}
