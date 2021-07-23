using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class NpcBehaviour : MonoBehaviour
{
    public VillageData myVillage;

    private GridSnapping grid;
    private MapController map;
    private NpcData npcData;
    public Text displayText;
    public GameObject statusDeathImage;
    private Vector2Int mapPosition = new Vector2Int(10,10);
    private Vector2Int homePosition = new Vector2Int(3, 3);
    [SerializeField]
    private bool returnHome;
    private string[,] localMapData;
    private Vector2Int[] directions = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

    public List<Vector2Int> knownWoodOres = new List<Vector2Int>();
    public List<Vector2Int> knownStoneOres = new List<Vector2Int>();
    public List<Vector2Int> knownGoldOres = new List<Vector2Int>();

    private void Awake()
    {
        localMapData = new string[SimulationSettings.simSettings.mapRows, SimulationSettings.simSettings.mapColumns];
 
        for (int i = 0; i < localMapData.GetLength(0); i++)
        {
            for (int j = 0; j < localMapData.GetLength(1); j++)
            {
                if (j == 0 || i ==0 || i == localMapData.GetLength(0) - 1 || j == localMapData.GetLength(1) - 1)
                {
                    localMapData[i, j] = "e";
                }
                else
                {
                    localMapData[i, j] = "u";
                }
            }
        }
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SimulationLogic.current.onTick += Tick;

        myVillage = gameObject.GetComponentInParent<VillageData>();
        homePosition = myVillage.arrayPosition;
        mapPosition = myVillage.arrayPosition;

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
        //Move(mapPosition.x, mapPosition.y);

    }

    public void Move(int x, int z) {
        mapPosition = new Vector2Int(x, z);
        moveOnWorldMap(x, z);
        TextFileController.WriteMapData(localMapData,"localMap");
    }
    
    public void Tick(int ticks){
        if (npcData.alive) {
            if (!returnHome){
                //explore
                for (int step = 0; step < npcData.moveLength; step++){
                    //find all possible possitions for each step
                    Vector2Int[] possiblePositions = new Vector2Int[8];
                    int[] possiblePositionsWeights = new int[8];
                    int counter = 0;

                    for (int i = -1; i <= 1; i++){
                        for (int j = -1; j <= 1; j++){
                            if (!(i == 0 && j == 0)){
                                if (localMapData[mapPosition.x + i, mapPosition.y + j] != "e") {
                                    localMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
                                    if (localMapData[mapPosition.x + i, mapPosition.y + j] == "W" && !knownWoodOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j))) {
                                        knownWoodOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                                    }
                                    else if (localMapData[mapPosition.x + i, mapPosition.y + j] == "S" && !knownStoneOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j))) {
                                        knownStoneOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                                    }
                                    else if (localMapData[mapPosition.x + i, mapPosition.y + j] == "G" && !knownGoldOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j))) {
                                        knownGoldOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                                    }
                                }
                                if (npcData.resources[npcData.carryType].Contains(localMapData[mapPosition.x + i, mapPosition.y + j])) {
                                    //Debug.Log("Resource: "+ npcData.resources[npcData.carryType] + " Found");

                                    map.PickUpResource(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                                    npcData.carryingResources.Add(localMapData[mapPosition.x + i, mapPosition.y + j]);
                                    localMapData[mapPosition.x + i, mapPosition.y + j] = "O";
                                    returnHome = true;
                                } 
                                possiblePositions[counter] = new Vector2Int(mapPosition.x + i, mapPosition.y + j);

                                possiblePositionsWeights[counter] = 1;
                                //calculate weight of each vertici 
                                if (localMapData[possiblePositions[counter].x, possiblePositions[counter].y] == "e")
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
                                                if (localMapData[possiblePositions[counter].x + ii, possiblePositions[counter].y + jj] == "u")
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
                                localMapData[mapPosition.x + i, mapPosition.y + j] = "x";
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
                        int up_counter = 0;
                        for (int i = mapPosition.x + 1; i < localMapData.GetLength(0)-1; i++) {
                            if (localMapData[i, mapPosition.y] == "u") {
                                foundUnexplored = true;
                                break;
                            }
                            up_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) up_counter = SimulationSettings.simSettings.mapColumns;
                        foundUnexplored = false;

                        int down_counter = 0;
                        for (int i = mapPosition.x - 1; i > 1; i--)
                        {
                            if (localMapData[i, mapPosition.y] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            down_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) down_counter = SimulationSettings.simSettings.mapColumns;
                        foundUnexplored = false;

                        int right_counter = 0;
                        for (int i = mapPosition.y + 1; i < localMapData.GetLength(1)-1; i++)
                        {
                            if (localMapData[mapPosition.x, i] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            right_counter++;
                        }
                        //if there isnt land that way maximize distance
                        if (!foundUnexplored) right_counter = SimulationSettings.simSettings.mapRows;
                        foundUnexplored = false;

                        int left_counter = 0;
                        for (int i = mapPosition.y - 1; i > 1; i--)
                        {
                            if (localMapData[mapPosition.x, i] == "u")
                            {
                                foundUnexplored = true;
                                break;
                            }
                            left_counter++;
                        }
                        if (!foundUnexplored) left_counter = SimulationSettings.simSettings.mapRows;
                      
                        foundUnexplored = false;
                        int[] directionLenghts =new int[]{ up_counter, down_counter, left_counter, right_counter };
                        Vector2Int newPosition = mapPosition + directions[indexOfMinNotZero(directionLenghts)];
                        if (directionLenghts[indexOfMinNotZero(directionLenghts)] != 100){
                            for (int points = 0; points < 5; points++) {
                                profitablePositions.Add(newPosition);
                            }
                        }
                    }
                    
                    Vector2Int nextPosition = profitablePositions[UnityEngine.Random.Range(0, profitablePositions.Count)];
                    
                    Move(nextPosition.x, nextPosition.y);
                }
            }
            else {
                if (homePosition == mapPosition)
                {
                    myVillage.AddResource(npcData.carryingResources);
                    returnHome = false;
                }
                else
                {
                    //return home
                    for (int step = 0; step < npcData.moveLength; step++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (!(i == 0 && j == 0))
                                {
                                    if (localMapData[mapPosition.x + i, mapPosition.y + j] != "e" && localMapData[mapPosition.x + i, mapPosition.y + j] != "V") localMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
                                }
                                else
                                {
                                    localMapData[mapPosition.x, mapPosition.y] = "x";
                                }
                            }
                        }

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
                        else if (mapPosition.x > homePosition.x)
                        {

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
                        else if (mapPosition.x == homePosition.x)
                        {
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

            }
          
            npcData.energy--;
            if (npcData.energy == 0)
            {
                npcData.alive = false;
                myVillage.NpcRemoved(gameObject);
                statusDeathImage.SetActive(true);
            }   
        }
        else {
            Destroy(gameObject);
        }
    }

    public void moveOnWorldMap(int x , int z) {
        transform.position = grid.GetNearestWorldPoint(transform.position, new Vector3Int(mapPosition.x, 0, mapPosition.y));
    }

    private int indexOfMinNotZero(int[] array) {
        int min = 100;
        int arrayIndex = 0;
        for (int i = 0; i < array.Length; i++) {
            if (min > array[i] && array[i] != 0) {
                min = array[i];
                arrayIndex = i;
            }
        }
        return arrayIndex;
    }

   
}
