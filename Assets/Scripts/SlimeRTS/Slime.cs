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

    [SerializeField] List<Color> slimeColors = new List<Color>();

    [SerializeField] Color currentColor;
    [SerializeField] Color targetColor;
    [SerializeField] float transitionSpeed = .5f;

    private bool updateColor = false;

    [SerializeField] MeshRenderer rend;
    [SerializeField] Animator animator;

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
            currentColor = slimeColors[0];

            TryGetComponent<MeshRenderer>(out rend);
            rend.material.color = currentColor;
        }
        else
        {
            Debug.Log("Error, slime motor not attached to " + gameObject.name, this);
        }

        if (TryGetComponent<Animator>(out animator))
        {
            animator.enabled = false;
        }
        else
        {
            Debug.Log("Error, animator not attached to " + gameObject.name, this);
        }
    }

    private void Update()
    {
        if (updateColor)
        {
            if (currentColor != targetColor)
            {
                currentColor = Color.Lerp(currentColor, targetColor, transitionSpeed * Time.deltaTime);
                rend.material.color = currentColor;
            }

            if (currentColor == targetColor)
            {
                updateColor = false;
            }
        }
    }

    public void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth <= 0)
        {
            GameManager.instance.AddDeadSlime(motor);
            animator.enabled = true;
            animator.SetTrigger("death");
        }

        if (currentHealth / (float)maxHealth >= .75f)
        {
            targetColor = slimeColors[0];
        }
        else if (currentHealth / (float)maxHealth >= .5f)
        {
            targetColor = slimeColors[1];
        }
        else if (currentHealth / (float)maxHealth >= .25f)
        {
            targetColor = slimeColors[2];
        }
        else if (currentHealth / (float)maxHealth >= 0f)
        {
            targetColor = slimeColors[3];
        }

        updateColor = true;
    }

    public void DeathCleanup()
    {
        Destroy(gameObject);
    }
}
