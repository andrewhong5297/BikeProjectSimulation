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
    public Dictionary<int, string> dayconverter = new Dictionary<int, string>(){
        {0,"Monday" },
        {1,"Tuesday" },
        {2,"Wednesday" },
        {3,"Thursday" },
        {4,"Friday" },
        {5,"Saturday" },
        {6,"Sunday" }
    };

    public int hour = 0; //0-23
    public int gameHour = 30000; //in milliseconds
    public int gameTenMinute;
    //lists for dataframes
    int num_rides;
    List<string> station_spawn = new List<string>();

    //parameters
    public string weather;
    public int numTrucks;
    public float ridershipStress = 1.0f; //ridership stress scenario value
    //truck distance? rider avg speed? 
    //

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

                hour += 1;
                
                num_rides = int.Parse(data_script.RidesDistribution[day + "-" + hour][0])/10; //divide by 10 so it can run on my pc

                time.text = "Day: " + dayconverter[day] + "\nHour: " + hour;

                if (hour == 24)
                {
                    hour = 0;
                    day += 1;
                    if (day == 7)
                    {
                        gamestate = 2; //ends game
                    }
                }

            }

            //spawn agents randomly every ten game minutes
            if (tenminutetimer.ElapsedMilliseconds > gameTenMinute)
            {
                tenminutetimer.Restart();

                //spawn agents throughout the hour next?
                int riders_spawned = 0;
                while (riders_spawned < num_rides/6 * ridershipStress) //could randomly move ridershipStress around
                {
                    float random_station = Random.Range(0f, 1f);

                    int selected = data_script.GetRandomIndex(data_script.StationStartP[day + "-" + hour]); //get index using data.function
                    GameObject selected_station = GameObject.Find(data_script.StationStartP["start station name"][selected]);

                    var selected_station_script = selected_station.GetComponent<StationAgent>();

                    if (selected_station_script.BikesAvailable == 0)
                    {
                        var score_script = gamemanager.GetComponent<Score>();
                        score_script.NoBikes += 1;
                    }
                    else
                    {
                        selected_station_script.SpawnAgent();
                    }

                    riders_spawned++;

                }
            }
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
}
