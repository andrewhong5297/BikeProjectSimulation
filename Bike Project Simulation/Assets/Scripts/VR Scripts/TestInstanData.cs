using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstanData : MonoBehaviour
{
    [SerializeField]
    private string stationname;

    [SerializeField]
    private StatisticalDataDisplay s_dataDisplayPrefab = null;

    private StatisticalDataDisplay dataDisplay = null;

    public void ShowData()
    {
        if (dataDisplay == null)
        {
            dataDisplay = StatisticalDataDisplay.Create(s_dataDisplayPrefab, stationname, this.transform);
        }

        else
        {
            dataDisplay.SetDisplay(stationname);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowData();
        }
    }
}
