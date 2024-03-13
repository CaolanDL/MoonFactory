using Logistics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class DebugUI : MonoBehaviour
{
    [SerializeField] GameObject VerticalLayout;
    [SerializeField] GameObject TextLinePrefab;

    int FPS;
    TextMeshProUGUI FPSCounter;

    int itemCount;
    TextMeshProUGUI ItemCounter;

    TextMeshProUGUI RoverCounter;

    private void Start()
    {
        FPSCounter = NewTextLine();
        ItemCounter = NewTextLine();
        RoverCounter = NewTextLine(); 
    }

    TextMeshProUGUI NewTextLine() { return Instantiate(TextLinePrefab, VerticalLayout.transform).GetComponent<TextMeshProUGUI>(); }

    private void Update()
    {
        FPS = (int)(1.0f / Time.smoothDeltaTime);
    }

    private void FixedUpdate()
    {
        FPSCounter.text = $"FPS {FPS}";

        itemCount = 0;
        foreach (Chain chain in ChainManager.chains) itemCount += chain.items.Count;
        ItemCounter.text = $"Items: {itemCount}";
    }
}
