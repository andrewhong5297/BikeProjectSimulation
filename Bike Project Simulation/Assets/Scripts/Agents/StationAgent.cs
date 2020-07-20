using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationAgent : MonoBehaviour
{
    GameObject gamemanager;

    public int Id;
    public int Capacity;
    public int BikesAvailable;
    public GameObject bike_prefab;

    private void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        CheckColor();
    }
    
    public void SpawnAgent()
    {
        GameObject biker = Instantiate(bike_prefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        var biker_settings = biker.GetComponent<BikeAgentLine>(); //line for non complex navmesh        
        var speed = gamemanager.GetComponent<ExcelDataFiles>();

        biker_settings.StartingStation = name;

        /*let's just say one unit is 0.00004 miles, so to travel at 6 mph we need to travel at 0.00166666666 miles per second, which we divide by the units to get 41.6f units a second. 
        * This is on a 1:1 timescale, so if we have 10 gametime seconds in an hour that becomes 10/3600 or 0.0027, which we divide from 41.6f to get 15407f units a second in gametime.
        * Our formula would be mps * 25000/0.0027 (units/gamesecond)
        */

        //so for every speed between 6mph and 18mph, we have to first convert to mps
        int rand = Random.Range(0, 99);
        float chosen_speed = speed.speed_dist[rand]*600f; //2500 is 10 seconds, divide by 6 because 600
        biker_settings.AgentSpeed = chosen_speed; //((chosen_speed / 3600)*25000)/((10 seconds per gamehour)/3600); //this should be randomly set from speed distribution from 6mph to 20 mph, 
    }

    public void takebike()
    {
        BikesAvailable -= 1;
        Debug.LogWarning(name + " bike taken");
        CheckColor();
    }

    public void returnbike()
    {
        BikesAvailable += 1;
        Debug.LogWarning(name + " bike returned");
        CheckColor();
    }

    void CheckColor()
    {
        if(BikesAvailable > (Capacity*0.25f) && BikesAvailable <= (Capacity * 0.5f))
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        }

        if (BikesAvailable <= (Capacity * 0.25f))
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }

        if (BikesAvailable > (Capacity * 0.5f))
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
    }
}
