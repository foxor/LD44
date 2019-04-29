using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Vector2 Velocity;

    protected Rigidbody2D RB;
    public void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 30f);
    }

    public void FixedUpdate()
    {
        RB.position += Velocity * Time.fixedDeltaTime;
    }
}
