using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapRandomizer : MonoBehaviour
{
    private System.Random rnd;
    private MapController mapController;
    private SimulationSettings.Settings settings;

    private void Awake()
    {
        rnd = new System.Random();
        mapController = gameObject.GetComponent<MapController>();
    }

    public void RandomizeMap()
    {
        mapController.ClearMap(true);

        settings = SimulationSettings.simSettings;

        List<int> indexes = Enumerable.Range(0, settings.mapRows * settings.mapColumns).ToList();

        PlaceBuilding(indexes, settings.amountOfVillages, MapController.FindPlaceableFromType(Placeable.Type.Village));
        PlaceBuilding(indexes, settings.amountOfEnergyPots, MapController.FindPlaceableFromType(Placeable.Type.Energy));
        PlaceBuilding(indexes, settings.amountOfGold, MapController.FindPlaceableFromType(Placeable.Type.Gold));
        PlaceBuilding(indexes, settings.amountOfStone, MapController.FindPlaceableFromType(Placeable.Type.Stone));
        PlaceBuilding(indexes, settings.amountOfWood, MapController.FindPlaceableFromType(Placeable.Type.Wood));
    }

    private void PlaceBuilding(List<int> indexes, int amount, Placeable placeable)
    {
        List<int> buildingIndexes = new List<int>();
        int rndIndex = 0;
        int index = 0;
        Vector2Int ind = Vector2Int.zero;
        for (int i = 0; i < amount; i++)
        {
            // Get random index for the indexes list
            rndIndex = rnd.Next(0, indexes.Count());
            // Read the corresponding value
            index = indexes[rndIndex];
            // Turn index of 1d to 2d idnex
            ind = ChangeTo2dIndex(index);
            // Delete index from indexes list based on rndIndex
            indexes.RemoveAt(rndIndex);
            // Don't place on the edge
            if (ind.x == 0 || ind.x == settings.mapRows - 1 || ind.y == 0 || ind.y == settings.mapColumns - 1)
                continue;
            // Place value to map
            mapController.AddBuilding(ind, placeable, true);
        }
    }

    private Vector2Int ChangeTo2dIndex(int index)
    {
        return new Vector2Int(index % settings.mapRows, index / settings.mapRows);
    }
}
