using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] float startingMass = 10f;

    public int currentHealth;
    public float currentMass;

    private SlimeMotor motor;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentMass = startingMass;
    }

    private void Start()
    {
        if (TryGetComponent<SlimeMotor>(out motor))
        {
            GameManager.instance.AddLivingSlime(motor);
        }
        else
        {
            Debug.Log("Error, slime motor not attached to " + gameObject.name, this);
        }
    }

    public void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
        {
            GameManager.instance.AddDeadSlime(motor);
        }
    }
}
