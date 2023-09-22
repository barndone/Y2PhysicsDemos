using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    [SerializeField] float openedAngleLimit = 75.0f;
    [SerializeField] HingeJoint leftDoor;
    [SerializeField] HingeJoint rightDoor;

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
    }
}
