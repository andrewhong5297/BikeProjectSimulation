using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class Gamemanager : MonoBehaviour
{
    //game stuff
    public PlaceBikeStations stations;

    GameObject gamemanager;

    public int gamestate = 0;
    public TextMeshProUGUI time;

    //track time
    Stopwatch timer = new Stopwatch();
    public int day = 0; //0-6
    public int hour = 0; //0-23
    public int gameHour = 10000; //in milliseconds

    //lists for dataframes
    int ridedata;
    List<string> station_spawn = new List<string>();

    //parameters
    public string weather;
    public int numTrucks;
    public float ridershipStress = 1.0f; //ridership stress scenario value

    private void Start()
    {
        timer.Start(); 
        gamemanager = GameObject.Find("GameManager");
    }
    
    // Update is called once per frame
    void Update()
    {

        if (gamestate == 1) //doesn't begin until data has been read in and bikes placed
        {
            //spawn trucks randomly? or at citibike HQ? 2nd ave and 36th street. maybe in gamestate 0 

            //spawn agents randomly
            if (timer.ElapsedMilliseconds >= gameHour) //every 60 seconds update time and spawn new bikers
            {
                timer.Restart();
                //shifting hour/day for datasets
                hour += 1;
                time.text = "Day: " + day + "\nHour: " + hour;

                if (hour == 24)
                {
                    hour = 0;
                    day += 1;
                    if (day == 7)
                    {
                        gamestate = 2;
                    }
                }
                var data_script = gamemanager.GetComponent<ExcelDataFiles>();

                //spawn agents throughout the hour next?
                ridedata = int.Parse(data_script.RidesDistribution[day + "-" + hour][0]);
                int riders_spawned = 0;
                UnityEngine.Debug.Log("Spawning riders: " + ridedata);

                while (riders_spawned < ridedata * ridershipStress) //could randomly move ridershipStress around
                {
                    //choose any station x times / minutes in an hour scaled
                    float random_station = Random.Range(0f, 1f);
                    

                    int selected = data_script.GetRandomIndex(data_script.StationStartP[day + "-" + hour]); //get index using data.function

                    UnityEngine.Debug.Log("other script read index: " + data_script.StartEndMatch1["unique"].IndexOf("0-0-2 Ave & E 122 St"));

                    //get station gameobject
                    GameObject selected_station = GameObject.Find(data_script.StationStartP["start station name"][selected]);
                    UnityEngine.Debug.LogWarning("Selected station to spawn: " + selected_station.name);
                    var selected_station_script = selected_station.GetComponent<StationAgent>();
                    selected_station_script.SpawnAgent();

                    riders_spawned++;
                    
                    
                }
                
            }
        }

        if (gamestate == 2)
        {
            var score_script = gamemanager.GetComponent<Score>();
            //resets game if Time = 24 or reset button is clicked
            score_script.NoBikes = 0;
            score_script.NoDocks = 0;
            gamestate = 1; 
            hour = 0;
            day = 0;
            
            //change parameters here

            GameObject[] leftover_bikes = GameObject.FindGameObjectsWithTag("Biker");
            foreach (GameObject bike in leftover_bikes)
                GameObject.Destroy(bike);

            GameObject[] trucks = GameObject.FindGameObjectsWithTag("Truck");
            foreach (GameObject truck in trucks)
                GameObject.Destroy(truck);
        }
    }
}
