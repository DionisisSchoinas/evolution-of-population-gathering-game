using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLogic : MonoBehaviour
{
    [SerializeField]
    public List<WorldObject> worldObjects;
    public int ticks = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            ticks += 1;
            foreach (WorldObject obj in worldObjects) {
                obj.Tick();
            }
        }
    }
}
