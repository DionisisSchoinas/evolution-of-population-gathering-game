using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NpcBehaviour : WorldObject
{
    NpcData npcData;
    public Text displayText;
    public GameObject statusDeathImage;
    // Start is called before the first frame update
    void Start()
    {
        npcData = GetComponent<NpcData>();
        Debug.Log(npcData.genome.Length);
        if (npcData.genome.Length == 11) {
            //First gene 
            Debug.Log(npcData.genome.Substring(0, 1));
            if (npcData.genome.Substring(0, 1) == "0") {
                displayText.text = "Μετακίνηση : 1 θέση \n";
                npcData.moveLength = 1;
            }
            else if (npcData.genome.Substring(0, 1) == "1")
            {
                displayText.text = "Μετακίνηση : 2 θέσεις\n";
                npcData.moveLength = 2;
            }
            //Second gene 
            if (npcData.genome.Substring(1, 2) == "00")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά ξυλείας\n";
                npcData.carryType = 0;
            }
            else if (npcData.genome.Substring(1, 2) == "01")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά πέτρας\n";
                npcData.carryType = 1;
            }
            else if (npcData.genome.Substring(1, 2) == "10")
            {
                displayText.text += "Εξειδικεύεται στην μεταφορά χρυσού\n";
                npcData.carryType = 2;
            }
            else if (npcData.genome.Substring(1, 2) == "11")
            {
                displayText.text += "Mεταφέρει όλους τους πόρους\n";
                npcData.carryType = 3;
            }
            //Third gene 
            if (npcData.genome.Substring(3, 2) == "00")
            {
                displayText.text += "Mια μονάδα πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 1;
            }
            else if (npcData.genome.Substring(3, 2) == "01")
            {
                displayText.text += "Δυο μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 2;
            }
            else if (npcData.genome.Substring(3, 2) == "10")
            {
                displayText.text += "Τρεις μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 3;
            }
            else if (npcData.genome.Substring(3, 2) == "11")
            {
                displayText.text += "Τέσσερις μονάδες πόρου κάθε στιγμή\n";
                npcData.maxCaringCapacity = 4;
            }
            //Forth gene 
            if (npcData.genome.Substring(5, 2) == "00")
            {
                displayText.text += "Ξεκινά με 10 Gold\n";
                npcData.gold = 10;
            }
            else if (npcData.genome.Substring(5, 2) == "01")
            {
                displayText.text += "Ξεκινά με 20Gold\n";
                npcData.gold = 20;
            }
            else if (npcData.genome.Substring(5, 2) == "10")
            {
                displayText.text += "Ξεκινά με 40Gold\n";
                npcData.gold = 40;
            }
            else if (npcData.genome.Substring(5, 2) == "11")
            {
                displayText.text += "Ξεκινά με 80Gold\n";
                npcData.gold =80;
            }
            //Fifth gene 
            if (npcData.genome.Substring(7, 2) == "00")
            {
                displayText.text += "Ξεκινά με 1 Energy pot\n";
                npcData.energyPots = 1;
            }
            else if (npcData.genome.Substring(7, 2) == "01")
            {
                displayText.text += "Ξεκινά με 2 Energy pot\n";
                npcData.energyPots = 2;
            }
            else if (npcData.genome.Substring(7, 2) == "10")
            {
                displayText.text += "Ξεκινά με 3 Energy pot\n";
                npcData.energyPots = 3;
            }
            else if (npcData.genome.Substring(7, 2) == "11")
            {
                displayText.text += "Ξεκινά με 4 Energy pot\n";
                npcData.energyPots = 4;
            }
            //Sixth gene 
            if (npcData.genome.Substring(9, 2) == "00")
            {
                displayText.text += "Ξεκινά με 50 Energy points\n";
                npcData.energy = 50;
            }
            else if (npcData.genome.Substring(9, 2) == "01")
            {
                displayText.text += "Ξεκινά με 100 Energy points\n";
                npcData.energy = 100;
            }
            else if (npcData.genome.Substring(9, 2) == "10")
            {
                displayText.text += "Ξεκινά με 200 Energy points\n";
                npcData.energy = 200;
            }
            else if (npcData.genome.Substring(9, 2) == "11")
            {
                displayText.text += "Ξεκινά με 400 Energy points\n";
                npcData.energy = 400;
            }
        }
    }

    public void move(int x, int z) {
        transform.position = new Vector3(x,0,z);
    }

    public override void tick()
    {
        if (npcData.alive) {
            for (int i = 0; i < npcData.moveLength; i++ )
            {
                move((int)transform.position.x + UnityEngine.Random.Range(-1, 2), (int)transform.position.z + UnityEngine.Random.Range(-1, 2));

            }
            npcData.energy--;
            if (npcData.energy == 0)
            {
                npcData.alive = false;
                statusDeathImage.SetActive(true);
            }
        }
        else {
            Destroy(gameObject);
        }
    }

}
