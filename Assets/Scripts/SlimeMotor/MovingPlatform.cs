using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints = new List<Transform> ();

    [SerializeField] int activeWP = 0;
    Rigidbody rb;

    public enum Behavior
    { 
        Once,
        Reverse,
        Loop,
        Repeat,
        PingPong
    }

    [SerializeField] float speed = 1.0f;

    public bool movin = true;

    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out rb))
        {
            //  do nothing because we have a rb :D
        }

        else
        {
            Debug.LogError("No Rigidbody attached to " + this.name, this);
        }
    }

    private void FixedUpdate()
    {
        if (movin)
        {
            if ((waypoints[activeWP].position - rb.position).sqrMagnitude <= 0.001f)
            {
                activeWP++;
                if (activeWP >= waypoints.Count)
                {
                    movin = false;
                    return;
                }

                else
                {
                    var dir = (waypoints[activeWP].position - rb.position).normalized;
                    rb.MovePosition(rb.position + dir * Time.deltaTime * speed);
                }
            }

            else
            {
                var dir = (waypoints[activeWP].position - rb.position).normalized;
                rb.MovePosition(rb.position + dir * Time.deltaTime * speed);
            }
        }

    }
}
