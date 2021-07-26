using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenomeValueDisplay : MonoBehaviour
{
    private Text genome;
    private Text value;

    private void Awake()
    {
        Text[] text = gameObject.GetComponentsInChildren<Text>();
        genome = text[0];
        value = text[1];
    }

    public void SetValue(SimulationStatistics.GenomeIntPair genomeIntPair)
    {
        if (genomeIntPair.genome.Equals(""))
        {
            gameObject.SetActive(false);
            return;
        }
        genome.text = genomeIntPair.genome;
        value.text = genomeIntPair.value.ToString();
    }
}
