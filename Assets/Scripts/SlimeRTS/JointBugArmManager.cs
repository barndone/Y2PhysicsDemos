using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The JointBugArmManager is responsible for keeping track of all parts of the arm associated with this component.
///     1. Joints
///     2. ImpulsePoint
///     3. Tracking how many slimes are in trigger range
///     4. Beginning the attack after a given time has elapsed
/// </summary>
public class JointBugArmManager : MonoBehaviour
{
    //  list of joints in this arm
    [SerializeField] List<Rigidbody> joints = new List<Rigidbody>();
    //  the body part the armLift force will be applied to
    [SerializeField] Rigidbody lowerArm;
    [SerializeField] Rigidbody upperArm;
    //  time the joint bug will wait before attacking
    [SerializeField] float attackWaitTime = 1.5f;
    //  internal timer for tracking the attack wait time
    private float waitTimer = 0.0f;

    //  how many slimes should be in range before starting the attack timer (default to 1)
    [SerializeField] int slimesToStartAttack = 1;

    //  internal tracker for how many slimes are in the trigger volume
    private List<SlimeMotor> slimesInRange= new List<SlimeMotor>();

    private bool attacking = false;
    private bool attackWish = false;

    [SerializeField] float lowerArmLift = 10.0f;
    [SerializeField] float upperArmLift;
    [SerializeField] float explosiveForce = 5.0f;
    [SerializeField] float explosionRadius = 2.0f;
    [SerializeField] float upwardExplosionForce = 4.0f;

    [SerializeField] int damage = 1;

    [SerializeField] UnityEvent attackEvent;

    [SerializeField] LayerMask finishAttackLayers;

    private void Awake()
    {
        Slime.removeFromList += RemoveDeadSlime;
    }

    private void OnDestroy()
    {
        Slime.removeFromList -= RemoveDeadSlime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SlimeMotor>(out var slime))
        { 
                slimesInRange.Add(slime);
                if (slimesInRange.Count >= slimesToStartAttack && !attacking) { attackWish = true; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<SlimeMotor>(out var slime))
        {
            slimesInRange.Remove(slime);
        }
    }

    private void Update()
    {
        if (attackWish)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= attackWaitTime)
            {
                waitTimer = 0.0f;
                attackWish = false;
                BeginArmSlam();
                attacking = true;
            }
        }
    }

    public void BeginArmSlam()
    {
        foreach (var joint in joints)
        {
            joint.isKinematic = false;
        }

        upperArm.AddForce(upperArm.transform.forward * upperArmLift, ForceMode.Impulse);
        lowerArm.AddForce(Vector3.up * lowerArmLift, ForceMode.Impulse);
    }

    public void ResolveAttackEnd()
    {
        foreach (var joint in joints)
        {
            joint.isKinematic = true;
        }

        attacking = false;

        if (slimesInRange.Count >= slimesToStartAttack && !attacking) { attackWish = true; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (attacking && finishAttackLayers.Contains(collision.gameObject.layer))
        {
            foreach (var slimeMotor in slimesInRange)
            {
                if (slimeMotor.TryGetComponent<Slime> (out var slime))
                {
                    slime.TakeDamage(damage);
                }
                slimeMotor.rb.AddExplosionForce(explosiveForce, collision.GetContact(0).point, explosionRadius, upwardExplosionForce, ForceMode.Impulse);
            }

            ResolveAttackEnd();
        }
    }

    public void RemoveDeadSlime(SlimeMotor _deadSlime)
    {
        slimesInRange.Remove(_deadSlime);
    }

    public void ArmDeath()
    {
        //  TODO implement event that passes this script to clean up reference in JointBugController.cs
        Destroy(this);
    }
}
