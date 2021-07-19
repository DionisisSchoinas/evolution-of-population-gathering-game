using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public enum Type
    {
        Gold,
        Stone,
        Wood,
        Energy,
        Village
    }

    public Type type;
    public int gridSpace = 1;
    [ColorUsageAttribute(true, true)]
    public Color buildingColor = Color.white;
}
