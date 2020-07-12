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
        bikescore.text = "# Times Not Enough Bikes: " + NoBikes;
        dockscore.text = "# Times Not Enough Docks: " + NoDocks;
    }
}
