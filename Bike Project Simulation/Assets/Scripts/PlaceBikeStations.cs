using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlaceBikeStations : MonoBehaviour
{
    public GameObject map;
    public float scaled_length, scaled_width;

    public GameObject bikestation; //to be instantiated

    public Gamemanager manager;
    public List<GameObject> stations = new List<GameObject>();

    //csv file
    #region excel data lists
    List<string> file_index = new List<string>();
    List<string> file_lon = new List<string>();
    List<string> file_station_name = new List<string>();
    List<string> file_station_id = new List<string>();
    List<string> file_capacity = new List<string>();
    List<string> file_lat = new List<string>();

    [SerializeField]
    List<int> index = new List<int>();
    [SerializeField]
    List<int> station_id = new List<int>();
    [SerializeField]
    List<int> capacity = new List<int>();

    List<float> lon = new List<float>();
    List<float> lat = new List<float>();
    
    [SerializeField]
    List<float> scaled_lon = new List<float> (new float[1011]);
    [SerializeField]
    List<float> scaled_lat = new List<float> (new float[1011]);
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        using (var reader = new StreamReader(@"C:\Users\Andrew\Documents\PythonScripts\Bike Project\station_status_daily\station_info.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                file_index.Add(values[0]);
                file_lon.Add(values[1]);
                file_lat.Add(values[2]);
                file_station_name.Add(values[3]);
                file_station_id.Add(values[4]);
                file_capacity.Add(values[5]);
            }
        }

        //convert from string to int/double type
        index = file_index.Select(s => int.Parse(s)).ToList();
        lon = file_lon.Select(s => float.Parse(s)).ToList();
        lat = file_lat.Select(s => float.Parse(s)).ToList();
        station_id = file_station_id.Select(s => int.Parse(s)).ToList();
        capacity = file_capacity.Select(s => int.Parse(s)).ToList();

        //scale before instantiation
        scaled_length = map.GetComponent<MeshRenderer>().bounds.size.x;
        scaled_width = map.GetComponent<MeshRenderer>().bounds.size.z;
        ScaleLatLon();

        //instantiate 
        InstantiateStations();
        manager.gamestate = 1; //starts the game!
    }

    void ScaleLatLon()
    {

        int idx = 0;
        foreach (float i in lon)
        {
            scaled_lon[idx] = i * scaled_length;
            idx++;
        }

        idx = 0;
        foreach (float i in lat)
        {
            scaled_lat[idx] = i * scaled_width;
            idx++;
        }
    }

    void InstantiateStations()
    {
        //cached origin at bottom left, with some manual adjustment due to loss of accuracy (8 decimal points of lat/lon versus only 4 when pulling from OSM)
        float position_x = map.transform.position.x + scaled_length / 2 - 11f;
        float position_z = map.transform.position.z + scaled_width / 2 - 39.0f;

        int idx = 0;
        foreach (string station in file_station_name)
        {
            //set origin as bottom left of plane
            Vector3 instantiation_pos = new Vector3(position_x - scaled_lon[idx], map.transform.position.y, position_z - scaled_lat[idx]);
            
            //instantiate stations based on public station data
            GameObject bikestation_new = Instantiate(bikestation, instantiation_pos, map.transform.rotation);
            bikestation_new.name = file_station_name[idx];
            stations.Add(bikestation_new);
            bikestation_new.transform.SetParent(gameObject.transform); //set stationmanager as parent for cleanliness of hierarchy

            var stationscript = bikestation_new.GetComponent<StationAgent>(); //set station starting parameters
            stationscript.Id = station_id[idx];
            stationscript.Capacity = capacity[idx];
            stationscript.BikesAvailable = 10; //change this to a certain day's instantiation? or maybe I do an averaging of days... and then you can choose day from UI. 

            idx++;
        }
    }
}
