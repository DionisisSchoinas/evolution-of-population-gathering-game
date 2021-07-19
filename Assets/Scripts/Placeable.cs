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
        Village,
        Ground
    }

    public Type type;
    public int gridSpace = 1;
    [ColorUsageAttribute(true, true)]
    public Color buildingColor = Color.white;

    private void Awake()
    {
        if (gridSpace <= 0)
            gridSpace = 1;

        if (gridSpace % 2 != 1)
        {
            gridSpace -= 1;
        }
    }

    public void CopyData(Placeable placeable)
    {
        this.type = placeable.type;
        this.gridSpace = placeable.gridSpace;
        this.buildingColor = placeable.buildingColor;
    }
}
