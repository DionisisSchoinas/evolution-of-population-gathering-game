using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class TextFileController : MonoBehaviour
{
    public static void WriteMapData(string[,] mapData,string filename)
    {
        string path = Application.persistentDataPath + "/"+ filename+".txt";

        //Debug.Log("Save location : " + path);

        //Write some text to the map.txt file
        StreamWriter writer = new StreamWriter(path, false);

        try
        {
            for (int i = 0; i < mapData.GetLength(0); i++)
            {
                string lineData = "";
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    lineData += mapData[i, j];
                }
                writer.WriteLine(lineData);
            }
        }
        finally
        {
            writer.Close();
        }
    }

    public static void WriteMapData(string[,] mapData)
    {
        WriteMapData(mapData, "map");
    }
}
