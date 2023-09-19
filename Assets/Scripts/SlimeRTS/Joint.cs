using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 5;

    public int CurrentHealth
    {
        get { return CurrentHealth; } private set { CurrentHealth = value; }
    }

    public void TakeDamage(int _damage)
    {

        // TODO: calculate total force impulse applied from slime
        // TODO: calculate as int

        CurrentHealth -= _damage;

        //  TODO: handle death
    }


}
