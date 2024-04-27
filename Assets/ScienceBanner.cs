using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScienceBanner : MonoBehaviour
{
    [SerializeField] TMP_Text PointsText;

    private void Awake()
    {
        GameManager.Instance.ScienceManager.PointsAdded += SetPoints;
    }

    void SetPoints(int points)
    {
        PointsText.text = points.ToString();
    }
}
