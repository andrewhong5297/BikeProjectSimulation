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
    public Score scoremanager;
    public PlaceBikeStations stations;
    public int gamestate = 0;
    List<int> selected_spawn = new List<int>(new int[100]); //6000

    public TextMeshProUGUI time;

    //track time
    Stopwatch timer = new Stopwatch();
    public int gameTime;
    public int day; //0-6
    public int hour; //0-23

    //parameters
    public string weather;
    public int numTrucks;
    public float ridershipStress = 1.0f; //ridership stress scenario value

    // Update is called once per frame
    void Update()
    {
        if (gamestate==0)
        {
            time.text = "Waiting to start...";
        }

        if (gamestate==1) //if 10 that's because I'm testing other features
        {
            timer.Start();
            //spawn trucks 
            
            //spawn agents randomly
            if (timer.ElapsedMilliseconds >= 10000) //every 10 seconds update time and spawn new bikers
            {
                gameTime += 1;
                time.text = "Hour of Day: " + gameTime;

                for (int i = 0; i < selected_spawn.Count; i++)
                {
                    //choose any station 6000 times, with a probability distribution from excel data file
                    selected_spawn[i] = Random.Range(0, 1011);
                }

                foreach(int spawn in selected_spawn)
                {
                    GameObject selected_station = stations.stations[spawn];

                    var selected_station_script = selected_station.GetComponent<StationAgent>();
                    selected_station_script.SpawnAgent();
                    //UnityEngine.Debug.LogWarning("Spawning From " + selected_station.name);
                }
                timer.Restart();
            }
        }

        if (gamestate==2)
        {
            //end game if Time = 24 or reset button is clicked
            gameTime = 0;
            gamestate = 1;
            //need to destroy all bikes and trucks after one week? probably each day is easier
        }
    }
}
