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
    public int _energyPots;
    public int energyPots
    {
        get
        {
            return _energyPots;
        }
        set
        {
            _energyPots = value;
            if (dataDisplay != null)
                dataDisplay.UpdateView();
        }
    }
    private int _energy;
    public int energy
    {
        get
        {
            return _energy;
        }
        set
        {
            _energy = value;
            if (dataDisplay != null)
                dataDisplay.UpdateView();
        }
    }
    public bool alive;
    public string[] resources = new string[] 
    { 
        MapController.MapBuildingToString(Placeable.Type.Wood), 
        MapController.MapBuildingToString(Placeable.Type.Stone), 
        MapController.MapBuildingToString(Placeable.Type.Gold),
        MapController.MapBuildingToString(Placeable.Type.Wood)+MapController.MapBuildingToString(Placeable.Type.Stone)+MapController.MapBuildingToString(Placeable.Type.Gold),
    };
    public Dictionary<Placeable.Type, int> carryingResources;
    public int totalItems;
    public string genomeString;
    public NpcDataDisplay dataDisplay;
    [HideInInspector]
    public NpcBehaviour npcBehaviour;

    // Used for stats
    public Dictionary<Placeable.Type, int> resourcesCarried;
    public int totalLife;

    public bool showingPath;

    private void Awake()
    {
        GenerateGenome();
        ParseGenome();
        carryingResources = new Dictionary<Placeable.Type, int>();
        resourcesCarried = new Dictionary<Placeable.Type, int>();
        totalItems = 0;
        totalLife = 0;
        alive = true;
        npcBehaviour = GetComponent<NpcBehaviour>();
        showingPath = false;
    }

    private void Start()
    {
        SimulationLogic.current.onShowMapOverlay += ShowingPath;
    }

    private void OnDestroy()
    {
        SimulationStatistics.current.NewData(this);
        SimulationLogic.current.onShowMapOverlay -= ShowingPath;
        if (showingPath)
            SimulationLogic.current.ShowMapOverlay(null);
    }

    private void ShowingPath(NpcData npcData)
    {
        if (npcData != null && npcData.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            showingPath = true;
        else
            showingPath = false;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    public void AddResource(string s)
    {
        Placeable.Type type = MapController.MapBuildingToEnum(s);
        AddResource(type, 1);
    }

    public void AddResource(Placeable.Type type, int number)
    {
        if (type == Placeable.Type.Energy)
            return;

        if (totalItems == maxCaringCapacity)
        {
            DropResource(type, number);
            return;
        }

        if (carryingResources.ContainsKey(type))
        {
            if (number + totalItems > maxCaringCapacity)
            {
                DropResource(type, number - (maxCaringCapacity - totalItems));

                carryingResources[type] += (maxCaringCapacity - totalItems);
                totalItems = maxCaringCapacity;
                return;
            }

            carryingResources[type] += number;
        }
        else
        {
            if (!resources[carryType].Contains(MapController.MapBuildingToString(type)))
            {
                DropResource(type, number);
                return;
            }

            if (number + totalItems > maxCaringCapacity)
            {
                DropResource(type, number - (maxCaringCapacity - totalItems));

                carryingResources.Add(type, maxCaringCapacity - totalItems);
                totalItems = maxCaringCapacity;
                return;
            }

            carryingResources.Add(type, number);
        }

        // Doesn't get cleared
        if (resourcesCarried.ContainsKey(type))
            resourcesCarried[type] += number;
        else
            resourcesCarried.Add(type, number);

        totalItems += number;
    }

    private void DropResource(Placeable.Type type, int number)
    {
        List<Placeable.Type> res = new List<Placeable.Type>();
        for (int i = 0; i < number; i++)
            res.Add(type);

        npcBehaviour.map.DropResources(npcBehaviour.mapPosition, res);
    }

    public void ClearInventory()
    {
        carryingResources.Clear();
        totalItems = 0;
    }
   
    private void GenerateGenome()
    {
        genome = "";
        for (int i = 0; i < 11; i++)
        {
            genome += Random.Range(0, 2);
        }
    }
    public void updateGenome(string genome)
    {
        this.genome = genome;
        ParseGenome();
    }

    public void ParseGenome()
    {
        if (genome.Length == 11)
        {
            //First gene
            if (genome.Substring(0, 1) == "0")
            {
                genomeString = "Μετακίνηση : 1 θέση \n";
                moveLength = 1;
            }
            else if (genome.Substring(0, 1) == "1")
            {
                genomeString = "Μετακίνηση : 2 θέσεις\n";
                moveLength = 2;
            }
            //Second gene 
            if (genome.Substring(1, 2) == "00")
            {
                genomeString += "Εξειδικεύεται στην μεταφορά ξυλείας\n";
                carryType = 0;
            }
            else if (genome.Substring(1, 2) == "01")
            {
                genomeString += "Εξειδικεύεται στην μεταφορά πέτρας\n";
                carryType = 1;
            }
            else if (genome.Substring(1, 2) == "10")
            {
                genomeString += "Εξειδικεύεται στην μεταφορά χρυσού\n";
                carryType = 2;
            }
            else if (genome.Substring(1, 2) == "11")
            {
                genomeString += "Mεταφέρει όλους τους πόρους\n";
                carryType = 3;
            }
            //Third gene 
            if (genome.Substring(3, 2) == "00")
            {
                genomeString += "Mια μονάδα πόρου κάθε στιγμή\n";
                maxCaringCapacity = 1;
            }
            else if (genome.Substring(3, 2) == "01")
            {
                genomeString += "Δυο μονάδες πόρου κάθε στιγμή\n";
                maxCaringCapacity = 2;
            }
            else if (genome.Substring(3, 2) == "10")
            {
                genomeString += "Τρεις μονάδες πόρου κάθε στιγμή\n";
                maxCaringCapacity = 3;
            }
            else if (genome.Substring(3, 2) == "11")
            {
                genomeString += "Τέσσερις μονάδες πόρου κάθε στιγμή\n";
                maxCaringCapacity = 4;
            }
            //Forth gene 
            if (genome.Substring(5, 2) == "00")
            {
                genomeString += "Ξεκινά με 10 Gold\n";
                gold = 10;
            }
            else if (genome.Substring(5, 2) == "01")
            {
                genomeString += "Ξεκινά με 20Gold\n";
                gold = 20;
            }
            else if (genome.Substring(5, 2) == "10")
            {
                genomeString += "Ξεκινά με 40Gold\n";
                gold = 40;
            }
            else if (genome.Substring(5, 2) == "11")
            {
                genomeString += "Ξεκινά με 80Gold\n";
                gold = 80;
            }
            //Fifth gene 
            if (genome.Substring(7, 2) == "00")
            {
                genomeString += "Ξεκινά με 1 Energy pot\n";
                energyPots = 1;
            }
            else if (genome.Substring(7, 2) == "01")
            {
                genomeString += "Ξεκινά με 2 Energy pot\n";
                energyPots = 2;
            }
            else if (genome.Substring(7, 2) == "10")
            {
                genomeString += "Ξεκινά με 3 Energy pot\n";
                energyPots = 3;
            }
            else if (genome.Substring(7, 2) == "11")
            {
                genomeString += "Ξεκινά με 4 Energy pot\n";
                energyPots = 4;
            }
            //Sixth gene 
            if (genome.Substring(9, 2) == "00")
            {
                genomeString += "Ξεκινά με 50 Energy points\n";
                energy = 50;
            }
            else if (genome.Substring(9, 2) == "01")
            {
                genomeString += "Ξεκινά με 100 Energy points\n";
                energy = 100;
            }
            else if (genome.Substring(9, 2) == "10")
            {
                genomeString += "Ξεκινά με 200 Energy points\n";
                energy = 200;
            }
            else if (genome.Substring(9, 2) == "11")
            {
                genomeString += "Ξεκινά με 400 Energy points\n";
                energy = 400;
            }
        }
    }
    public int GetItem(Placeable.Type type)
    {
        int count = 0;
        if (carryingResources.TryGetValue(type, out count))
            return count;
        return 0;
    }
}
