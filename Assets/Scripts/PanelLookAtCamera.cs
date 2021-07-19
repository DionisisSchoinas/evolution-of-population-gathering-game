using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelLookAtCamera : MonoBehaviour
{
    public Transform cam;

    public void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
