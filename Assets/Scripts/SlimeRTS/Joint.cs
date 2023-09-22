using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Joint : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 5;

    [SerializeField] UnityEvent jointBreakEvent;

    private int currentHealth;
    public int CurrentHealth
    {
        get { return currentHealth; } private set { currentHealth = value; }
    }

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int _damage)
    {
        CurrentHealth -= _damage;

        if (CurrentHealth < 0)
        {
            jointBreakEvent.Invoke();
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
}
