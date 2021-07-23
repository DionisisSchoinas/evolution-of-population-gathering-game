using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOverlay : MonoBehaviour
{
    Material material;

    private void Awake()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;
        material.SetFloat("_Alpha", 0f);
        material.SetColor("_Color", Color.red);
    }

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", Mathf.Clamp(alpha, 0f, 1f));
    }
}
