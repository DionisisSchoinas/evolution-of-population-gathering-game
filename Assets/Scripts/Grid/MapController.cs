using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class MapController : MonoBehaviour
{
    public string[,] mapData;

    private static int _villages;
    public static int villages
    {
        get
        {
            return _villages;
        }
        set
        {
            _villages = value;
        }
    }

    private GridSnapping gridSnapping;
    private SimulationData simulationData;
    private MapRandomizer mapRandomizer;

    private void Awake()
    {
        gridSnapping = gameObject.GetComponent<GridSnapping>();
        simulationData = FindObjectOfType<SimulationData>();
        mapRandomizer = gameObject.GetComponent<MapRandomizer>();
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
        if (!running)
        {
            RedrawMap(true);
        }
    }

    public bool hasSpace(Vector3Int index, Placeable placeable)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f); // Always 0
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                if (!MapBuildingToEnum(mapData[i, j]).Equals(Placeable.Type.Ground))
                    return false;
            }
        }
        return true;
    }

    public Vector2Int AddBuilding(Vector3Int index, Placeable placeable)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f); // Always 0
        string val = "";
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                val = MapBuildingToString(placeable.type);
                // Check if village is being placed
                if (int.TryParse(val, out _))
                {
                    val = (++villages).ToString();
                }
                mapData[i, j] = val;
            }
        }
        TextFileController.WriteMapData(mapData);
        return new Vector2Int(index.x, index.z);
    }

    public void AddBuilding(Vector2Int index, Placeable placeable, bool writeOut)
    {
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f); // Always 0
        string val = "";
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.y - blockSize); j <= (index.y + blockSize); j++)
            {
                val = MapBuildingToString(placeable.type);
                // Check if village is being placed
                if (int.TryParse(val, out _))
                {
                    val = (++villages).ToString();
                }
                mapData[i, j] = val;
            }
        }

        gridSnapping.PlaceBuildingFromMapData(index, placeable);

        if (writeOut)
            TextFileController.WriteMapData(mapData);
    }

    public Vector2Int DeleteBuilding(Vector3Int index, Placeable placeable)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f); // Always 0
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                // Check if village is being deleted
                if (int.TryParse(mapData[i, j], out _))
                {
                    villages--;
                }
                mapData[i, j] = MapBuildingToString(Placeable.Type.Ground);
            }
        }
        TextFileController.WriteMapData(mapData);

        return new Vector2Int(index.x, index.z);
    }

    public void RenumberVillage(Vector3Int index, VillageData villageData)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(FindPlaceableFromType(Placeable.Type.Village).gridSpace / 2f); // Always 0
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                mapData[i, j] = villageData.number.ToString();
            }
        }
        TextFileController.WriteMapData(mapData);
    }

    private void PlaceEntireMap()
    {
        Placeable.Type type;
        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                // Find building type
                type = MapBuildingToEnum(mapData[i, j]);
                if (type == Placeable.Type.Ground)
                    continue;

                if (type == Placeable.Type.Village)
                    villages++;

                // Create a building based on the string value on the map data
                gridSnapping.PlaceBuildingFromMapData(new Vector2Int(i,j), FindPlaceableFromType(type));
            }
        }
    }

    public void RedrawMap(bool drawOldMap)
    {
        // Reset normilization vectors
        int i = Mathf.FloorToInt(SimulationSettings.simSettings.mapRows / 2f - 1);
        int j = Mathf.FloorToInt(SimulationSettings.simSettings.mapColumns / 2f - 1);
        SimulationSettings.indexNormalizeVector = new Vector3Int(i, 0, j);
        SimulationSettings.gridRrowsIndexLimits = new Vector2Int(-i, Mathf.CeilToInt(SimulationSettings.simSettings.mapRows / 2f));
        SimulationSettings.gridColumnsIndexLimits = new Vector2Int(-j, Mathf.CeilToInt(SimulationSettings.simSettings.mapColumns / 2f));

        // Redraw grid
        gridSnapping.RedrawGrid();

        // Get map data if dimensions match
        mapData = TextFileController.ReadMapData();

        // If dims don't match mapData will be null and it will do a full reset
        ClearMap();

        // Place old map
        if (drawOldMap)
            PlaceEntireMap();
        else
            mapRandomizer.RandomizeMap();
    }

    public void ClearMap(bool resetAll)
    {
        if (resetAll)
            mapData = null;
        ClearMap();
    }

    private void ClearMap()
    {
        if (mapData == null)
        {
            mapData = new string[SimulationSettings.simSettings.mapRows, SimulationSettings.simSettings.mapColumns];
            for (int i = 0; i < mapData.GetLength(0); i++)
            {
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    mapData[i, j] = MapBuildingToString(Placeable.Type.Ground);
                }
            }
        }

        villages = 0;
        simulationData.ClearVillages();
        TextFileController.WriteMapData(mapData);
        Placeable[] children = gameObject.GetComponentsInChildren<Placeable>();
        foreach (Placeable child in children)
            Destroy(child.gameObject);
    }

    public void PickUpResource(Vector2Int arrayPosition)
    {
        try
        {
            // Remove from map data
            mapData[arrayPosition.x, arrayPosition.y] = MapBuildingToString(Placeable.Type.Ground);
            // Remove from map display
            Destroy(gridSnapping.mapDataTransforms[arrayPosition.x, arrayPosition.y].gameObject);
            // Remove from map display data
            gridSnapping.mapDataTransforms[arrayPosition.x, arrayPosition.y] = null;
        }
        catch
        {
            Debug.Log("Tried to pickup ore at double time, position : " + arrayPosition);
        }
    }

    private Vector3Int NormalizeIndex(Vector3Int index)
    {
        return index + SimulationSettings.indexNormalizeVector;
    }

    public static string MapBuildingToString(Placeable.Type placeable)
    {
        switch (placeable)
        {
            case Placeable.Type.Gold:
                return "G";
            case Placeable.Type.Stone:
                return "S";
            case Placeable.Type.Wood:
                return "W";
            case Placeable.Type.Energy:
                return "E";
            case Placeable.Type.Village:
                return villages.ToString();
            default:
                return "-";
        }
    }

    public static Placeable.Type MapBuildingToEnum(string s)
    {
        if (int.TryParse(s, out _))
            return Placeable.Type.Village;

        switch (s)
        {
            case "G":
                return Placeable.Type.Gold;
            case "S":
                return Placeable.Type.Stone;
            case "W":
                return Placeable.Type.Wood;
            case "E":
                return Placeable.Type.Energy;
            default:
                return Placeable.Type.Ground;
        }
    }
    public static Placeable FindPlaceableFromType(Placeable.Type type)
    {
        foreach (Placeable placeable in SelectPlaceable.placeables)
        {
            if (placeable.type == type)
                return placeable;
        }
        return null;
    }
}
