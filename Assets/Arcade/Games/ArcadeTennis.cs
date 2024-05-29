using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class ArcadeTennis : MonoBehaviour
{
    ArcadeManager arcadeManager;
    public PlayerInputActions playerInputActions;
    public ArcadeActions arcadeActions;

    InputAction ia_up;
    InputAction ia_down; 

    
    private void Awake()
    {
        arcadeManager = GetComponentInParent<ArcadeManager>();  
        
        ia_up = arcadeManager.playerInputActions.Arcade.Up;
        ia_down = arcadeManager.playerInputActions.Arcade.Down; 

        initalypos = playerPaddle.position.y;
    }

    float initalypos = 0;
    [SerializeField] Rigidbody2D playerPaddle;
    [SerializeField] float paddleRange = 2f;
    [SerializeField] float paddleSpeed = 100f;

    private void Update()
    {
        float input = 0;
        if(ia_up.IsPressed()) input = Time.deltaTime * paddleSpeed;
        if(ia_down.IsPressed()) input = -Time.deltaTime * paddleSpeed;

        var newPosition = Mathf.Clamp(playerPaddle.position.y + input, -paddleRange + initalypos, paddleRange + initalypos);
        playerPaddle.MovePosition(new(playerPaddle.position.x, newPosition));
    }
}
