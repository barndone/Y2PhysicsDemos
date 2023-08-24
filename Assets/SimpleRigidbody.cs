using System;
using UnityEngine;

public class SimpleRigidbody : MonoBehaviour
{
    public Vector3 velocity;

    public bool useGravity;

    public enum UpdateMode
    {
        None,
        Update,
        FixedUpdate
    }
    public UpdateMode updateMode;

    private void Update()
    {
        // early exit if we aren't using this UpdateMode
        if (updateMode != UpdateMode.Update) { return; }

        UpdateVelocity();
        UpdatePosition();
    }

    private void FixedUpdate()
    {
        // early exit if we aren't using this UpdateMode
        if (updateMode != UpdateMode.FixedUpdate) { return; }

        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        // integrate gravitational forces into velocity
        if (useGravity)
        {
            // apply the acceleration of gravity
            velocity += Physics.gravity * Time.deltaTime;
        }
    }

    private void UpdatePosition()
    {
        // integrate velocity into position
        transform.position += velocity * Time.deltaTime;
    }
}