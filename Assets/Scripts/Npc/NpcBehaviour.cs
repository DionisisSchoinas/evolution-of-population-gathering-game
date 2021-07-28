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
    [HideInInspector]
    public MapController map;
    public NpcData npcData;
    public Vector2Int mapPosition = new Vector2Int(10,10);
    private Vector2Int homePosition = new Vector2Int(3, 3);
    [SerializeField]
    private bool _returnHome;
    public string[,] localMapData;
    private Vector2Int[] directions = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
    private SimulationData simulationData;
    public List<Vector2Int> knownWoodOres = new List<Vector2Int>();
    public List<Vector2Int> knownStoneOres = new List<Vector2Int>();
    public List<Vector2Int> knownGoldOres = new List<Vector2Int>();

    public bool hasMate;

    private void Awake()
    {
        simulationData = FindObjectOfType<SimulationData>();
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

        hasMate = false;

        myVillage = gameObject.GetComponentInParent<VillageData>();
        homePosition = myVillage.arrayPosition;
        mapPosition = myVillage.arrayPosition;

        _returnHome = false;
        grid = FindObjectOfType<GridSnapping>();
        map = FindObjectOfType<MapController>();
        npcData = GetComponent<NpcData>();
    }

    private void OnDestroy()
    {
        SimulationLogic.current.onTick -= Tick;
        simulationData.agents.Remove(this);
        map.DropResources(mapPosition, npcData.carryingResources);
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Move(mapPosition.x, mapPosition.y);

        SimulationLogic.current.onTick += Tick;
    }

    public void Move(int x, int z) {
        mapPosition = new Vector2Int(x, z);
        moveOnWorldMap();
        //TextFileController.WriteMapData(localMapData,"localMap");
    }

    public void Tick(int ticks){
        simulationData.updateAgent(this);
        if (npcData.alive) {
            if (distance(mapPosition, homePosition) > npcData.energy && npcData.energyPots > 0) {
                consumeEnergyPot();
            }          
            else if (!_returnHome) {
                Vector2Int neiboringOre = findNeiboringOre();
                if (neiboringOre != mapPosition) {

                    map.PickUpResource(neiboringOre);
                    if (map.mapData[neiboringOre.x, neiboringOre.y] == MapController.MapBuildingToString(Placeable.Type.Energy))
                    {
                        npcData.energyPots += 1;
                        localMapData[neiboringOre.x, neiboringOre.y] = "O";
                    }
                    else
                    {
                        if (npcData.totalItems < npcData.maxCaringCapacity)
                        {
                            npcData.AddResource(localMapData[neiboringOre.x, neiboringOre.y]);
                            localMapData[neiboringOre.x, neiboringOre.y] = "O";

                            //remove from known ores
                            knownGoldOres.Remove(neiboringOre);
                            knownWoodOres.Remove(neiboringOre);
                            knownStoneOres.Remove(neiboringOre);
                        }
                        else
                        {
                            _returnHome = true;
                        }
                    }
                }
                else if (returnClosestOre() != new Vector2Int(1200, 1200) && distance(mapPosition, returnClosestOre()) > 1)
                {
                    lookForOre();
                }
                else
                {
                    explore();
                }
            }
            else {
                if (homePosition == mapPosition)
                {
                    myVillage.AddResource(npcData.carryingResources);
                    npcData.ClearInventory();
                    _returnHome = false;
                }
                else
                {
                    returnHome();
                }

            }
          
            npcData.energy--;
            npcData.totalLife++;

            if (npcData.energy == 0)
            {
                npcData.alive = false;
            }   
        }
        else
        {
            destroyAgent();
        }
    }
    public void moveOnWorldMap() {
        transform.position = grid.GetNearestWorldPoint(transform.position, new Vector3Int(mapPosition.x, 0, mapPosition.y));
    }
    private int indexOfMinNotZero(int[] array) {
        int min = 600;
        int arrayIndex = 0;
        for (int i = 0; i < array.Length; i++) {
            if (min > array[i] && array[i] != 0) {
                min = array[i];
                arrayIndex = i;
            }
        }
        return arrayIndex;
    }
    private Vector2Int returnClosestOre() {
        Vector2Int closestOreCoords = new Vector2Int(1200, 1200);
        
        switch (npcData.carryType){
            case 0:
                foreach (Vector2Int coord in knownWoodOres) {
                    if (distance(mapPosition, coord) < distance(mapPosition, closestOreCoords)) {
                        closestOreCoords = coord;
                    }
                }
                break;
            case 1:
                foreach (Vector2Int coord in knownStoneOres)
                {
                    if (distance(mapPosition, coord) < distance(mapPosition, closestOreCoords))
                    {
                        closestOreCoords = coord;
                    }
                }
                break;
            case 2:
                foreach (Vector2Int coord in knownGoldOres)
                {
                    if (distance(mapPosition, coord) < distance(mapPosition, closestOreCoords))
                    {
                        closestOreCoords = coord;
                    }
                }
                break;
            case 3:
                break;

        }
        return closestOreCoords;
    }
    private int distance(Vector2Int coord1,Vector2Int coord2) { 
        return Math.Abs(coord1.x- coord2.x) + Math.Abs(coord1.y - coord2.y);
    }
    private void explore(){
        //explore
        for (int step = 0; step < npcData.moveLength; step++)
        {
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
                        if (localMapData[mapPosition.x + i, mapPosition.y + j] != "e")
                        {
                            localMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
                            if (localMapData[mapPosition.x + i, mapPosition.y + j] == MapController.MapBuildingToString(Placeable.Type.Wood) && !knownWoodOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j)))
                            {
                                knownWoodOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                            }
                            else if (localMapData[mapPosition.x + i, mapPosition.y + j] == MapController.MapBuildingToString(Placeable.Type.Stone) && !knownStoneOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j)))
                            {
                                knownStoneOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                            }
                            else if (localMapData[mapPosition.x + i, mapPosition.y + j] == MapController.MapBuildingToString(Placeable.Type.Gold) && !knownGoldOres.Contains(new Vector2Int(mapPosition.x + i, mapPosition.y + j)))
                            {
                                knownGoldOres.Add(new Vector2Int(mapPosition.x + i, mapPosition.y + j));
                            }
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
                                        try
                                        {
                                            if (localMapData[possiblePositions[counter].x + ii, possiblePositions[counter].y + jj] == "u")
                                            {
                                                possiblePositionsWeights[counter] += 1;
                                            }
                                        }
                                        catch
                                        {
                                            // Tested a few times, never crashed, left this here just in case
                                            Debug.Log(possiblePositions[counter]);
                                            Debug.Log("Crashed here : " + (possiblePositions[counter].x + ii) + ", " + (possiblePositions[counter].y + jj));
                                        }
                                    }
                                }
                            }
                        }
                        counter++;
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
            if (pointTotal <= 8)
            {
                int up_counter = 0;
                for (int i = mapPosition.x + 1; i < localMapData.GetLength(0) - 1; i++)
                {
                    if (localMapData[i, mapPosition.y] == "u")
                    {
                        foundUnexplored = true;
                        break;
                    }
                    up_counter++;
                }
                //if there isnt land that way maximize distance
                if (!foundUnexplored) up_counter = 600;
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
                if (!foundUnexplored) down_counter = 600;
                foundUnexplored = false;

                int right_counter = 0;
                for (int i = mapPosition.y + 1; i < localMapData.GetLength(1) - 1; i++)
                {
                    if (localMapData[mapPosition.x, i] == "u")
                    {
                        foundUnexplored = true;
                        break;
                    }
                    right_counter++;
                }
                //if there isnt land that way maximize distance
                if (!foundUnexplored) right_counter = 600;
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
                int[] directionLenghts = new int[] { up_counter, down_counter, left_counter, right_counter };
                Vector2Int newPosition = mapPosition + directions[indexOfMinNotZero(directionLenghts)];
                if (directionLenghts[indexOfMinNotZero(directionLenghts)] != 600)
                {
                    for (int points = 0; points < 5; points++)
                    {
                        profitablePositions.Add(newPosition);
                    }
                }
            }

            Vector2Int nextPosition = profitablePositions[UnityEngine.Random.Range(0, profitablePositions.Count)];

            Move(nextPosition.x, nextPosition.y);
        }
    }
    private void lookForOre() {
        for (int step = 0; step < npcData.moveLength; step++)
        {
            Vector2Int closestOre = returnClosestOre();
            //if the agent is close to ore remove from known ores and pick it up
            if (distance(mapPosition, closestOre) == 1)
            {
                if (map.mapData[closestOre.x, closestOre.y] == MapController.MapBuildingToString(Placeable.Type.Ground)) { 
                    //remove from known ores
                    knownGoldOres.Remove(closestOre);
                    knownWoodOres.Remove(closestOre);
                    knownStoneOres.Remove(closestOre);
                }
                break;
            }
            //if you agent is far to ore move him towords the general direction
            else
            {
                Vector2Int oreDirection = new Vector2Int(mapPosition.x, mapPosition.y);

                if (mapPosition.x < closestOre.x)
                {
                    oreDirection += new Vector2Int(1, 0);
                }
                else if (mapPosition.x > closestOre.x)
                {
                    oreDirection += new Vector2Int(-1, 0);
                }
                else if (mapPosition.y < closestOre.y)
                {
                    oreDirection += new Vector2Int(0, 1);
                }
                else if (mapPosition.y > closestOre.y)
                {
                    oreDirection += new Vector2Int(0, -1);
                }

                Move(oreDirection.x, oreDirection.y);
            }
        }
    }
    private void returnHome() {
        //return home
        for (int step = 0; step < npcData.moveLength; step++)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        if (localMapData[mapPosition.x + i, mapPosition.y + j] != "e") 
                            localMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];
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
    private Vector2Int findNeiboringOre()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // update map with new data while searching
                if (localMapData[mapPosition.x + i, mapPosition.y + j] != "e")
                    localMapData[mapPosition.x + i, mapPosition.y + j] = map.mapData[mapPosition.x + i, mapPosition.y + j];

                if (!(i == 0 && j == 0))
                {
                    if (npcData.resources[npcData.carryType].Contains(localMapData[mapPosition.x + i, mapPosition.y + j]) || localMapData[mapPosition.x + i, mapPosition.y + j] == MapController.MapBuildingToString(Placeable.Type.Energy))
                    {
                        //Debug.Log(map.mapData[mapPosition.x + i, mapPosition.y + j]);
                        return new Vector2Int(mapPosition.x + i, mapPosition.y + j);
                    }
                }
            }
        }
        return new Vector2Int(mapPosition.x, mapPosition.y);
    }
    public string[,] GetMap()
    {
        return localMapData;
    }
    public void consumeEnergyPot() 
    {
        if (npcData.energyPots > 0) { 
        npcData.energyPots -= 1;
        npcData.energy += 20;
        }
    }   
    public void destroyAgent() 
    {
        myVillage.NpcRemoved(gameObject);
        if (npcData != null && npcData.dataDisplay != null)
            Destroy(npcData.dataDisplay.gameObject);
        Destroy(gameObject);
    }
    public string getGenome() 
    {
        return npcData.genome;
    }
   
}
