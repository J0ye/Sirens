using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Range(0f, 1000f)]
    public float turnSpeed = 100f;
    [Range(0f, 100f)]
    public float acceleration = 100f;
    [Range(0f, 1000f)]
    public float maxSpeed = 100f;
    [Range(0f, 10f)]
    public float driftValue = 2f;


    private Rigidbody2D rb;
    private float currentSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Get input
        float h = -Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate speed from input and acceleration (transform.up is forward)
        Vector2 speed = transform.up * (v * acceleration);
        rb.AddForce(speed);

        // Create car rotation
        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f)
        {
            rb.rotation += h * turnSpeed * (rb.velocity.magnitude / maxSpeed);
        }
        else
        {
            rb.rotation -= h * turnSpeed * (rb.velocity.magnitude / maxSpeed);
        }

        // Change velocity based on rotation
        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.left)) * driftValue;
        Vector2 relativeForce = Vector2.right * driftForce;
        Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green);
        rb.AddForce(rb.GetRelativeVector(relativeForce));

        // Force max speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        currentSpeed = rb.velocity.magnitude;
    }

    private void Turn(float val)
    {
        rb.rotation -= val * turnSpeed * (rb.velocity.magnitude / 5.0f);
    }
}
