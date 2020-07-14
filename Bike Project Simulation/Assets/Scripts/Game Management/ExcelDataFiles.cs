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
    public Stopwatch timer = new Stopwatch();

    //start dist
    public Dictionary<string, List<string>> StationStartP = new Dictionary<string, List<string>>();

    //end dist
    public Dictionary<string, List<string>> StationEndP = new Dictionary<string, List<string>>();

    //start and end dist
    public Dictionary<string, List<string>> StartEndMatch1 = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> StartEndMatch2 = new Dictionary<string, List<string>>();

    //total rides
    public Dictionary<string, List<string>> RidesDistribution = new Dictionary<string, List<string>>();

    //daily bike distribution init data
    //need to upload this later 

    //list of stations in data?
    public List<string> stations = new List<string>();

    public string[] test;

    void Start()
    {
        timer.Start();
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startstationdistribution.csv", StationStartP);
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/endstationdistribution.csv", StationEndP);
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startendstationmatching1.csv", StartEndMatch1);
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/startendstationmatching2.csv", StartEndMatch2); //need to append
        ReadCsvFile("https://raw.githubusercontent.com/andrewhong5297/BikeProjectSimulation/master/Bike%20Project%20Simulation/ridersdayhourdistribution.csv", RidesDistribution);

        foreach (string key in StartEndMatch1.Keys)
        {
            foreach (string value in StartEndMatch2[key])
            {
                StartEndMatch1[key].Add(value);
            }
        }

        UnityEngine.Debug.Log(timer.Elapsed);
        timer.Stop();
        stations = StartEndMatch1["Wyckoff St & 3 Ave"];
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
                        dataframe[key].Add(new List<string> { values[i] }[0]);
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
}
