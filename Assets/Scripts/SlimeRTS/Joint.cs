using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Joint : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 5;

    private Rigidbody rb;

    [SerializeField] UnityEvent jointBreakEvent;

    public static event Action<Rigidbody> cleanUpReferences;
    public static event Action<Joint> onMouseOverEvent;
    public static event Action<string> healthBarDeathCleanupEvent;

    public string unitName;

    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; } private set { currentHealth = value; }
    }
    public int MaxHealth { get { return maxHealth; } }
    private void OnEnable()
    {
        CurrentHealth = maxHealth;
        TryGetComponent<Rigidbody>(out rb);
    }

    public void TakeDamage(int _damage)
    {
        CurrentHealth -= _damage;

        if (CurrentHealth < 0)
        {            
            healthBarDeathCleanupEvent.Invoke(unitName);
            jointBreakEvent.Invoke();
            cleanUpReferences.Invoke(rb);
            Destroy(gameObject);
        }
    }

    //  check for collisions
    public void OnCollisionEnter(Collision collision)
    {
        //   if the colliding object is a slime, take damage
        if (collision.gameObject.TryGetComponent<SlimeMotor>(out var slime))
        {

            //  convert the impulse into the total force by dividing by fixedDeltaTime
            //  calculate how much damage this joint should take
            var damage = CalculateDamageFromForce(collision.impulse / Time.fixedDeltaTime);

            //  check if we would be taken below 0 health
            if (CurrentHealth - damage <= 0)
            {
                //  iterate through all the living slimes, if the target is equal to this transform--
                foreach (var slimes in GameManager.instance.livingSlimes)
                {
                    if (slimes.GetDestinationTransform() == transform)
                    {
                        //  set the destination to this transform's position to avoid null reference
                        slimes.SetDestination(transform.position);
                    }
                }
            }

            //  take damage!
            TakeDamage(damage);
        }
        //  otherwise do nothing
    }
    public int CalculateDamageFromForce(Vector3 _force)
    {
        //  divide the absolute value of the force magnitude by 10, cast to int
        return (int)Mathf.Abs(_force.magnitude) / 10;
    }
    public void OnMouseEnter()
    {
        onMouseOverEvent.Invoke(this);
    }
}
