using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnapping : MonoBehaviour
{
    public Vector2 gridSize = new Vector2(100, 100);
    public GameObject placeableDisplay;
    
    private float blockSize;
    private bool placing;
    private Placeable placeable;
    private GameObject showingPlaceable;
    private bool overGrid;

    private void Awake()
    {
        blockSize = transform.localScale.x / gridSize.x;
        placing = false;
        overGrid = false;
    }

    private void Update()
    {
        if (!placing)
            return;

        if (overGrid && Input.GetKeyDown(KeyCode.Mouse0))  // Place
        {
            if (showingPlaceable != null)
            {
                Material mat = showingPlaceable.GetComponent<MeshRenderer>().material;
                mat.SetFloat("_Alpha", 1f);
                showingPlaceable = null;
                PickedPlaceable(placeable);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))  // Cancel
        {
            placing = false;
            if (showingPlaceable != null)
            {
                Destroy(showingPlaceable);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!placing)
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            overGrid = true;
            if (showingPlaceable != null)
                showingPlaceable.transform.position = ClampPlaceableOnGrid(GetNearestGridPoint(hit.point, blockSize), placeable);
        }
        else
        {
            overGrid = false;
        }
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
    }

    private Vector3 GetNearestGridPoint(Vector3 position, float blockSize)
    {
        position -= transform.position;

        int x = Mathf.RoundToInt(position.x / blockSize);
        int z = Mathf.RoundToInt(position.z / blockSize);

        Vector3 result = new Vector3(
            (float)x * blockSize - blockSize / 2f,
            position.y,
            (float)z * blockSize - blockSize / 2f
        );

        result += transform.position;

        return result;
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
