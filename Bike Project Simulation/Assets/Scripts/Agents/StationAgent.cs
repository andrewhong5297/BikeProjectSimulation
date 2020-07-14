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

        /*let's just say one unit is 0.00004 miles, so to travel at 6 mph we need to travel at 0.00166666666 miles per second, which we divide by the units to get 41.6f units a second. 
        * This is on a 1:1 timescale, so if we have 10 gametime seconds in an hour that becomes 10/3600 or 0.0027, which we divide from 41.6f to get 15407f units a second in gametime.
        * Our formula would be mps * 25000/0.0027 (units/gamesecond)
        */

        //so for every speed between 6mph and 18mph, we have to first convert to mps 
        biker_settings.AgentSpeed = 15000; //this should be randomly set from speed distribution from 6mph to 20 mph, 
    }
}
