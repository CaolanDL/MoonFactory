using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScienceBanner : MonoBehaviour
{
    [SerializeField] TMP_Text PointsText;

    private void Awake()
    {
        GameManager.Instance.ScienceManager.PointsChanged += SetPoints;
        GameManager.Instance.ScienceManager.ResearchCompleted += PlayAnimation;
    }

    void SetPoints(int points)
    {
        PointsText.text = points.ToString();
    }

    void PlayAnimation()
    {
        var pulse = PointsText.gameObject.AddComponent<GraphicsPulseOnce>();
        pulse.Set(1.2f, 12f);

    }
}
