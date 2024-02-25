using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float2 position;
    public float2 worldPosition;

    private float _zoom = 2;
    public float zoom
    {
        get { return _zoom; }
        set { _zoom = Mathf.Clamp(value, 1, 12); }
    }

    [Header("Variables")]

    [SerializeField] private int moveSpeed = 16;
    [SerializeField] private float zoomSpeed = 1;

    [Header("Object References")]

    public GameObject cameraOrigin;
    public Camera playerCamera;

    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.CameraControls.Enable();
    } 
    void OnDisable()
    {
        inputActions.CameraControls.Disable();
    }


    private void Update()
    {
        InputMove();
        InputZoom();
    }

    private Vector2 velocity;
    private int resistance = 1;
    private int maxVelocity = 5;

    void InputMove()
    {
        Vector2 inputVector = inputActions.CameraControls.WASD.ReadValue<Vector2>();

        inputVector = inputVector.normalized * moveSpeed * Time.deltaTime;

        velocity += inputVector;

        velocity = velocity;

        position +=  (float2) inputVector;

        Vector3 worldVector = Quaternion.Euler(new Vector3(0, 45, 0)) * new Vector3(position.x, 0, position.y);

        worldPosition = new(worldVector.x, worldVector.z);

        cameraOrigin.transform.position = worldVector;
    }

    void InputZoom()
    {
        zoom += inputActions.CameraControls.Zoom.ReadValue<float>() * (zoom / 10); //zoomSpeed;
        playerCamera.orthographicSize = zoom;
    }
}
