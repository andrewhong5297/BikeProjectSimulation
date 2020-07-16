using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class ExcelDataFiles : MonoBehaviour
{
    
    Stopwatch timer = new Stopwatch();

    #region init excel dictionary lists
    //start dist
    public Dictionary<string, List<string>> StationStartP = new Dictionary<string, List<string>>();

    //end dist
    public Dictionary<string, List<string>> StationEndP = new Dictionary<string, List<string>>();

    //start and end dist
    public Dictionary<string, List<string>> StartEndMatch1 = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> StartEndMatch2 = new Dictionary<string, List<string>>();

    //total rides
    public Dictionary<string, List<string>> RidesDistribution = new Dictionary<string, List<string>>();

    #endregion
    
    public List<int> speed_dist = new List<int>(new int[100]);

    private void Awake()
    {
        CreateSpeedDist();

        timer.Start();

        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startstationdistribution.csv", StationStartP);
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/endstationdistribution.csv", StationEndP);

        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startendstationmatching1.csv", StartEndMatch1);
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startendstationmatching2.csv", StartEndMatch2);
         
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/ridersdayhourdistribution.csv", RidesDistribution);
 
        
        #region append and remove data for start/end matching
        foreach (string key in StartEndMatch1.Keys)
        {
            foreach (string value in StartEndMatch2[key])
            {
                StartEndMatch1[key].Add(value);
            }
            //free up memory
            StartEndMatch2.Remove(key);
        }
        #endregion
        UnityEngine.Debug.Log("finished reading: " + timer.Elapsed);
        timer.Stop();
    }

    //create speed_dist from 6mph to 18 mph
    void CreateSpeedDist()
    {
        //create speed_dist
        int i = 0;
        while (i < 100)
        {
            if (i < 5)
            {
                speed_dist[i] = 6;
            }
            if (i < 10 && i >= 5)
            {
                speed_dist[i] = 7;
            }
            if (i < 15 && i >= 10)
            {
                speed_dist[i] = 8;
            }

            if (i < 40 && i >= 15)
            {
                speed_dist[i] = 10;
            }

            if (i < 85 && i >= 40)
            {
                speed_dist[i] = 12;
            }

            if (i < 90 && i >= 85)
            {
                speed_dist[i] = 18;
            }

            if (i < 100 && i >= 90)
            {
                speed_dist[i] = 20;
            }
            i++;
        }
    }

    //probably have a different void for each file. 
    void ReadCsvFile(string link, Dictionary<string, List<string>> dataframe)
    {
        WebClient client = new WebClient();
        Stream stream = client.OpenRead(link);
        using (var reader = new StreamReader(stream))
        {
            bool keys_made = false;
            List<string> keys = new List<string>();

            //this goes line by line
            while (!reader.EndOfStream)
            {
                var lines = reader.ReadLine();
                var values = lines.Split(',');

                //first start the dict
                if (!keys_made)
                {
                    foreach (string value in values)
                    {
                        dataframe.Add(new List<string> { value }[0], new List<string> { "filler" });
                    }
                    keys = new List<string>(dataframe.Keys);
                }
                else //after keys have already been made
                {
                    int i = 0;
                    foreach (string key in keys)
                    {
                        if(string.IsNullOrEmpty(new List<string> { values[i] }[0]))
                        {
                        dataframe[key].Add("0"); //data was left as nan to increase read in time
                        }
                        else
                        {
                        dataframe[key].Add(new List<string> { values[i] }[0]);
                        }
                        
                        i++;
                    }
                }
                keys_made = true;
            }
        }

        //remove "filler" from each column
        int j = 0;
        foreach (string key in dataframe.Keys)
        {
            dataframe[key].Remove("filler");
            j++;
        }
    }

    //weighted dist function
    public int GetRandomIndex(List<string> probabilities)
    {
        float chosen_p = UnityEngine.Random.Range(0f, 1f);

        List<float> num_probabilities = probabilities.Select(s => float.Parse(s)).ToList();

        int returned_idx=0;

        foreach (float prob in num_probabilities)
        {
            if(chosen_p >= prob)
            {
                returned_idx = num_probabilities.IndexOf(prob);
            }
            if(chosen_p < prob)
            {
                break; //end when probability has been found
            }
        }

        return returned_idx;
    }
}
