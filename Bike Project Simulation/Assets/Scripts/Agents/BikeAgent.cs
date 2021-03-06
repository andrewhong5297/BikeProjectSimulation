﻿using System.Collections;
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
    Stopwatch bike_timer = new Stopwatch();

    //station vars
    public string StartingStation;
    string EndingStation;
    GameObject end;
    bool endfound = false;
    GameObject start;

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

        FindDestination();
    }
    
    // Update is called once per frame
    void Update()
    {
        //only check once find destination has finished running
        if (endfound)
        {
            bike_timer.Start();
            
            float timeelap = bike_timer.ElapsedMilliseconds / 1000f;
            
            
            //timetotravel needs to be fixed
            if(timeelap > 30)
            {   
                UnityEngine.Debug.LogError("destroyed on time basis");
                CheckEnd();
                Vector3 dist_vects = end.transform.position - transform.position;
                float dist_news = dist_vects.sqrMagnitude;
                if (dist_news <= 20)
                {
                    OnDestinationReached();
                }
            }
            

            Vector3 dist_vect = end.transform.position - transform.position;
            float dist_new = dist_vect.sqrMagnitude;

            if (dist_new <= 20)
            {
                CheckEnd();
                //nested loop in case we have to go to a different dock
                if (dist_new <= 20)
                {
                    OnDestinationReached();
                }
            }
        }
    }

    void FindDestination()
    {
        UnityEngine.Debug.Log("finding destination");
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
