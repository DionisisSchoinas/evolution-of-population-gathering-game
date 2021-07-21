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
    private Vector2Int homePosition = new Vector2Int(1, 1);
    [SerializeField]
    private bool returnHome;
    private string[,] localmMapData;
    private Vector2Int[] directions = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

    private void Awake()
    {
        localmMapData = new string[100, 100];
 

        for (int i = 0; i < localmMapData.GetLength(0); i++)
        {
            for (int j = 0; j < localmMapData.GetLength(1); j++)
            {
                if (j == 0 || i ==0 || i == localmMapData.GetLength(0) - 1 || j == localmMapData.GetLength(1) - 1)
                {
                    localmMapData[i, j] = "e";
                   
                }
                else
                {
                    localmMapData[i, j] = "u";
                    
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        returnHome = false;
        grid = FindObjectOfType<GridSnapping>();
        map= FindObjectOfType<MapController>();
        npcData = GetComponent<NpcData>();
    
        if (npcData.genome.Length == 11) {
            //First gene 
         
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
        mapPosition = new Vector2Int(x, z);
        moveOnWorldMap(x, z);
        localmMapData[x,z] = map.mapData[x,z];
        TextFileController.WriteMapData(localmMapData,"localMap");

    }

    public override void Tick(){
        if (npcData.alive) {
            if (!returnHome){
                //explore
                for (int step = 0; step < npcData.moveLength; step++){
                    //find all possible possitions for each step
                    Vector2Int[] possiblePositions = new Vector2Int[8];
                    int[] possiblePositionsWeights = new int[8];

                    int counter = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (!(i == 0 && j == 0))
                            {
                                if(localmMapData[mapPosition.x + i, mapPosition.y + j]!="e") localmMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
                                possiblePositions[counter] = new Vector2Int(mapPosition.x + i, mapPosition.y + j);

                                possiblePositionsWeights[counter] = 1;
                                //calculate weight of each vertici 
                                if (localmMapData[possiblePositions[counter].x, possiblePositions[counter].y] == "e")
                                {
                                    possiblePositionsWeights[counter] = 0;
                                }
                                else
                                {
                                    for (int ii = -1; ii <= 1; ii++)
                                    {
                                        for (int jj = -1; jj <= 1; jj++)
                                        {
                                            if (!(ii == 0 && jj == 0))
                                            {
                                                if (localmMapData[possiblePositions[counter].x + ii, possiblePositions[counter].y + jj] == "u")
                                                {
                                                    possiblePositionsWeights[counter] += 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                counter++;
                            }
                            else{
                                localmMapData[mapPosition.x + i, mapPosition.y + j] = "x";
                            }
                        }
                    }
                    List<Vector2Int> profitablePositions = new List<Vector2Int>();

                    //find the most profitable possitions in the possible possitions

                    //Add positions to list acording to acumulated points bassed on likelihood of discovery

                    int pointTotal = 0;
                    for (int position = 0; position < possiblePositions.Length; position++)
                    {
                        for (int positionPointsIndex = 0; positionPointsIndex < possiblePositionsWeights[position]; positionPointsIndex++)
                        {
                            profitablePositions.Add(possiblePositions[position]);
                            pointTotal++;
                        }
                    }
                    //if npc is lost
                    bool foundUnexplored = false;
                    if (pointTotal <= 8) {
                        Debug.Log("Lost");                                                                                                                                                                                                                              
                        int up_counter = 0;
                        for (int i = mapPosition.x + 1; i < localmMapData.GetLength(0)-1; i++) {
                            if (localmMapData[i, mapPosition.y] == "u") {
                                foundUnexplored = true;
                                break;
                            }
                            up_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) up_counter = 100;
                        foundUnexplored = false;

                        int down_counter = 0;
                        for (int i = mapPosition.x - 1; i > 1; i--)
                        {
                            if (localmMapData[i, mapPosition.y] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            down_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) down_counter = 100;
                        foundUnexplored = false;

                        int right_counter = 0;
                        for (int i = mapPosition.y + 1; i < localmMapData.GetLength(1)-1; i++)
                        {
                            if (localmMapData[mapPosition.x, i] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            right_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) right_counter = 100;
                        foundUnexplored = false;

                        int left_counter = 0;
                        for (int i = mapPosition.y - 1; i > 1; i--)
                        {
                            if (localmMapData[mapPosition.x, i] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            left_counter++;
                        }
                        if (!foundUnexplored) left_counter = 100;
                        foundUnexplored = false;


                        int[] directionLenghts =new int[]{ up_counter, down_counter, left_counter, right_counter };
                        Vector2Int newPosition = mapPosition + directions[indexOfMinNotZero(directionLenghts)];
                        for (int points = 0; points < 5; points++) {
                            profitablePositions.Add(newPosition);
                        }
                        Debug.Log(directions[indexOfMinNotZero(directionLenghts)]);
                    }
                    
                    Vector2Int nextPosition = profitablePositions[UnityEngine.Random.Range(0, profitablePositions.Count)];
                    
                    Move(nextPosition.x, nextPosition.y);
                }
            }
            else {
                //return home
                for (int step = 0; step < npcData.moveLength; step++) {
                    if (mapPosition.x < homePosition.x)
                    {
                        if (mapPosition.y == homePosition.y)
                        {
                            Move(mapPosition.x + 1, mapPosition.y);
                        }
                        else if (mapPosition.y < homePosition.y)
                        {
                            Move(mapPosition.x + 1, mapPosition.y + 1);
                        }
                        else if (mapPosition.y > homePosition.y)
                        {
                            Move(mapPosition.x + 1, mapPosition.y - 1);
                        }
                    }
                    else if (mapPosition.x > homePosition.x){

                        if (mapPosition.y == homePosition.y)
                        {
                            Move(mapPosition.x - 1, mapPosition.y);
                        }
                        else if (mapPosition.y < homePosition.y)
                        {
                            Move(mapPosition.x - 1, mapPosition.y + 1);
                        }
                        else if (mapPosition.y > homePosition.y)
                        {
                            Move(mapPosition.x - 1, mapPosition.y - 1);
                        }
                    }
                    else if (mapPosition.x == homePosition.x) {
                     
                        if (mapPosition.y == homePosition.y)
                        {
                            Move(mapPosition.x, mapPosition.y);
                        }
                        else if (mapPosition.y < homePosition.y)
                        {
                            Move(mapPosition.x, mapPosition.y + 1);
                        }
                        else if (mapPosition.y > homePosition.y)
                        {
                            Move(mapPosition.x, mapPosition.y - 1);
                        }
                    }
                }
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

    private int indexOfMinNotZero(int[] array) {
        int min = 100;
        int arrayIndex = 0;
        for (int i = 0; i < array.Length; i++) {
            if (min > array[i] && array[i]!=0) {
                min = array[i];
                arrayIndex = i;

            }
        }
        Debug.Log(array[arrayIndex]);
        return arrayIndex;
    }
}
