using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody;

    [SerializeField] float startSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody.AddForce(Vector2.left * startSpeed);
    }
}
