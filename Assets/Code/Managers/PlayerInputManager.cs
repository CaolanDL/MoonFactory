using MoonFactory.Interfaces;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems; 

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
    [SerializeField] public double2 MouseWorldPositon = new(0, 0);

    public bool isMouseOverUI = true;

    [SerializeField] public CameraController cameraController;

    public PlayerInputActions inputActions; 

    private void Awake()
    {
        inputActions = new PlayerInputActions(); 

        cameraController = GetComponent<CameraController>();
    }

    void OnEnable()
    {
        inputActions.DefaultControls.Enable();
        inputActions.CameraControls.Enable();
        inputActions.UIControls.Enable();
        inputActions.HUDControls.Enable();
    }

    public enum InputState
    {
        Default,
        Menu,
        Construction,
        Cancel,
        Demolish
    }

    public void ChangeInputState(InputState newInputState)
    {
        inputState = newInputState;

        if(newInputState == InputState.Construction)
        {
            AudioManager.Instance.PlaySound(AudioData.Instance.UI_BuildButton);
        }
    }

    private void Update()
    {
        isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (GameManager.Instance.GameWorld == null) return;

        HandleCameraMove(); 
        HandleHUDInputs();

        if (!isMouseOverUI)
        {
            HandleCameraZoom();
            UpdateSpatialMousePosition();
        }

        switch (inputState)
        {
            default:
                break;

            case InputState.Default:
                HandleDefaultInput();
                break;

            case InputState.Construction:
                HandleConstructionInput();
                break; 

            case InputState.Demolish:
                HandleDemolishInput();
                break;
        }
    }

    public void GotoDefaultInputState()
    {
        GameManager.Instance.HUDManager.MouseIconManager.SetActiveIcon(MouseIconManager.Icon.None);
        ChangeInputState(InputState.Default);
    }

    public void HandleCameraMove()
    {
        cameraController.InputMove();
    }

    public void HandleCameraZoom()
    {
        cameraController.InputZoom();
    }

    public void HandleHUDInputs()
    {
        var HUDManager = GameManager.Instance.HUDManager;

        if (inputActions.HUDControls.OptionsMenu.WasPressedThisFrame())
        {
            HUDManager.ToggleOptionsMenu();
        }

        if (inputActions.HUDControls.BuildMenu.WasPressedThisFrame())
        {
            HUDManager.ToggleBuildMenu();
        }

        if (inputActions.HUDControls.ScienceMenu.WasPressedThisFrame())
        {
            HUDManager.ToggleScienceMenu();
        }

        if (inputActions.HUDControls.Demolish.WasPressedThisFrame() && HUDManager.ConstructionMenu.activeSelf == true)
        {
            HUDManager.BulldozeButtonPressed();
        }
    } 

    public void HandleDefaultInput()
    { 
        if (inputActions.DefaultControls.Pick.WasPressedThisFrame())
        {
            Entity entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(MouseGridPositon);

            if (entity != null)
            {
                if (entity.GetType() == typeof(StructureGhost))
                {
                    ChangeInputState(InputState.Construction);
                    GameManager.Instance.ConstructionManager.StartPlacingGhosts(((StructureGhost)entity).structureData);
                }
                if (entity.GetType().IsSubclassOf(typeof(Structure)))
                {
                    ChangeInputState(InputState.Construction);
                    GameManager.Instance.ConstructionManager.StartPlacingGhosts(((Structure)entity).StructureData);
                }
            }

            return;
        }

        Ray ray = cameraController.activeMainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Rover")))
        {
            if (isMouseOverUI) { return; }

            Entity entity = hit.transform.gameObject.GetComponent<DisplayObject>().parentEntity;  
            var rover = ((Rover)entity);

            rover.RenderSelectionOutline();

            if (inputActions.DefaultControls.Select.WasPressedThisFrame())
            {  
                rover.Clicked(Input.mousePosition); 
            }
        }
        else if (inputActions.DefaultControls.Select.WasPressedThisFrame())
        {
            if (isMouseOverUI) { return; } 
             
            Entity entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(MouseGridPositon);

            if (entity != null)
            {
                if (entity.GetType().IsSubclassOf(typeof(Structure)))
                {
                    ((Structure)entity).Clicked(Input.mousePosition);
                }
            } 
             
            return;
        }

        RenderSelectGizmo();

        void RenderSelectGizmo()
        {
            if(isMouseOverUI) { return; }
            if(GameManager.Instance.GameWorld == null) { return; }

            var entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(MouseGridPositon);
            if (entity != null) { entity.RenderSelectionOutline(); }
        } 
    }

    public void HandleConstructionInput()
    { 
        RenderGizmoAtMouseTile();

        GameManager.Instance.HUDManager.MouseIconManager.SetActiveIcon(MouseIconManager.Icon.Build);

        ConstructionManager constructionManager = GameManager.Instance.ConstructionManager; 

        if (inputActions.DefaultControls.Rotate.WasPressedThisFrame())
        {
            constructionManager.RotateGhost(1);
        }

        if (isMouseOverUI != true)
        {
            constructionManager.DrawGhostAtMouse(); 

            if (inputActions.DefaultControls.Select.IsPressed())
            {
                constructionManager.PlaceGhost(MouseWorldPositon);
            }
        } 

        if (inputActions.DefaultControls.ExitTool.WasPressedThisFrame())
        {
            GotoDefaultInputState();
            return;
        }
    }

    Entity lastEntity = null;

    public void HandleDemolishInput()
    {
        GameManager.Instance.HUDManager.MouseIconManager.SetActiveIcon(MouseIconManager.Icon.Cancel);

        RenderGizmoAtMouseTile();

        if (inputActions.DefaultControls.Select.IsPressed())
        {
            Entity entity = GameManager.Instance.GameWorld.worldGrid.GetEntityAt(MouseGridPositon);

            if(entity == lastEntity) { return; }

            if (entity != null)
            {
                if (entity.GetType() == typeof(StructureGhost))
                {
                    ((StructureGhost)entity).Cancel();
                    PlayCancelSound();
                }
                else if (entity.GetType().IsSubclassOf(typeof(Structure)))
                {
                    var structure = (Structure)entity;

                    if(structure.CanDemolish() == false) return;  

                    if (DevFlags.InstantBuilding)
                    {
                        structure.Demolish();
                        PlayDemolishSound();
                        return;
                    }

                    if (structure.flaggedForDemolition != true)
                    {
                        structure.FlagForDemolition();
                        PlayDemolishSound();
                    }
                    else structure.CancelDemolition(); 
                }
                else if(entity is IDemolishable)
                {
                    var demolishable = (IDemolishable)entity;
                    demolishable.ToggleDemolition();
                    PlayDemolishSound();
                }
            }

            lastEntity = entity;

            return;

            void PlayDemolishSound() => AudioManager.Instance.PlaySound(AudioData.Instance.Tool_Bulldoze, 1f);
            void PlayCancelSound() => AudioManager.Instance.PlaySound(AudioData.Instance.Tool_DeleteGhost, 1f);
        }
        if (inputActions.DefaultControls.Select.WasReleasedThisFrame())
        {
            lastEntity = null;
        }

        if (inputActions.DefaultControls.ExitTool.WasPressedThisFrame())
        {
            GotoDefaultInputState();
            return;
        }
    } 

    public void UpdateSpatialMousePosition()
    { 
        Ray ray = cameraController.activeMainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("MouseToGridPosition")))
        {
            Vector3 mouseWorldPosition = hit.point;
            MouseWorldPositon = new double2(mouseWorldPosition.x, mouseWorldPosition.z);
            MouseGridPositon = new int2((int)Mathf.Round(mouseWorldPosition.x), (int)Mathf.Round(mouseWorldPosition.z));
        }
    }

    public void RenderGizmoAtMouseTile()
    {
        Graphics.DrawMesh(GlobalData.Instance.gizmo_TileGrid, new TinyTransform(MouseGridPositon, 0).ToMatrix(), GlobalData.Instance.mat_PulsingGizmo, 0);
    }
}