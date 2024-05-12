using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPositionInvoker : MonoBehaviour
{
    [SerializeField] TutorialTag TutorialTag;

    // Update is called once per frame
    void Update()
    {
        if (TutorialProxy.IsActive)
        {
            TutorialProxy.SetPopupPosition?.Invoke(transform.position, TutorialTag);
        }
    }
}
