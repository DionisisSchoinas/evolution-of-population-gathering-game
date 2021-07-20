using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public string[,] mapData;

    private void Awake()
    {
        mapData = new string[100, 100];
        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                mapData[i,j] = MapBuildingToString(Placeable.Type.Ground);
            }
        }
    }

    public bool hasSpace(Vector3Int index, Placeable placeable)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f);
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
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f);
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                mapData[i, j] = MapBuildingToString(placeable.type);
            }
        }
        TextFileController.WriteMapData(mapData);
    }

    public void DeleteBuilding(Vector3Int index, Placeable placeable)
    {
        index = NormalizeIndex(index);
        int blockSize = Mathf.FloorToInt(placeable.gridSpace / 2f);
        for (int i = (index.x - blockSize); i <= (index.x + blockSize); i++)
        {
            for (int j = (index.z - blockSize); j <= (index.z + blockSize); j++)
            {
                mapData[i, j] = MapBuildingToString(Placeable.Type.Ground);
            }
        }
        TextFileController.WriteMapData(mapData);
    }

    private Vector3Int NormalizeIndex(Vector3Int index)
    {
        return index + new Vector3Int(49, 0, 49);
    }

    public string MapBuildingToString(Placeable.Type placeable)
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
                return "V";
            default:
                return "-";
        }
    }

    public Placeable.Type MapBuildingToEnum(string s)
    {
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
            case "V":
                return Placeable.Type.Village;
            default:
                return Placeable.Type.Ground;
        }
    }
}
