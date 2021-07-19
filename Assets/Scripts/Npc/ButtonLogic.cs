using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonLogic : MonoBehaviour
{
    public GameObject canvas;
    public Text butonText;
    bool viewEnabled = false;
    public void Start()
    {
  
    }
    public void toggleCanvas()
    {
        if (viewEnabled)
        {
            canvas.SetActive(false);
            butonText.text = "Show";
            viewEnabled = false;
        }
        else
        {
            canvas.SetActive(true);
            butonText.text = "Hide";
            viewEnabled = true;
        }
    }
}
