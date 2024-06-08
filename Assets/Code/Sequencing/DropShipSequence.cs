using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropShipSequence : MonoBehaviour
{
    public int2 spawnPoint;

    private void Awake()
    {
        GameManager.OnGameExit += DestroySelf; 
        GameManager.Instance.HUDManager.ToggleScienceMenu();
    }

    private void OnDestroy() => GameManager.OnGameExit -= DestroySelf; 

    void DestroySelf() => Destroy(gameObject); 

    public void Finished()
    {
        RoverManager.Instance.SpawnNewWidget(spawnPoint);
        Destroy(gameObject);
    }
}
