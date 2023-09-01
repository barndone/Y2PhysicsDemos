using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantForceVolume : MonoBehaviour
{
    [SerializeField] float force = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var otherRB))
        {
            otherRB.AddForce(transform.forward * force, ForceMode.Force);
        }
    }
}
