using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMPro;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    //game stuff
    public PlaceBikeStations stations;

    GameObject gamemanager;

    public int gamestate = 0;
    public TextMeshProUGUI time;

    //track time
    Stopwatch hourtimer = new Stopwatch();
    Stopwatch tenminutetimer = new Stopwatch();
    public int day = 0; //0-6
    public int hour = 0; //0-23
    
    Dictionary<int, string> dayconverter = new Dictionary<int, string>(){
        {0,"Monday" },
        {1,"Tuesday" },
        {2,"Wednesday" },
        {3,"Thursday" },
        {4,"Friday" },
        {5,"Saturday" },
        {6,"Sunday" }
    };

    int gameHour = 1000; //in milliseconds
    int gameTenMinute;


    int num_rides;
    bool num_rides_found = false;
    List<string> station_spawn = new List<string>();

    //parameters
    string weather;
    int numTrucks;
    float ridershipStress = 1.0f; //ridership stress scenario value
    //truck distance? rider avg speed? 

    private void Start()
    {
        hourtimer.Start();
        tenminutetimer.Start();
        gamemanager = GameObject.Find("GameManager");
        gameTenMinute = gameHour / 6;
    }

    // Update is called once per frame
    void Update()
    {
        //spawn trucks randomly? or at citibike HQ? 2nd ave and 36th street. maybe in gamestate 0 
        
        if (gamestate == 1) //doesn't begin until data has been read in and bikes placed
        {
            var data_script = gamemanager.GetComponent<ExcelDataFiles>();

            //change day/hour every game hour
            if (hourtimer.ElapsedMilliseconds >= gameHour) //every 60 seconds update time and spawn new bikers
            {
                hourtimer.Restart();

                if (hour == 23)
                {
                    hour = 0;
                    day += 1;
                    if (day == 7)
                        {
                            gamestate = 2; //ends game
                        }
                }
                else
                {
                    hour += 1;
                }

                UnityEngine.Debug.LogError(day + "-" + hour);
                num_rides = int.Parse(data_script.RidesDistribution[day + "-" + hour][0]); //divide by 10 so it can run on my pc
                num_rides_found = true;

                time.text = "Day: " + dayconverter[day] + "\nHour: " + hour;
                UnityEngine.Debug.LogWarning("number of riders this hour: " + num_rides);

                SpawnRiders(num_rides);
            }

            #region ten minute spawning
            /*
            //spawn agents randomly every ten game minutes
            if (tenminutetimer.ElapsedMilliseconds > gameTenMinute && num_rides_found);
            {
                tenminutetimer.Restart();
                
                UnityEngine.Debug.LogWarning("starting call loop...");
                //spawn agents throughout the hour next?
                float riders_spawned = 0;
                float riders_to_spawn = num_rides / 6;

                UnityEngine.Debug.LogWarning("riders to spawn: " + riders_to_spawn);
                while (riders_spawned < num_rides) //could randomly move ridershipStress around
                {
                    float random_station = Random.Range(0f, 1f);

                    int selected = data_script.GetRandomIndex(data_script.StationStartP[day + "-" + hour]); //get index using data.function
                    GameObject selected_station = GameObject.Find(data_script.StationStartP["start station name"][selected]);

                    var selected_station_script = selected_station.GetComponent<StationAgent>();
                    UnityEngine.Debug.LogWarning("selected station " + selected_station.name);
                    if (selected_station_script.BikesAvailable == 0)
                    {
                        var score_script = gamemanager.GetComponent<Score>();
                        score_script.NoBikes += 1;
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("calling spawnagent...");
                        selected_station_script.SpawnAgent();
                        riders_spawned++;
                    }
                }
            }
            */
            #endregion
        }

        //end game
        if (gamestate == 2)
        {
            var score_script = gamemanager.GetComponent<Score>();
            //resets game if Time = 24 or reset button is clicked
            score_script.NoBikes = 0;
            score_script.NoDocks = 0;
            gamestate = 0; //1 if we want to restart
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

    void SpawnRiders(int num_rides)
    {
        var data_script = gamemanager.GetComponent<ExcelDataFiles>();

        int riders_spawned = 0;
        while (riders_spawned < num_rides) //could randomly move ridershipStress around
        {
            float random_station = Random.Range(0f, 1f);
            int selected = data_script.GetRandomIndex(data_script.StationStartP[day + "-" + hour]); //get index using data.function
            GameObject selected_station = GameObject.Find(data_script.StationStartP["start station name"][selected]);

            var selected_station_script = selected_station.GetComponent<StationAgent>();
            //UnityEngine.Debug.LogWarning("selected station " + selected_station.name);
            
            if (selected_station_script.BikesAvailable == 0)
            {
                var score_script = gamemanager.GetComponent<Score>();
                score_script.NoBikes += 1;
            }
            else
            {
                UnityEngine.Debug.LogWarning("calling spawnagent...");
                //selected_station_script.SpawnAgent();
                riders_spawned++;
            }
            
            riders_spawned++;
        }
    }
}
