using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

#region Input Example Comment
// Input Example:
// Player moves camera to a location with WASD or rightmouse drag
// Player opens the build menu and selects a building
// A ghost model is rendered at the grid location under the mouse
// If the location is occupied the ghost is red
// If the location is available the ghost resumes its default colour
// If the player left clicks a ghostStructure is placed (GhostStructure adds itself to construction manager on instanciation)
// If the player presses R or ScrollWheel the ghost rotates
// WASD still controls the camera, but right click no longer does.
// if the player right clicks the action is cancelled and control reverts back to normal.

// Suggested Advanced Implementation:
// Player input state is managed as a stack of inputhandlers
// Input handlers define the effects of InputActions
// Each frame a queue of player iputs is created
//     The InputHandler stack is iterated through calling the relevent inputs, if an input succeedes it is removed from the queue 
#endregion


public class PlayerInputManager : MonoBehaviour
{ 
    public InputState inputState = InputState.Construction;

    public int2 MouseGridPositon = new(0,0);

    public CameraController cameraController;


    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }

    public enum InputState
    { 
        Default,
        Menu,
        Construction,
        Cancel,
        Bulldoze
    }

    private void Update()
    {
        UpdateMouseGridPosition();

        switch (inputState)
        {
            default:
                break; 

            case InputState.Default:
                HandleDefaultInput();
                break;

            case InputState.Menu:
                break;

            case InputState.Construction:
                HandleConstructionInput();
                break;

            case InputState.Cancel:
                break;

            case InputState.Bulldoze:
                break;
        }
    }

    public void HandleCameraControl()
    {

    }

    public void HandleDefaultInput()
    {
        HandleCameraControl();

        
    }  

    public void HandleConstructionInput()
    {
        HandleCameraControl();

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            //GameManager.Instance.constructionManager.PlaceGhost(MouseGridPositon);
        }
    } 


    public void UpdateMouseGridPosition()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Ray ray = cameraController.playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, 6)) //Layer Mask: MouseToGridPosition
        {
            mouseWorldPosition = hit.point;
            MouseGridPositon = new int2((int)Mathf.Round(mouseWorldPosition.x), (int)Mathf.Round(mouseWorldPosition.z));
        }
    }
}