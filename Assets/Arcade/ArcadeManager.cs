using System.Collections.Generic;
using UnityEngine;
using static PlayerInputActions;

public class ArcadeManager : MonoBehaviour
{
    [SerializeField] List<GameObject> Games;

    public PlayerInputActions playerInputActions;
    public ArcadeActions arcadeActions;

    private void Awake()
    {
        if (GameManager.Instance != null) playerInputActions = GameManager.Instance.PlayerInputManager.inputActions;
        else playerInputActions = new(); 

        arcadeActions = playerInputActions.Arcade;
        arcadeActions.Enable();

        ShowControls(true);
    }

    [SerializeField] GameObject controls;
    bool controlsShown = false;
    void ShowControls(bool shown)
    {
        controlsShown = shown;
        controls.SetActive(shown);
    }

    [SerializeField] GameObject gameList;
    void ShowGameList(bool shown)
    {
        gameList.SetActive(shown);
    } 

    public void LoadGame(int index)
    {
        Instantiate(Games[index], transform);
    }

    private void Update()
    {
        if (controlsShown)
        { 
            if (arcadeActions.Down.WasPressedThisFrame() || arcadeActions.Up.WasPressedThisFrame() || arcadeActions.Right.WasPressedThisFrame() || arcadeActions.Left.WasPressedThisFrame() || arcadeActions.Action.WasPressedThisFrame())
            {
                ShowControls(false);
                ShowGameList(true);
            }
        }
    }
}
