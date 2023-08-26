using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeMotor : MonoBehaviour
{
    [SerializeField] float verticalStrength = 1.0f;
    [SerializeField] float horizontalStrength = 1.0f;
    [SerializeField] float jumpDelay = 1.5f;
    private float jumpTimer = 0.0f;

    public Rigidbody rb;

    public Transform destination;
    [SerializeField] Vector3 destinationDirection;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool grounded;

    bool jumpWish = false;

    [SerializeField] int happinessNeeded = 10;
    int happinessTracker = 0;
    [SerializeField] float scaleIncrease = 1.5f;

    private bool levelUpWish = false;

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
            if (jumpTimer >= jumpDelay && !jumpWish)
            {
                jumpWish = true;
            }

            else
            {
                jumpTimer += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (jumpWish)
        {
            destinationDirection = (destination.position - rb.position).normalized;
            jumpTimer = 0.0f;

            var force = new Vector3(destinationDirection.x * horizontalStrength, verticalStrength, destinationDirection.z * horizontalStrength);

            rb.AddForce(force, ForceMode.Impulse);
            grounded = false;
            jumpWish = false;
        }

        if (levelUpWish)
        {
            rb.AddForce(Vector3.up * verticalStrength, ForceMode.Impulse);
            rb.gameObject.transform.localScale = rb.gameObject.transform.localScale * scaleIncrease;
            happinessTracker = 0;
            levelUpWish = false;
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

    public void ApplyHappiness(int value)
    {
        happinessTracker += value;

        if (happinessTracker >= happinessNeeded)
        {
            levelUpWish = true;
        }
    }
}
