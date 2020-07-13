using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TruckAgent : MonoBehaviour
{
    public PlaceBikeStations stations; //for accessing list of stations

    public int BikesCarried;
    public float DistanceRoam;
    public NavMeshAgent truck;

    public string StartingStation; //agent should be instantiated from the Bike Station (stationagent) script
    public string EndingStation;

    GameObject end;
    GameObject start;

    public float AgentSpeed = 250f; //need to figure out scale of map then adjust by time scale

    // Start is called before the first frame update
    void Start()
    {
        start = GameObject.Find(StartingStation);
        end = GameObject.Find(StartingStation); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 20)
        {
            start = end;
            end = ChooseNextStation();
            truck.SetDestination(end.transform.position);
            truck.speed = AgentSpeed;
        }
    }

    GameObject ChooseNextStation()
    {
        //end is the closest station 
        return end;
    }
    
    void FilterStationByTime()
    {
        //needs its own timer
    }
}
