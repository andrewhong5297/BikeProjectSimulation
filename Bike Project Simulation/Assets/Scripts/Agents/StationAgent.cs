using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationAgent : MonoBehaviour
{
    //some way to track time?
    public int Id;
    public int Capacity;
    public int BikesAvailable;

    public GameObject bike_prefab;


    List<int> speed_dist = new List<int>(new int[100]);

    public void SpawnAgent()
    {
        //creates an agent of some speed
        GameObject biker = Instantiate(bike_prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

        var biker_settings = biker.GetComponent<BikeAgent>();

        biker_settings.StartingStation = name;
        biker_settings.AgentSpeed = 150f; //this should be randomly set from speed distribution from 6mph to 20 mph, 
    }

}
