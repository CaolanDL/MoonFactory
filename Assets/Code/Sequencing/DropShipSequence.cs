using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropShipSequence : MonoBehaviour
{
    public int2 spawnPoint;

    private void Awake()
    {
        GameManager.Instance.HUDManager.ToggleScienceMenu();
    }

    public void Finished()
    {
        RoverManager.Instance.SpawnNewWidget(spawnPoint);
        Destroy(gameObject);
    }
}
