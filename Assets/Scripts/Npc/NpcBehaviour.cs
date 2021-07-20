using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class NpcBehaviour : WorldObject
{
    private GridSnapping grid;
    private MapController map;
    NpcData npcData;
    public Text displayText;
    public GameObject statusDeathImage;
    private Vector2Int mapPosition = new Vector2Int(10,10);
    
    private string[,] localmMapData;
    private float[,] weightMap;

    private void Awake()
    {
        localmMapData = new string[100, 100];
        weightMap= new float[100, 100];

        for (int i = 0; i < localmMapData.GetLength(0); i++)
        {
            for (int j = 0; j < localmMapData.GetLength(1); j++)
            {
                if (j == 0 || i ==0 || i == localmMapData.GetLength(0) - 1 || j == localmMapData.GetLength(1) - 1)
                {
                    localmMapData[i, j] = "e";
                    weightMap[i, j] = -1f;
                }
                else
                {
                    localmMapData[i, j] = "u";
                    weightMap[i, j] = 1f;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<GridSnapping>();
        map= FindObjectOfType<MapController>();
        npcData = GetComponent<NpcData>();
        Debug.Log(npcData.genome.Length);
        if (npcData.genome.Length == 11) {
            //First gene 
            Debug.Log(npcData.genome.Substring(0, 1));
            if (npcData.genome.Substring(0, 1) == "0") {
                displayText.text = "Μετακίνηση : 1 θέση \n";
                npcData.moveLength = 1;
            }
            else if (npcData.genome.Substring(0, 1) == "1")
            {
                displayText.text = "Μετακίνηση : 2 θέσεις\n";
                npcData.moveLength = 2;
            }
            //Second gene 
            if (npcData.genome.Substring(1, 2) == "00")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά ξυλείας\n";
                npcData.carryType = 0;
            }
            else if (npcData.genome.Substring(1, 2) == "01")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά πέτρας\n";
                npcData.carryType = 1;
            }
            else if (npcData.genome.Substring(1, 2) == "10")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά χρυσού\n";
                npcData.carryType = 2;
            }
            else if (npcData.genome.Substring(1, 2) == "11")
            {
                displayText.text += "Mεταφέρει όλους τους πόρους\n";
                npcData.carryType = 3;
            }
            //Third gene 
            if (npcData.genome.Substring(3, 2) == "00")
            {
                displayText.text += "Mια μονάδα πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 1;
            }
            else if (npcData.genome.Substring(3, 2) == "01")
            {
                displayText.text += "Δυο μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 2;
            }
            else if (npcData.genome.Substring(3, 2) == "10")
            {
                displayText.text += "Τρεις μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 3;
            }
            else if (npcData.genome.Substring(3, 2) == "11")
            {
                displayText.text += "Τέσσερις μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 4;
            }
            //Forth gene 
            if (npcData.genome.Substring(5, 2) == "00")
            {
                displayText.text += "Ξεκινά με 10 Gold\n";
                npcData.gold = 10;
            }
            else if (npcData.genome.Substring(5, 2) == "01")
            {
                displayText.text += "Ξεκινά με 20Gold\n";
                npcData.gold = 20;
            }
            else if (npcData.genome.Substring(5, 2) == "10")
            {
                displayText.text += "Ξεκινά με 40Gold\n";
                npcData.gold = 40;
            }
            else if (npcData.genome.Substring(5, 2) == "11")
            {
                displayText.text += "Ξεκινά με 80Gold\n";
                npcData.gold =80;
            }
            //Fifth gene 
            if (npcData.genome.Substring(7, 2) == "00")
            {
                displayText.text += "Ξεκινά με 1 Energy pot\n";
                npcData.energyPots = 1;
            }
            else if (npcData.genome.Substring(7, 2) == "01")
            {
                displayText.text += "Ξεκινά με 2 Energy pot\n";
                npcData.energyPots = 2;
            }
            else if (npcData.genome.Substring(7, 2) == "10")
            {
                displayText.text += "Ξεκινά με 3 Energy pot\n";
                npcData.energyPots = 3;
            }
            else if (npcData.genome.Substring(7, 2) == "11")
            {
                displayText.text += "Ξεκινά με 4 Energy pot\n";
                npcData.energyPots = 4;
            }
            //Sixth gene 
            if (npcData.genome.Substring(9, 2) == "00")
            {
                displayText.text += "Ξεκινά με 50 Energy points\n";
                npcData.energy = 50;
            }
            else if (npcData.genome.Substring(9, 2) == "01")
            {
                displayText.text += "Ξεκινά με 100 Energy points\n";
                npcData.energy = 100;
            }
            else if (npcData.genome.Substring(9, 2) == "10")
            {
                displayText.text += "Ξεκινά με 200 Energy points\n";
                npcData.energy = 200;
            }
            else if (npcData.genome.Substring(9, 2) == "11")
            {
                displayText.text += "Ξεκινά με 400 Energy points\n";
                npcData.energy = 400;
            }
        }
        Move(mapPosition.x, mapPosition.y);

    }

    public void Move(int x, int z) {
        Debug.Log(new Vector3Int(x, 0, z) + " -> " + grid.GetNearestGridPoint(new Vector3(x, 0, z)));
        //transform.position = grid.GetNearestGridPoint(new Vector3(x,0,z));



        weightMap[x, z] = 0.5f;
        mapPosition = new Vector2Int(x, z);
        moveOnWorldMap(x, z);
        localmMapData[x,z] = map.mapData[x,z];
        TextFileController.WriteMapData(localmMapData,"localMap");

        string[,] weightMapTemp = new string[100, 100];
        for (int i = 0; i < weightMap.GetLength(0); i++){
            for (int j = 0; j < weightMap.GetLength(1); j++) {
                weightMapTemp[i, j] = weightMap[i, j]+"";
            }
        }
        TextFileController.WriteMapData(weightMapTemp, "weightMap");

    }

    public override void Tick(){
        if (npcData.alive) {
            for (int step = 0; step < npcData.moveLength; step++ ){
                //find all possible possitions for each step
                Vector2Int[] possiblePositions = new Vector2Int[8];
                int counter = 0;
                float maxWeight=0;
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++){
                        if (!(i == 0 && j == 0))
                        {
                            localmMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
                            possiblePositions[counter] = new Vector2Int(mapPosition.x + i, mapPosition.y + j);
                            if (maxWeight < weightMap[mapPosition.x + i, mapPosition.y + j]) maxWeight = weightMap[mapPosition.x + i, mapPosition.y + j];
                            counter++;
                        }
                        else {
                            localmMapData[mapPosition.x + i, mapPosition.y + j] = "x";
                        }
                    }
                }
                List<Vector2Int> profitablePositions = new List<Vector2Int>();
                //find the most profitable possitions in the possible possitions
                for (int position = 0; position < possiblePositions.Length; position++) {
                    if (weightMap[possiblePositions[position].x, possiblePositions[position].y]==maxWeight){
                        profitablePositions.Add(possiblePositions[position]);
                    }
                }
                Vector2Int nextPosition = profitablePositions[UnityEngine.Random.Range(0, profitablePositions.Count)];
                Move(nextPosition.x,nextPosition.y);

                //Move((int)mapPosition.x + UnityEngine.Random.Range(-1, 2), (int)mapPosition.y + UnityEngine.Random.Range(-1, 2));

            }

            npcData.energy--;
            if (npcData.energy == 0)
            {
                npcData.alive = false;
                statusDeathImage.SetActive(true);
            }
        }
        else {
            Destroy(gameObject);
        }
    }
    public void moveOnWorldMap(int x , int z) {
        transform.position = grid.GetNearestWorldPoint(transform.position, new Vector3Int(mapPosition.x,0,mapPosition.y));
    }

}
