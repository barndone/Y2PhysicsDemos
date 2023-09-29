using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    [SerializeField] float openedAngleLimit = 75.0f;
    [SerializeField] HingeJoint leftDoor;
    [SerializeField] HingeJoint rightDoor;

    [SerializeField] GameObject additionalLocks;

    [SerializeField] List<SlimeMotor> slimesInRange = new List<SlimeMotor>();

    [SerializeField] float explosiveForce = 5.0f;
    [SerializeField] float explosionRadius = 2.0f;
    [SerializeField] float upwardExplosionForce = 4.0f;

    [SerializeField] int damage = 4;

    // handle changing the limits on the doors after the lock is broken
    public void UnlockDoor()
    {
        if (leftDoor != null)
        {
            var limit = leftDoor.limits;
            limit.min = -openedAngleLimit;
            limit.max = openedAngleLimit;
            leftDoor.limits = limit;
        }

        if (rightDoor != null)
        {
            var limit = rightDoor.limits;
            limit.min = -openedAngleLimit;
            limit.max = openedAngleLimit;
            rightDoor.limits = limit;
        }

        foreach (var slimeMotor in slimesInRange)
        {
            if (slimeMotor.TryGetComponent<Slime>(out var slime))
            {
                slime.TakeDamage(damage);
            }
            slimeMotor.rb.AddExplosionForce(explosiveForce, transform.position, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
        }
        
        if (additionalLocks != null)
        {
            //  iterate through all the living slimes, if the target is equal to this transform--
            foreach (var slimes in GameManager.instance.livingSlimes)
            {
                if (slimes.GetDestinationTransform() == additionalLocks.transform)
                {
                    //  set the destination to this transform's position to avoid null reference
                    slimes.SetDestination(additionalLocks.transform.position);
                }
            }
            Destroy(additionalLocks);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SlimeMotor>(out var slime))
        {
            slimesInRange.Add(slime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<SlimeMotor>(out var slime))
        {
            slimesInRange.Remove(slime);
        }
    }
}
