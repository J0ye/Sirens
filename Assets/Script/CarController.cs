using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Driving Settings")]
    [Range(0f, 1000f)]
    public float turnSpeed = 100f;
    [Range(0f, 100f)]
    public float acceleration = 100f;
    [Range(0f, 1000f)]
    public float maxSpeed = 100f;
    [Range(0f, 10f)]
    public float driftValue = 2f;
    [Range(0f, 10f)]
    public float driftDrag = 2f;
    public bool enableControls = true;

    [Header("Lights")]
    public List<Light2D> frontLights = new List<Light2D>();
    public List<Light2D> breakeLights = new List<Light2D>();
    public List<Light2D> sirens = new List<Light2D>();
    [Range(0f, 1f)]
    public float sirenSpeed = 0.1f;
    public AudioSource siren;
    private bool sirenReady = true;



    private Rigidbody2D rb;
    private float currentSpeed;
    private float startDrag;
    private float sirenIntensity;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startDrag = rb.drag;
        SetUpLights();
    }

    private void Update()
    {
        #region light controls
        if (Input.GetAxis("Vertical") < 0)
        {
            // Player is engaging the breaks
            ChangeLights(breakeLights, true);
        }
        else
        {
            ChangeLights(breakeLights, false);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeLights(frontLights, !frontLights[0].enabled);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeLights(sirens, !sirens[0].enabled);
            if (sirens[0].enabled)
                siren.Play();
            else siren.Stop();
        }

        SirenEffect();
        #endregion
    }

    private void FixedUpdate()
    {
        // Get input
        float h = -Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        #region car movement
        // Calculate speed from input and acceleration (transform.up is forward)
        Vector2 speed = transform.up * (v * acceleration);
        rb.AddForce(speed);
        #endregion

        #region car rotation
        float direction = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.up));
        if (direction >= 0.0f)
        {
            rb.rotation += h * turnSpeed * (rb.velocity.magnitude / maxSpeed);
        }
        else
        {
            rb.rotation -= h * turnSpeed * (rb.velocity.magnitude / maxSpeed);
        }
        #endregion

        #region drifting
        float drift = driftValue;
        if (Input.GetKey(KeyCode.Space))
        {
            drift = 0.2f;
            rb.drag += driftDrag/10 * Time.deltaTime * rb.velocity.magnitude;
            rb.drag = Mathf.Clamp(rb.drag, 0f, driftDrag);
        }else
        {
            rb.drag = startDrag;
        } 

        // Change velocity based on rotation
        float driftForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.left)) * drift;
        Vector2 relativeForce = Vector2.right * driftForce;
        Debug.DrawLine(rb.position, rb.GetRelativePoint(relativeForce), Color.green);
        rb.AddForce(rb.GetRelativeVector(relativeForce));
        #endregion

        // Force max speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        currentSpeed = rb.velocity.magnitude;        
    }

    private void SetUpLights()
    {
        if (sirens.Count > 0) sirenIntensity = sirens[0].intensity;
        ChangeLights(sirens, false);
        ChangeLights(breakeLights, false);
        sirens[1].intensity = 0;

    }

    private void ChangeLights(List<Light2D> targets, bool val)
    {
        foreach(Light2D lig in targets)
        {
            lig.enabled = val;
        }
    }

    private void SirenEffect()
    {
        if (sirens.Count < 2 || !sirenReady) return; // Dont execute if the sirens are not correctly set up

        if(sirens[0].intensity > 0)
        {
            // Turn 0 off and 1 back on
            sirens[0].intensity = 0;
            sirens[1].intensity = sirenIntensity;
        } else
        {
            // turn 1 off and 0 back on
            sirens[1].intensity = 0;
            sirens[0].intensity = sirenIntensity;
        }
        StartCoroutine(SirenPause());
    }

    private IEnumerator SirenPause()
    {
        sirenReady = false;

        yield return new WaitForSeconds(sirenSpeed);

        sirenReady = true;
    }
}
