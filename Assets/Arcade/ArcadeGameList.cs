using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeGameList : MonoBehaviour
{
    ArcadeManager arcadeManager;

    [SerializeField] List<GameObject> pointers;
    int pointerIndex = 0;

    InputAction ia_up;
    InputAction ia_down;
    InputAction ia_select;

    private void Awake()
    {
        arcadeManager = GetComponentInParent<ArcadeManager>();
        foreach (GameObject p in pointers)
        {
            p.SetActive(false);
        }
        pointers[pointerIndex].SetActive(true);
         
        ia_up = arcadeManager.playerInputActions.Arcade.Up;
        ia_down = arcadeManager.playerInputActions.Arcade.Down;
        ia_select = arcadeManager.playerInputActions.Arcade.Action;
    }

    private void Update()
    {
        if (ia_up.WasPressedThisFrame())
        {
            ChangePointer(pointerIndex - 1);
        }
        if (ia_down.WasPressedThisFrame())
        {
            ChangePointer(pointerIndex + 1);
        }
        if (ia_select.WasPressedThisFrame())
        {
            arcadeManager.LoadGame(pointerIndex);
            gameObject.SetActive(false);
        }
    }

    void ChangePointer(int index)
    {
        foreach (GameObject p in pointers)
        {
            p.SetActive(false);
        }
        pointerIndex = Mathf.Clamp(index, 0, pointers.Count - 1);
        pointers[pointerIndex].SetActive(true);
    }
}
