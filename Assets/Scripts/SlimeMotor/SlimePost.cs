using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePost : MonoBehaviour
{
    [SerializeField] int happinessDelta = 1;
    [SerializeField] LayerMask slimeMask;

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == slimeMask)
        {
            SlimeMotor slime = other.gameObject.GetComponent<SlimeMotor>();

            slime.destination.position = transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (1 << collision.gameObject.layer == slimeMask)
        {
            SlimeMotor slime = collision.gameObject.GetComponent<SlimeMotor>();
            slime.ApplyHappiness(happinessDelta);
        }
    }
}
