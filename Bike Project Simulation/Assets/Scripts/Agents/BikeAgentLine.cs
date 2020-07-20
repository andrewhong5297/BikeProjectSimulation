using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;

public class BikeAgentLine : MonoBehaviour
{
    GameObject stationmanager;
    GameObject gamemanager;

    public List<Transform> waypoints = new List<Transform>();
    public List<Collider> waypoints_Test = new List<Collider>();

    public NavMeshAgent Agent;
    Stopwatch bike_timer = new Stopwatch();

    //station vars
    public string StartingStation;
    string EndingStation;
    GameObject end;
    bool endfound = false;
    GameObject start;

    string StartingTag;
    string EndingTag;
    int currentwaypoint = 0;

    //speeds
    public float AgentSpeed; //distance per second.
    float distancetotravel;
    float timetotravel; //time to go to endstation, in case something goes wrong with navmesh pathfinding
    
    private void Start()
    {
        Agent.updateUpAxis = false;
        gamemanager = GameObject.Find("GameManager");
        stationmanager = GameObject.Find("Station Manager");
        start = GameObject.Find(StartingStation);
        StartingTag = start.tag;

        FindDestination();
    }
    
    // Update is called once per frame
    void Update()
    {
        //only check once find destination has finished running
        if (endfound)
        {
            Vector3 dist_vect = waypoints[currentwaypoint].transform.position - transform.position;
            float dist_new = dist_vect.sqrMagnitude;

            if (dist_new <= 50)
            {
                //if last waypoint reached
                if (currentwaypoint == waypoints.Count-1)
                {
                    CheckEnd();
                    dist_vect = end.transform.position - transform.position;
                    dist_new = dist_vect.sqrMagnitude;
                    if (dist_new <= 50)
                    {
                        OnDestinationReached();
                    }
                }
                else
                {
                    try
                    {
                        SetNextWaypoint();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        CheckEnd();
                        OnDestinationReached(); //not sure if this is fine, will have to test
                    }
                    
                }
            }
        }
    }
    
    void SetNextWaypoint()
    {
        currentwaypoint += 1;
        Agent.SetDestination(waypoints[currentwaypoint].position);
        Vector3 direction = (waypoints[currentwaypoint].position - transform.position).normalized;
        Quaternion qDir = Quaternion.LookRotation(direction);
        //transform.rotation = qDir; //allows for instant turning. causes problems when on certain bridges
    }

    void FindDestination()
    {
        endfound = true;
        UnityEngine.Debug.Log("finding destination");

        var time = gamemanager.GetComponent<Gamemanager>();
        var data = gamemanager.GetComponent<ExcelDataFiles>();
        
        //chooses out of cdf of matched stations, sometimes key error so just keep searching until found.
        try
        {
            int chosen_station = data.GetRandomIndex(data.StartEndMatch1[time.day + "-" + time.hour + "-" + StartingStation]);
            UnityEngine.Debug.Log("chosen station number: " + chosen_station);
            EndingStation = data.StartEndMatch1["end station name"][chosen_station];
        }
        catch (KeyNotFoundException)
        {
            UnityEngine.Debug.Log("keynotfound");
            EndingStation = FindNearestStation(start).name;
        }

        end = GameObject.Find(EndingStation);
        
        //set waypoints to destination
        #region find waypoints

        bool done_finding = false;
        
        bool same_borough = false;
        if (start.tag == end.tag)
            same_borough = true;

        string within_borough = start.gameObject.tag;

        Transform savedwaypoint = start.transform;

        int search_radius = 50;
        bool went_on_bridge = false;
        int i = 0;
        while (i < 50)//(!done_finding), for some reason done_finding doesn't work 
        {
            if (went_on_bridge)
            {
                search_radius = 200;
            }
            else
            {
                search_radius = 100;
            }
            went_on_bridge = false; //resets every loop

            Collider[] stations = Physics.OverlapSphere(savedwaypoint.position, search_radius);
            waypoints_Test = stations.ToList<Collider>();

            foreach (Collider col in stations)
            {
                if (col.gameObject.tag == within_borough)
                {
                    if (col.gameObject.name == end.name) //break if end transform is in sight
                    {
                        waypoints.Add(end.transform);
                        done_finding = true;
                        i = 99; //ends while loop
                        break;
                    }

                    Vector3 dist_vect = end.transform.position - col.gameObject.transform.position;
                    dist_vect.y = 0;
                    float dist_new = dist_vect.sqrMagnitude;

                    //checks distance against current waypoint
                    dist_vect = end.transform.position - savedwaypoint.position;
                    dist_vect.y = 0;
                    float dist_old = dist_vect.sqrMagnitude;

                    if (dist_new < dist_old)
                    {
                        Transform pre_save = savedwaypoint;
                        savedwaypoint = col.gameObject.transform;
                    }
                }
            }

            //check for bridge if we have to go across boroughs 
            if (!same_borough)
            {
                Vector3 dist_vect = end.transform.position - savedwaypoint.position;
                dist_vect.y = 0;
                float dist_new = dist_vect.sqrMagnitude;

                Collider[] bridges = Physics.OverlapSphere(savedwaypoint.position, 100); //from previously saved waypoint
                foreach (Collider bridge in bridges)
                {
                    if (bridge.gameObject.tag == "Bridge")
                    {
                        dist_vect = end.transform.position - bridge.transform.position;
                        dist_vect.y = 0;
                        float dist_bridge = dist_vect.sqrMagnitude;
                        if (dist_bridge < dist_new) //if bridge brings us closer than chosen station
                        {
                            savedwaypoint = bridge.transform;
                            within_borough = end.tag; //change tag to next borough 
                            same_borough = true; //shouldn't have to cross two bridges?
                            went_on_bridge = true;
                        }
                    }
                }
            }

            if (!done_finding)
            {
                waypoints.Add(savedwaypoint);
            }

            if (i==20)
            {
                waypoints.Add(end.transform);
            }

            i++;
        }
        #endregion
        //-1 bike from start station 
        start = GameObject.Find(StartingStation);
        var start_status = start.GetComponent<StationAgent>();
        start_status.takebike();

        Agent.SetDestination(waypoints[currentwaypoint].transform.position);
        Agent.speed = AgentSpeed;
        endfound = true;
    }

    void CheckEnd()
    {
        var end_status = end.GetComponent<StationAgent>();
        var scorecard = gamemanager.GetComponent<Score>();

        //checks for docks
        if (end_status.Capacity == end_status.BikesAvailable)
        {
            scorecard.NoDocks += 1;
            end = FindNearestStation(end);
        }

        Agent.SetDestination(end.transform.position);

        //timer parameters
        Vector3 dist_vect = end.transform.position - transform.position;
        float dist_new = dist_vect.sqrMagnitude * 1.2f; //multiplier for not birds-eye-view
        timetotravel = AgentSpeed / dist_new;
        bike_timer.Restart();
        Agent.speed = AgentSpeed / 2; //usually people move slower when looking for stations to dock
    }

    void OnDestinationReached()
    {
        var end_status = end.GetComponent<StationAgent>();
        end_status.returnbike();
        //add one avail bike then destroy agent
        UnityEngine.Debug.Log("destination reached");
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
