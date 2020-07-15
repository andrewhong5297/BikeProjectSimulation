using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TruckAgent : MonoBehaviour
{
    public PlaceBikeStations stations; //for accessing list of stations
    GameObject gamemanager;

    public int BikesCarried;
    public float DistanceRoam;
    public NavMeshAgent truck;

    public string StartingStation; //agent should be instantiated from the Bike Station (stationagent) script
    public string EndingStation;

    GameObject end;
    GameObject start;

    public float AgentSpeed = 40000f; //probably faster

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        truck.updateUpAxis = false;
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
        var data = gamemanager.GetComponent<ExcelDataFiles>();
        //get most popular end stations by time

        //get most popular start stations by time

        //have some sort of flag for if that station has already been truck assigned?

        return end;
    }
    
    void FilterStationByTime()
    {
        //needs its own timer
    }
}
