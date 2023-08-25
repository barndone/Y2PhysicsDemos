using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMotor : MonoBehaviour
{
    [SerializeField] float verticalStrength = 1.0f;
    [SerializeField] float horizontalStrength = 1.0f;
    [SerializeField] float jumpDelay = 1.5f;
    private float jumpTimer = 0.0f;

    [SerializeField] Rigidbody rb;

    [SerializeField] Transform destination;
    [SerializeField] Vector3 destinationDirection;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool grounded;

    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out rb))
        {
            //  do nothing because we have a rb :D
            grounded = true;
        }

        else
        {
            Debug.LogError("No Rigidbody attached to " + this.name, this);
        }
    }

    private void Update()
    {
        if (grounded)
        {
            //  if our jump timer has surpassed our jump delay field
            if (jumpTimer >= jumpDelay)
            {
                destinationDirection = (destination.position - rb.position).normalized;
                jumpTimer = 0.0f;

                var force = new Vector3(destinationDirection.x * horizontalStrength, verticalStrength, destinationDirection.z * horizontalStrength);

                rb.AddForce(force, ForceMode.Impulse);
                grounded = false;
            }

            else
            {
                jumpTimer += Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var firstContact = collision.GetContact(0);
        var contactPoint = firstContact.point;

        var collisionAngle = Quaternion.Euler((contactPoint - rb.position).normalized);

        if (1 << firstContact.otherCollider.gameObject.layer == groundLayer)
        {

            Debug.DrawRay(firstContact.point, firstContact.normal * 5, Color.red, 5.0f);
            grounded = true;
        }
    }
}
