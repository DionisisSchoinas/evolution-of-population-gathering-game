using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : MonoBehaviour
{
    public string genome;
    //Npc variables
    public int moveLength;
    public int carryType;
    public int maxCaringCapacity;
    public int gold;
    public int energyPots;
    public int energy;
    public bool alive;
    public string[] resources = new string[] {"W", "S", "G","WSG"}; 
    
    private void Awake()
    {
        generateGenome();
        alive = true;
    }
   
    private void generateGenome()
    {
        genome = "";
        for (int i = 0; i < 11; i++){

            genome += Random.Range(0, 2);
        }
    }
}
