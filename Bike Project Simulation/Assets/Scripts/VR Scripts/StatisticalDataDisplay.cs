using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticalDataDisplay : MonoBehaviour
{
    [SerializeField]
    private Text stationname;
    private static Dictionary<int, Queue<StatisticalDataDisplay>> dataDisplay = null;
    private int prefabId = 0; //associated with the queue 

    public static StatisticalDataDisplay Create(StatisticalDataDisplay prefab, string stationName, Transform parent)
    {
        //take in prefab

        //check if there is a display in the pool 
        if (dataDisplay == null)
        {
            dataDisplay = new Dictionary<int, Queue<StatisticalDataDisplay>>();
        }

        StatisticalDataDisplay s_dataDisplay = null;

        if (dataDisplay.ContainsKey(prefab.GetInstanceID()))
        {
            if(dataDisplay[prefab.GetInstanceID()].Count > 0)
            {
                s_dataDisplay = dataDisplay[prefab.GetInstanceID()].Dequeue();
            }
        }

        if (s_dataDisplay == null)
        {
            s_dataDisplay = GameObject.Instantiate(prefab);

            Debug.Log("instantiated " + prefab.GetInstanceID());
        }

        s_dataDisplay.prefabId = prefab.GetInstanceID();
        s_dataDisplay.stationname.text = stationName;
        s_dataDisplay.transform.SetParent(parent);

        s_dataDisplay.transform.localPosition = Vector3.zero;
        s_dataDisplay.transform.rotation = parent.rotation;

        s_dataDisplay.Show();

        return s_dataDisplay;
    }

    public void SetDisplay(string stationName)
    {
        this.stationname.text = stationName;
    }

    public void Show()
    {
        //lerp to fade in another script
    }

    public void Hide()
    {
        //use canvas group
    }

    public void OnFullyHidden()
    {
        if(!dataDisplay.ContainsKey(prefabId))
        {
            dataDisplay.Add(prefabId, new Queue<StatisticalDataDisplay>());
        }

        dataDisplay[prefabId].Enqueue(this);
    }
}
