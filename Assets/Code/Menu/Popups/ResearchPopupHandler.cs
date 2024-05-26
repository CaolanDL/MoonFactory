using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResearchPopupHandler : PopupHandler
{
    [SerializeField] TMP_Text SciencePointCount;

    public void SetSciencePoints(int points)
    {
        SciencePointCount.text = "+"+points.ToString();
    }
}
