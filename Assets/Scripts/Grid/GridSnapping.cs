using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnapping : MonoBehaviour
{
    public Vector2 gridSize = new Vector2(100, 100);
    public GameObject placeableDisplay;

    private MapController mapController;

    private float blockSize;
    private bool placing;
    private Placeable placeable;
    private GameObject showingPlaceable;
    private bool overGrid;
    private bool deleting;
    private Placeable currentDelete;

    private void Awake()
    {
        mapController = gameObject.GetComponent<MapController>();

        blockSize = transform.localScale.x / gridSize.x;
        placing = false;
        overGrid = false;
        deleting = false;
    }

    private void Update()
    {
        if (overGrid && Input.GetKeyDown(KeyCode.Mouse0))  // Left click
        {
            MouseClicked(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))  // Right Click
        {
            MouseClicked(KeyCode.Mouse1);
        }
    }

    private void MouseClicked(KeyCode key)
    {
        if (placing)
        {
            switch(key)
            {
                // Place
                case KeyCode.Mouse0:
                    if (showingPlaceable != null)
                    {
                        // Add to map data
                        mapController.AddBuilding(GetGetNearestGridPointIndex(showingPlaceable.transform.position, blockSize), placeable);
                        // Enable collider
                        showingPlaceable.gameObject.GetComponent<BoxCollider>().enabled = true;
                        // Add extra building data
                        Placeable pl = showingPlaceable.gameObject.AddComponent<Placeable>();
                        pl.CopyData(placeable);
                        // Disable transparency
                        Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
                        mat.SetFloat("_Alpha", 1f);

                        // Spawn new building
                        showingPlaceable = null;
                        PickedPlaceable(placeable);
                    }
                    return;
                // Cancel
                case KeyCode.Mouse1:
                    placing = false;
                    if (showingPlaceable != null)
                    {
                        Destroy(showingPlaceable);
                    }
                    return;
            }
        }

        if (deleting)
        {
            switch (key)
            {
                // Delete
                case KeyCode.Mouse0:
                    if (showingPlaceable != null && currentDelete != null)
                    {
                        // Add to map data
                        mapController.DeleteBuilding(GetGetNearestGridPointIndex(showingPlaceable.transform.position, blockSize), placeable);
                        // Delete building
                        Destroy(currentDelete.transform.gameObject);

                        // Reset 
                        currentDelete = null;
                    }
                    return;
                // Cancel
                case KeyCode.Mouse1:
                    deleting = false;
                    if (showingPlaceable != null)
                    {
                        currentDelete = null;
                        Destroy(showingPlaceable);
                    }
                    return;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!placing && !deleting)
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            overGrid = true;

            if (placing)
            {
                if (showingPlaceable != null)
                    showingPlaceable.transform.position = ClampPlaceableOnGrid(GetNearestGridPoint(hit.point, blockSize), placeable);
            }
            else if (deleting)
            {
                Placeable placeable = hit.transform.gameObject.GetComponent<Placeable>();
                // Hit ground
                if (placeable == null)
                {
                    Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
                    mat.SetFloat("_Alpha", 0f);
                    currentDelete = null;
                }
                // Hit building
                else
                {
                    Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
                    mat.SetFloat("_Alpha", 0.6f);
                    showingPlaceable.transform.localScale = new Vector3(placeable.gridSpace * 1.2f, 1f, placeable.gridSpace * 1.2f);
                    showingPlaceable.transform.position = placeable.transform.position;
                    currentDelete = placeable;
                }
            }
        }
        else
        {
            overGrid = false;
        }
    }

    public void Deleting()
    {
        placing = false;
        if (showingPlaceable != null)
        {
            Destroy(showingPlaceable);
        }

        deleting = true;

        showingPlaceable = Instantiate(placeableDisplay);
        Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_Alpha", 0f);
        mat.SetColor("_Color", Color.red);
        showingPlaceable.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    public void PickedPlaceable(Placeable placeable)
    {
        if (showingPlaceable != null)
        {
            Destroy(showingPlaceable);
        }

        placing = true;
        this.placeable = placeable;

        showingPlaceable = Instantiate(placeableDisplay);
        showingPlaceable.transform.localScale = new Vector3(placeable.gridSpace, 0.5f, placeable.gridSpace);
        Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_Alpha", 0.6f);
        mat.SetColor("_Color", placeable.buildingColor);
        showingPlaceable.gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    private Vector3Int GetGetNearestGridPointIndex(Vector3 position, float blockSize)
    {
        position -= transform.position;

        int x = Mathf.RoundToInt(position.x / blockSize);
        int z = Mathf.RoundToInt(position.z / blockSize);

        return new Vector3Int(x, 0, z);
    }

    private Vector3 GetNearestGridPoint(Vector3 position, float blockSize)
    {
        Vector3Int index = GetGetNearestGridPointIndex(position, blockSize);

        Vector3 result = new Vector3(
            (float)index.x * blockSize - blockSize / 2f,
            position.y,
            (float)index.z * blockSize - blockSize / 2f
        );

        result += transform.position;

        return result;
    }

    public Vector3 GetNearestGridPoint(Vector3 position)
    {
        return GetNearestGridPoint(position, blockSize);
    }

    private Vector3 ClampPlaceableOnGrid(Vector3 position, Placeable placeable)
    {
        /*
        float leftEdge = transform.position.x - transform.localScale.x / 2f + blockSize / 2f + (float)placeable.gridSpace / 2f;
        float rightEdge = transform.position.x + transform.localScale.x / 2f - blockSize / 2f - (float)placeable.gridSpace / 2f;
        float downEdge = transform.position.z - transform.localScale.z / 2f + blockSize / 2f + (float)placeable.gridSpace / 2f;
        float upEdge = transform.position.z + transform.localScale.z / 2f - blockSize / 2f - (float)placeable.gridSpace / 2f;
        */

        float leftEdge = transform.position.x - transform.localScale.x / 2f + (float)placeable.gridSpace / 2f;
        float rightEdge = transform.position.x + transform.localScale.x / 2f - (float)placeable.gridSpace / 2f;
        float downEdge = transform.position.z - transform.localScale.z / 2f + (float)placeable.gridSpace / 2f;
        float upEdge = transform.position.z + transform.localScale.z / 2f - (float)placeable.gridSpace / 2f;

        position.x = Mathf.Clamp(position.x, leftEdge, rightEdge);
        position.z = Mathf.Clamp(position.z, downEdge, upEdge);

        return position;
    }
}
