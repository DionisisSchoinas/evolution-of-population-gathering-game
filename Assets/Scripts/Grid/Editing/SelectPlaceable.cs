using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectPlaceable : MonoBehaviour
{
    public GridSnapping grid;
    public MapController mapController;
    public Button clearButton;
    public Button deleteButton;

    public static List<Placeable> placeables;

    private void Awake()
    {
        placeables = new List<Placeable>(gameObject.GetComponentsInChildren<Placeable>());
        foreach (Placeable placeable in placeables)
        {
            placeable.gameObject.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    PickedPlaceable(placeable);
                }
            );
        }

        clearButton.onClick.AddListener(Clear);

        deleteButton.onClick.AddListener(Delete);
    }

    private void PickedPlaceable(Placeable placeable)
    {
        grid.PickedPlaceable(placeable);
    }

    private void Clear()
    {
        mapController.ClearMap(true);
    }

    private void Delete()
    {
        grid.Deleting();
    }
}
