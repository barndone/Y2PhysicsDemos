using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointBugController : MonoBehaviour
{
    //  references to the arm managers for the joint bug
    //      references for the arm managers on the lower arms
    [SerializeField] private JointBugArmManager leftForearm;
    [SerializeField] private JointBugArmManager rightForearm;
    //      references for the arm managers on the upper arms
    [SerializeField] private JointBugArmManager leftBicep;
    [SerializeField] private JointBugArmManager rightBicep;

    //  Are the arms fully intact?
    //      I.E- forearm AND bicep
    public bool leftArmIntact = true;
    public bool rightArmIntact = true;

    //  does this still have arms?
    //      AT LEAST a bicep
    public bool leftArmExists = true;
    public bool rightArmExists = true;


    //  references to all of the joints that comprise this jointBug
    [SerializeField] List<Rigidbody> joints = new List<Rigidbody>();
         

    private void OnEnable()
    {
        JointBugArmManager.partBroken += HandlePartBreak;
        Joint.cleanUpReferences += HandleJointRemoval;
    }

    private void OnDisable()
    {
        JointBugArmManager.partBroken -= HandlePartBreak;
        Joint.cleanUpReferences -= HandleJointRemoval;
    }

    private void HandlePartBreak(JointBugArmManager _manager)
    {
        Debug.Log(_manager.gameObject.name + " death event received from", _manager);

        if (_manager == leftForearm)
        {
            leftArmIntact = false;
            leftBicep.ActivateLimb();
        }

        else if (_manager == rightForearm)
        {
            rightArmIntact = false;
            rightBicep.ActivateLimb();
        }

        else if (_manager == leftBicep)
        {
            leftArmExists = false;
        }

        else if (_manager == rightBicep)
        {
            rightArmExists = false;
        }

        if (!leftArmIntact && !rightArmIntact)
        {
            EnableRagdoll();
        }
    }

    public void HandleJointRemoval(Rigidbody _joint)
    {
        if (joints.Contains(_joint))
        {
            joints.Remove(_joint);
        }
    }

    public void EnableRagdoll()
    {
        foreach (Rigidbody joint in joints)
        {
            joint.isKinematic = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (Rigidbody joint in joints)
        {
            joint.isKinematic = true;
        }
    }


}
