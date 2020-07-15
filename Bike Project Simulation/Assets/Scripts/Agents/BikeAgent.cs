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
    public string StartingStation; //agent should be instantiated from the Bike Station (stationagent) script
    public string EndingStation;
    GameObject end;
    bool endfound = false;
    GameObject start;
    bool startfound = false;

    List<string> probabilities = new List<string>();

    public List<string> test = new List<string>();

    //speeds
    public float AgentSpeed; //distance per second.
    public float distancetotravel;
    float timetotravel; //time to go to endstation, in case something goes wrong with navmesh

    private void Start()
    {
        Agent.updateUpAxis = false;
        gamemanager = GameObject.Find("GameManager");
        stationmanager = GameObject.Find("Station Manager");
    }

    // Update is called once per frame
    void Update()
    {
        if (!startfound)
        {
            CheckStart();
        }

        //agent moves towards new start station if there is one
        if (!endfound)
        {
            if (Vector3.Distance(gameObject.transform.position, start.transform.position) <= 20)
            {
                FindDestination();
            }
        }

        //agent moves towards end station
        if (endfound)
        {
            if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 20 || timer.ElapsedMilliseconds/1000 > timetotravel) // or if time is up
            {
                CheckEnd();
                //nested loop in case we have to go to a different dock
                if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 20 || timer.ElapsedMilliseconds / 1000 > timetotravel)
                {
                    OnDestinationReached();
                }
            }
        }
    }

    void CheckStart()
    {
        start = GameObject.Find(StartingStation);

        var start_status = start.GetComponent<StationAgent>();
        var scorecard = gamemanager.GetComponent<Score>();

        while (start_status.BikesAvailable == 0)
        {
            scorecard.NoBikes += 1;
            start = FindNearestStation(start);
        }

        Agent.SetDestination(start.transform.position);
        Agent.speed = AgentSpeed;
        startfound = true;
    }

    void FindDestination()
    {
        var time = gamemanager.GetComponent<Gamemanager>();
        var data = gamemanager.GetComponent<ExcelDataFiles>();
        
        int day = time.day;
        int hour = time.hour;

        UnityEngine.Debug.LogWarning(day + "-" + hour + "-" + StartingStation);
        test = data.StartEndMatch1["unique"];
        int index_of_startingstation = data.StartEndMatch1["unique"].IndexOf(day + "-" + hour + "-" + StartingStation);

        UnityEngine.Debug.Log(index_of_startingstation + "is index of unique");

        foreach (string key in data.StartEndMatch1.Keys)
        {
            probabilities.Add(data.StartEndMatch1[key][index_of_startingstation]);
        }
        
        //chooses out of cdf of matched stations
        int chosen_station = data.GetRandomIndex(probabilities);
        UnityEngine.Debug.Log("chosen station number: " + chosen_station);

        EndingStation = data.StartEndMatch1.ElementAt(chosen_station).Key;

        end = GameObject.Find(EndingStation);
        var end_status = end.GetComponent<StationAgent>();

        while (end_status.Capacity == end_status.BikesAvailable)
        {
            end = FindNearestStation(end);
            //no penalty as haven't left yet
        }

        Agent.SetDestination(end.transform.position);
        Agent.speed = AgentSpeed;
        endfound = true;

        var start_status = start.GetComponent<StationAgent>();
        start_status.takebike();

        //timer parameters
        Vector3 dist_vect = end.transform.position - transform.position;
        float dist_new = dist_vect.sqrMagnitude * 1.2f; //multiplier for not birds-eye-view
        distancetotravel = dist_new;
        timetotravel = AgentSpeed / dist_new;
        timer.Start();
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

            if(dist_new < dist_old) 
            {
                closest = station;
            }
            dist_old = dist_new;
        }
        return closest;
    }
}
