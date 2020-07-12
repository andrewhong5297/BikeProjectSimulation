using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    //game stuff
    public Score scoremanager;
    public PlaceBikeStations stations;
    public int gamestate = 0;
    List<int> selected_spawn = new List<int>(new int[100]);

    public TextMeshProUGUI time;

    //track time
    public Stopwatch timer = new Stopwatch();
    public int gameTime;

    //parameters
    public string weather;
    public int numTrucks;

    // Update is called once per frame
    void Update()
    {
        if (gamestate==0)
        {
            time.text = "Waiting to start...";
        }

        if (gamestate==1)
        {
            //start spawning trucks
            timer.Start();
            //spawn agents randomly?
            if (timer.ElapsedMilliseconds >= 10000) //every 5 seconds update time and spawn new bikers
            {
                gameTime += 1;
                time.text = "Hour of Day: " + gameTime;

                for (int i = 0; i < selected_spawn.Count; i++)
                {
                    selected_spawn[i] = Random.Range(0, 1012);
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
        }
    }
}
