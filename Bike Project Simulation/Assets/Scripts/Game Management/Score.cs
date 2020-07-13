using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int NoBikes = 0;
    public int NoDocks = 0;

    public TextMeshProUGUI bikescore;
    public TextMeshProUGUI dockscore;

    private void Update()
    {
        bikescore.text = "Not Enough Bikes: " + NoBikes;
        dockscore.text = "Not Enough Docks: " + NoDocks;
    }
}
