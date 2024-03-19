using Unity.Mathematics;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.VFX;
using System.Linq.Expressions;
using System;

public class CameraController : MonoBehaviour
{
    public Vector3 position;
    private Vector3 velocity;

    private float _zoom = 3;
    public float zoom
    {
        get { return _zoom; }
        set { _zoom = Mathf.Clamp(value, minZoom, maxZoom); }
    }

    static Quaternion isoCameraRotation = Quaternion.Euler(new Vector3(0, -45, 0));

    [Header("Movement")]
    [SerializeField] private float motionSmoothness = 10;
    [SerializeField] private float maxSpeed = 5;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float zoomMotionScaling = 0.5f;
    [Space]
    [SerializeField] private float minZoom = 2;
    [SerializeField] private float maxZoom = 15;

    [Space]
    [SerializeField] private float zoomPositionMultiplier = 3f;
    [SerializeField] private float zoomPositionOffset = 0.5f;

    private bool InIsometricView = true;
    [NonSerialized] public Camera activeMainCamera;

    [Header("Object References")]
    public GameObject cameraOrigin;

    public Camera isometricMainCamera;
    public Camera topDownMainCamera;

    public Camera isometricViewportCamera;
    public Camera topDownViewportCamera;

    PlayerInputActions InputActions;


    void OnEnable()
    {
        InputActions = new PlayerInputActions();
        InputActions.CameraControls.Enable();
    }
    void OnDisable()
    {
        InputActions.CameraControls.Disable();
    }

    private void Awake()
    {
        activeMainCamera = isometricMainCamera;
    }

    private void Update()
    {
        InputZoom();
        InputMove();

        UpdateCameraGridPosition();
    }

    public void SwapViews()
    {
        bool boolSwitch = isometricMainCamera.gameObject.activeSelf;

        InIsometricView = !boolSwitch;

        isometricMainCamera.gameObject.SetActive(!boolSwitch);
        topDownMainCamera.gameObject.SetActive(boolSwitch);

        isometricViewportCamera.gameObject.SetActive(boolSwitch);
        topDownViewportCamera.gameObject.SetActive(!boolSwitch);

        if (InIsometricView)
        {
            activeMainCamera = isometricMainCamera;
        }
        else
        {
            activeMainCamera = topDownMainCamera;
        }
    }

    void InputMove()
    {
        Vector2 inputVector = InputActions.CameraControls.Move.ReadValue<Vector2>();

        Vector3 inputVector3 = new Vector3(inputVector.x, 0, inputVector.y);

        velocity = Vector3.Lerp(velocity, inputVector3 * maxSpeed, motionSmoothness * Time.deltaTime);

        if (InIsometricView) { position += isoCameraRotation * (velocity * Time.deltaTime * (zoom * zoomMotionScaling)); }
        else { position += (velocity * Time.deltaTime * (zoom * zoomMotionScaling)); }

        cameraOrigin.transform.position = position;
    }

    void InputZoom()
    {
        zoom += InputActions.CameraControls.Zoom.ReadValue<float>() * (zoom / 10); //zoomSpeed;

        isometricMainCamera.orthographicSize = zoom;
        isometricViewportCamera.orthographicSize = zoom;

        topDownViewportCamera.orthographicSize = zoom + 1;
        topDownMainCamera.orthographicSize = zoom + 1;

        var zoomPos = Mathf.Clamp(-zoom * zoomPositionMultiplier - zoomPositionOffset, -1000, -5);

        isometricMainCamera.transform.localPosition = new Vector3(0, 0, zoomPos);
        isometricViewportCamera.transform.localPosition = new Vector3(0, 0, zoomPos);

        topDownViewportCamera.transform.localPosition = new Vector3(0, -zoomPos, 0);
        topDownMainCamera.transform.localPosition = new Vector3(0, -zoomPos, 0);
    }


    public (int2 xVisibleRange, int2 yVisibleRange) GetDiamondVisibleRange()
    {
        float2 cameraPosition = new float2(position.x, position.z);

        float cameraZoom = zoom;

        int xSize = (int)(8 * cameraZoom); // Need to include screen aspect ratio compensation;
        int ySize = (int)(8 * cameraZoom); // Currently defaulting to 16:9

        int2 xRangeOut = new int2((int)cameraPosition.x - xSize / 2, (int)cameraPosition.x + xSize / 2 + 1);
        int2 yRangeOut = new int2((int)cameraPosition.y - ySize / 2, (int)cameraPosition.y + ySize / 2 + 1);

        return (xRangeOut, yRangeOut);
    }

    public int2 GetLocalVisibleRange()
    {
        float cameraZoom = zoom;

        int Size = (int)(1.5 * cameraZoom + 2);

        return new int2(-Size, Size);
    }

    public int2 CameraGridPosition = new();

    public void UpdateCameraGridPosition()
    {
        Vector2 screenMiddle = new Vector2(isometricMainCamera.scaledPixelWidth / 2, isometricMainCamera.scaledPixelHeight / 2);

        Ray ray = isometricMainCamera.ScreenPointToRay(screenMiddle);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("MouseToGridPosition")))
        {
            Vector3 cameraWorldPosition = hit.point;
            CameraGridPosition = new int2((int)Mathf.Round(cameraWorldPosition.x), (int)Mathf.Round(cameraWorldPosition.z));
        }
    }

    // !! Deprecated !! //
    private float resistance = 1;
    void InputMove2()
    {
        float deltaResistance = resistance * maxSpeed * Time.deltaTime;
        float deltaAcceleration = motionSmoothness * maxSpeed * Time.deltaTime;

        // Apply "air" resistance
        if (velocity.magnitude - deltaResistance <= 0)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity = ShortenVector(velocity, deltaResistance);
        }

        Vector3 ShortenVector(Vector3 vector, float length)
        {
            Vector3 _vector = vector;
            _vector.Normalize();
            _vector *= length;
            return vector - _vector;
        }

        // Add player input
        Vector2 inputVector = InputActions.CameraControls.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized * deltaAcceleration;
        velocity += new Vector3(inputVector.x, 0, inputVector.y);

        // Clamp velocity between 0 & maximum velocity;
        velocity = ClampVectorMagnitude(velocity, 0, maxSpeed);

        Vector3 ClampVectorMagnitude(Vector3 vector, float min, float max)
        {
            if (vector.magnitude < max && vector.magnitude > min) { return vector; }
            Vector3 _vector = vector;
            if (_vector.magnitude < min)
            {
                _vector.Normalize();
                _vector *= min;
            }
            else if (_vector.magnitude > max)
            {
                _vector.Normalize();
                _vector *= max;
            }
            return _vector;
        }

        // Update camera position
        position += isoCameraRotation * (velocity / 100);
        cameraOrigin.transform.position = position;
    }
}