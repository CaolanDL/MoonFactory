using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScienceBanner : MonoBehaviour
{
    [SerializeField] TMP_Text PointsText;

    [SerializeField] Color ZeroColor;
    [SerializeField] Color PositiveColor;

    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        ScienceManager.SciencePointsChanged += SetPoints;
        ScienceManager.ResearchCompleted += PlayAnimation;
    }

    void UnsubscribeFromEvents()
    {
        ScienceManager.SciencePointsChanged -= SetPoints;
        ScienceManager.ResearchCompleted -= PlayAnimation;
    }

    void SetPoints(int points)
    {
        if(points > 0) PointsText.color = PositiveColor; 
        else PointsText.color = ZeroColor; 

        PointsText.text = points.ToString();
    }

    void PlayAnimation(ResourceData r, int p)
    {
        var pulse = PointsText.gameObject.AddComponent<GraphicsPulseOnce>();
        pulse.Set(1.2f, 12f); 
    }
}
