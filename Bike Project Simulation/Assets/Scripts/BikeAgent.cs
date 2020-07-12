using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BikeAgent : MonoBehaviour
{
    public Camera cam;
    public Score scorecard;
    public PlaceBikeStations stations;

    //need a time variable to take from somewhere... right now 5 or 10 seconds in an hour makes the most sense. 
    public NavMeshAgent Agent;

    public string StartingStation; //agent should be instantiated from the Bike Station (stationagent) script
    public string EndingStation;

    GameObject end;
    bool endfound = false;
    GameObject start;
    bool startfound = false;

    public float AgentSpeed;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 18f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startfound)
        {
            CheckStart();
        }
        //agent moves towards new start station if there is one
        if (!endfound)
        {
            if (Vector3.Distance(gameObject.transform.position, start.transform.position) <= 20)
            {
                FindDestination();
            }
        }

        //agent moves towards end station
        if (endfound)
        {
            Debug.Log(Vector3.Distance(gameObject.transform.position, end.transform.position));
            if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 40)
            {
                CheckEnd();
                //nested loop in case we have to go to a different dock
                if (Vector3.Distance(gameObject.transform.position, end.transform.position) <= 40)
                {
                    OnDestinationReached();
                }
            }
        }
        
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            
            if(Physics.Raycast(ray, out hit))
            {
                Agent.SetDestination(hit.point);
                Debug.LogWarning("trying to move");
            }
        }
        */
    }

    void CheckStart()
    {
        //checks if StartingStation has bikes or not
        //if not, then find nearest station
        //add to global counter of full stations
        start = GameObject.Find(StartingStation);

        var start_status = start.GetComponent<StationAgent>();

        while (start_status.BikesAvailable == 0)
        {
            scorecard.NoBikes += 1;
            start = FindNearestStation(start);
        }

        Agent.SetDestination(start.transform.position);
        Agent.speed = AgentSpeed;
        startfound = true;
    }

    void FindDestination()
    {
        //chooses out of pdf of 15 most popular stations, checks availability of that station. 
        //run a pause? 
        Debug.Log("finding destination");
        EndingStation = "Scholes St & Manhattan Ave";
        end = GameObject.Find(EndingStation);
        var end_status = end.GetComponent<StationAgent>();

        while (end_status.Capacity == end_status.BikesAvailable)
        {
            end = FindNearestStation(end);
            //no penalty as haven't left yet
        }

        Agent.SetDestination(end.transform.position);
        Agent.speed = AgentSpeed;
        Debug.Log("Destination Set!");
        endfound = true;
    }

    void CheckEnd()
    {
        var end_status = end.GetComponent<StationAgent>();

        while (end_status.Capacity == end_status.BikesAvailable)
        {
            scorecard.NoDocks += 1;
            end = FindNearestStation(end);
        }

        Agent.SetDestination(end.transform.position);
        Agent.speed = AgentSpeed / 2; //usually people move slower when looking for stations to dock
    }

    void OnDestinationReached()
    {
        var end_status = end.GetComponent<StationAgent>();
        end_status.BikesAvailable += 1;
        //add one avail bike then destroy agent
        Destroy(gameObject);
    }

    GameObject FindNearestStation(GameObject input_station)
    {
        //finds nearest station
        GameObject closest = input_station;
        float dist_old = 999f;
        foreach (GameObject station in stations.stations)
        {
            Vector3 dist_vect = station.transform.position - input_station.transform.position;
            float dist_new = dist_vect.sqrMagnitude;

            if(dist_new < dist_old) 
            {
                closest = station;
            }
            dist_old = dist_new;
        }
        return closest;
    }

    /* For later, might be easier to just use euclidan for this since actual distance doesn't matter. 
     * R = 6373.0 #radius of the Earth
        
    lat1 = math.radians(start_unique[start_unique["station name"]==start_station]['latitude'])     
    lon1 = math.radians(start_unique[start_unique["station name"]==start_station]['longitude'])     
    
    #fastest way to filter? since it needs to check distance.                                  
    lat2 = math.radians(end_unique[end_unique["station name"]==other_station]['latitude'])     
    lon2 = math.radians(end_unique[end_unique["station name"]==other_station]['longitude'])     

    #change in coordinates                                 
    dlon = lon2 - lon1      
    dlat = lat2 - lat1

    #Haversine formula    
    a = math.sin(dlat / 2)**2 + math.cos(lat1) * math.cos(lat2) * math.sin(dlon / 2)**2
    c = 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))
    distance = R * c
    
    #convert from kilometers to miles
    distance = distance/1.603
    
    #multiplier to account for actual route distance
    distance = distance*1.1
    */
}
