using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectPlaceable : MonoBehaviour
{
    public GridSnapping grid;
    public Button deleteButton;

    private List<Placeable> placeables;

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

        deleteButton.onClick.AddListener(
            delegate
            {
                Delete();
            }
        );
    }

    private void PickedPlaceable(Placeable placeable)
    {
        grid.PickedPlaceable(placeable);
    }

    private void Delete()
    {
        grid.Deleting();
    }
}
