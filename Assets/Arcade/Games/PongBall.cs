using ExtensionMethods;
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
        var force = (Vector2.left * startSpeed).Rotate((Random.value - 0.5f) * 25f);
        rigidbody.AddForce(force);
    }
}
