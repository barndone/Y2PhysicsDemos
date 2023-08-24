using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    [SerializeField] List<SimpleRigidbody> simpleRigidbodies = new List<SimpleRigidbody>();
    [SerializeField] Vector3 impulseVelcity;

    // Start is called before the first frame update
    void Start()
    {
        if (simpleRigidbodies.Count != 0)
        {
            foreach (var rb in simpleRigidbodies)
            {
                rb.velocity = impulseVelcity;
            }
        }

        else
        {
            var rb = GetComponent<SimpleRigidbody>();
            rb.velocity = impulseVelcity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
