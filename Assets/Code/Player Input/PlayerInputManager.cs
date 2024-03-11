using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 

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
    public InputState inputState = InputState.Default;

    [SerializeField] public int2 MouseGridPositon = new(0, 0);

    public bool isMouseOverUI = true;

    [SerializeField] public CameraController cameraController;

    PlayerInputActions inputActions;


    private void Awake()
    {
        inputActions = new PlayerInputActions();

        cameraController = GetComponent<CameraController>();
    }

    void OnEnable()
    {
        inputActions.DefaultControls.Enable();

        inputActions.ConstructionControls.Enable();
    }

    void OnDisable()
    {
        inputActions.DefaultControls.Disable();

        inputActions.ConstructionControls.Disable();
    }


    public enum InputState
    {
        Default,
        Menu,
        Construction,
        Cancel,
        Bulldoze
    }

    public void ChangeInputState(InputState newInputState)
    {
        inputState = newInputState;
    }

    private void Update()
    {
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (!isMouseOverUI)
        {
            UpdateMouseGridPosition();
        }

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

        if (inputActions.DefaultControls.PickStructure.WasPressedThisFrame())
        {
            Entity entity = GameManager.Instance.gameWorld.worldGrid.GetEntityAt(MouseGridPositon);

            if (entity != null)
            {
                if (entity.GetType() == typeof(StructureGhost))
                {
                    ChangeInputState(InputState.Construction);
                    GameManager.Instance.ConstructionManager.StartPlacingGhosts(((StructureGhost)entity).data);
                }
                if (entity.GetType().IsSubclassOf(typeof(Structure)))
                { 
                    ChangeInputState(InputState.Construction);
                    GameManager.Instance.ConstructionManager.StartPlacingGhosts(((Structure)entity).structureData);
                } 
            }
        }
    }

    public void HandleConstructionInput()
    {
        HandleCameraControl();

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ChangeInputState(InputState.Default);
            return;
        }

        ConstructionManager constructionManager = GameManager.Instance.ConstructionManager;

        if (isMouseOverUI != true)
        {
            constructionManager.DrawGhostAtMouse(MouseGridPositon);

            if (inputActions.ConstructionControls.PlaceGhost.IsPressed())
            {
                constructionManager.PlaceGhost(MouseGridPositon);
            }
        }

        if (inputActions.ConstructionControls.RotateGhost.WasPressedThisFrame())
        {
            constructionManager.RotateGhost(1);
        }  
    }


    public void UpdateMouseGridPosition()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Ray ray = cameraController.playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("MouseToGridPosition")))
        {
            mouseWorldPosition = hit.point;
            MouseGridPositon = new int2((int)Mathf.Round(mouseWorldPosition.x), (int)Mathf.Round(mouseWorldPosition.z));
        }
    }
}