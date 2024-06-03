using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController3D : MonoBehaviour
{
    [SerializeField] public Camera activeMainCamera;
    [SerializeField] public VolumeProfile postProcProfile;
    [SerializeField] public DepthOfField dof;

    [SerializeField] int renderDistance = 20;

    [Space]
    [SerializeField] Transform OrbitOrigin;
    [SerializeField] private float motionSmoothness = 10;
    [SerializeField] private float maxSpeed = 5;
    public Vector3 position;
    private Vector3 velocity;
    public int2 CameraGridPosition => new(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

    [Space]
    [SerializeField] Transform OrbitSpin;
    [SerializeField] public float spinSpeed = 0.32f;
    private float _spin = -45;
    public float spin
    {
        get { return _spin; }
        set { _spin = (value % 360 + 360) % 360; } // Wrap spin around 360 degrees
    }

    [Space]
    [SerializeField] Transform OrbitPitch;
    [SerializeField] public float pitchSpeed = 0.2f;
    [SerializeField] private float minPitch = 18;
    [SerializeField] private float maxPitch = 80;
    private float _pitch = 32;
    public float pitch
    {
        get { return _pitch; }
        set { _pitch = Mathf.Clamp(value, minPitch + (zoom / 2), maxPitch); } // Arc the min tilt to compensate for zoom out
    }

    [Space]
    [SerializeField] Transform OrbitOffset;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2;
    [SerializeField] private float maxZoom = 15;
    [SerializeField] private float _zoom = 8;
    public float zoom
    {
        get { return _zoom; }
        set { _zoom = Mathf.Clamp(value, minZoom, maxZoom); }
    }
    private float zoomVelocity;
    [SerializeField] private float _targetZoom = 8;
    public float targetZoom
    {
        get { return _targetZoom; }
        set { _targetZoom = Mathf.Clamp(value, minZoom, maxZoom); }
    }

    private void Awake()
    {
        postProcProfile.TryGet<DepthOfField>(out dof);
    }

    private void LateUpdate()
    { 
        OrbitSpin.localRotation = Quaternion.Euler(0, spin, 0); // Set camera spin
        OrbitPitch.localRotation = Quaternion.Euler(pitch, 0, 0); // Set camera pitch

        zoom = Mathf.SmoothDamp(zoom, targetZoom, ref zoomVelocity, 0.1f, 36f);
        OrbitOffset.localPosition = new Vector3(0, 0, -zoom); // Set Zoom
        dof.focusDistance.value = zoom;

        OrbitOrigin.position = position; // Set Camera Position
    }

    public void InputPan(Vector2 input)
    { 
        spin += input.x * spinSpeed;  
        pitch -= input.y * pitchSpeed; 
    }

    public void InputMove(Vector2 input)
    { 
        Vector3 inputVector3 = new Vector3(input.x, 0, input.y);

        inputVector3 = Quaternion.Euler(0, spin, 0) * inputVector3;

        velocity = Vector3.Lerp(velocity, inputVector3 * maxSpeed, motionSmoothness * Time.deltaTime);

        position += (velocity * Time.deltaTime * zoom); 
    }

    public void InputZoom(float input)
    {
        targetZoom += input * zoomSpeed; //zoomSpeed;    
    }

    public void ResetPosition()
    {
        position = Vector3.zero;
    }

    public int2 GetLocalVisibleRange()
    { 
        int Size = (int)(1.5f * renderDistance + 2); 
        return new int2(-Size, Size);
    }
}
