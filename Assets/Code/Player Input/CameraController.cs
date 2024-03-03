using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Vector3 position;
    private Vector3 velocity;

    private float _zoom = 2;
    public float zoom
    {
        get { return _zoom; }
        set { _zoom = Mathf.Clamp(value, 1, 12); }
    }

    static Quaternion cameraRotation = Quaternion.Euler(new Vector3(0, 45, 0));

    [Header("Variables")]
    [SerializeField] private float motionSmoothness = 10;
    [SerializeField] private float maxSpeed = 5; 
    [SerializeField] private float zoomSpeed = 10;
    [SerializeField] private float zoomScaling = 0.5f;

    [Header("Object References")]
    public GameObject cameraOrigin;
    public Camera playerCamera;
    public Camera topDownCamera;


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

    private void Update()
    {
        InputZoom();
        InputMove(); 
    }

    void InputMove()
    {
        Vector2 inputVector = InputActions.CameraControls.Move.ReadValue<Vector2>();

        Vector3 inputVector3 = new Vector3(-inputVector.y, 0, inputVector.x);

        velocity = Vector3.Lerp(velocity, inputVector3 * maxSpeed, motionSmoothness * Time.deltaTime); 

        position += cameraRotation * (velocity * Time.deltaTime * (zoom * zoomScaling));
        cameraOrigin.transform.position = position;
    } 

    void InputZoom()
    {
        zoom += InputActions.CameraControls.Zoom.ReadValue<float>() * (zoom / 10); //zoomSpeed;

        playerCamera.orthographicSize = zoom;
        topDownCamera.orthographicSize = zoom;
        playerCamera.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(-zoom*2, -1000, -5));
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
        position += cameraRotation * (velocity/100);
        cameraOrigin.transform.position = position;
    }
}