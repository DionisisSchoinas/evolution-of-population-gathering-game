using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        gridSnapping = gameObject.GetComponent<GridSnapping>();
        ClearMap();
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

    public void AddBuilding(Vector3Int index, Placeable placeable)
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

    public void DeleteBuilding(Vector3Int index, Placeable placeable)
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
    }

    public void ClearMap()
    {
        mapData = new string[100, 100];
        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                mapData[i, j] = MapBuildingToString(Placeable.Type.Ground);
            }
        }
        villages = 0;
        TextFileController.WriteMapData(mapData);
        Placeable[] children = gameObject.GetComponentsInChildren<Placeable>();
        foreach (Placeable child in children)
            Destroy(child.gameObject);
    }

    private Vector3Int NormalizeIndex(Vector3Int index)
    {
        return index + new Vector3Int(49, 0, 49);
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
}
