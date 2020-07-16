using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;

public class BikeAgent : MonoBehaviour
{
    GameObject stationmanager;
    GameObject gamemanager;

    public NavMeshAgent Agent;
    public Stopwatch timer = new Stopwatch();

    //station vars
    public string StartingStation;
    public string EndingStation;
    GameObject end;
    bool endfound = false;
    GameObject start;

    //speeds
    public float AgentSpeed; //distance per second.
    public float distancetotravel;
    public float timetotravel; //time to go to endstation, in case something goes wrong with navmesh pathfinding

    private void Start()
    {
        
        NavMeshHit myNavHit;

        if (NavMesh.SamplePosition(transform.position, out myNavHit, 500, -1))
        {
            transform.position = myNavHit.position; //some of the stations aren't really on the navmesh so this helps reset it
        }
        
        Agent.updateUpAxis = false;
        gamemanager = GameObject.Find("GameManager");
        stationmanager = GameObject.Find("Station Manager");
        start = GameObject.Find(StartingStation);
        FindDestination();
    }

    // Update is called once per frame
    void Update()
    {
        //only check once find destination has finished running
        if (endfound)
        {
            timer.Start();
            float timeelap = timer.ElapsedMilliseconds / 1000f;
            
            /*
            //timetotravel needs to be fixed
            if(timeelap > 3)
            {   
                UnityEngine.Debug.LogError("destroyed on time basis");
                CheckEnd();
                OnDestinationReached();
            }
            */

            if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 20)
            {
                CheckEnd();
                //nested loop in case we have to go to a different dock
                if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 20)
                {
                    OnDestinationReached();
                }
            }
        }
    }

    void FindDestination()
    {
        var time = gamemanager.GetComponent<Gamemanager>();
        var data = gamemanager.GetComponent<ExcelDataFiles>();

        //chooses out of cdf of matched stations, sometimes key error so just keep searching until found.
        try
        {
            int chosen_station = data.GetRandomIndex(data.StartEndMatch1[time.day + "-" + time.hour + "-" + StartingStation]);
            //UnityEngine.Debug.Log("chosen station number: " + chosen_station);
            EndingStation = data.StartEndMatch1["end station name"][chosen_station];
        }
        catch (KeyNotFoundException)
        {
            UnityEngine.Debug.Log("keynotfound");
            EndingStation = FindNearestStation(start).name;
        }

        end = GameObject.Find(EndingStation);
        var end_status = end.GetComponent<StationAgent>();

        while (end_status.Capacity == end_status.BikesAvailable)
        {
            end = FindNearestStation(end);
            //no penalty here as haven't left yet
        }

        Agent.SetDestination(end.transform.position);
        Agent.speed = AgentSpeed;
        endfound = true;

        //-1 bike from start station 
        start = GameObject.Find(StartingStation);
        var start_status = start.GetComponent<StationAgent>();
        start_status.takebike();

        //timer parameters
        Vector3 dist_vect = end.transform.position - transform.position;
        float dist_new = dist_vect.sqrMagnitude * 1.2f; //multiplier for not birds-eye-view
        distancetotravel = dist_new;
        timetotravel = AgentSpeed / dist_new;
    }

    void CheckEnd()
    {
        var end_status = end.GetComponent<StationAgent>();
        var scorecard = gamemanager.GetComponent<Score>();

        while (end_status.Capacity == end_status.BikesAvailable)
        {
            scorecard.NoDocks += 1;
            end = FindNearestStation(end);
        }

        //need a try except here to just call time to travel
        Agent.SetDestination(end.transform.position);

        //timer parameters
        Vector3 dist_vect = end.transform.position - transform.position;
        float dist_new = dist_vect.sqrMagnitude * 1.2f; //multiplier for not birds-eye-view
        timetotravel = AgentSpeed / dist_new;
        timer.Restart();
        Agent.speed = AgentSpeed / 2; //usually people move slower when looking for stations to dock
    }

    void OnDestinationReached()
    {
        var end_status = end.GetComponent<StationAgent>();
        end_status.returnbike();
        //add one avail bike then destroy agent
        Destroy(gameObject);
    }

    GameObject FindNearestStation(GameObject input_station)
    {
        //finds nearest station
        GameObject closest = input_station;
        float dist_old = 999f;
        var stations = stationmanager.GetComponent<PlaceBikeStations>();
        foreach (GameObject station in stations.stations)
        {
            Vector3 dist_vect = station.transform.position - input_station.transform.position;
            float dist_new = dist_vect.sqrMagnitude;

            if (dist_new < dist_old)
            {
                closest = station;
            }
            dist_old = dist_new;
        }
        return closest;
    }
}
