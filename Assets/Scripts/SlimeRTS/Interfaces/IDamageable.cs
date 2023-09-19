using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  interface for all objects that can take damage
interface IDamageable
{
    public int CurrentHealth
    {
        get { return CurrentHealth; }
        private set { CurrentHealth = value; }
    }
    
    void TakeDamage(int _damage);
}
