using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour, IDamageable, IHealable
{
    //  max possible health for the slime
    [SerializeField] int maxHealth = 5;
    //  starting mass of the slime
    [SerializeField] float startingMass = 10f;

    //  public get and private set field 
    public int CurrentHealth
    {
        get { return CurrentHealth; }
        private set { CurrentHealth = value; }
    }


    public float CurrentMass
    {
        get { return CurrentMass; }
        private set { CurrentMass = value; }
    }
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
        CurrentHealth = maxHealth;
        CurrentMass = startingMass;
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

    //  implemented to satisfy IDamageable
    public void TakeDamage(int _damage)
    {
        CurrentHealth -= _damage;

        if (CurrentHealth <= 0)
        {
            GameManager.instance.AddDeadSlime(motor);
            animator.enabled = true;
            animator.SetTrigger("death");
        }

        if (CurrentHealth / (float)maxHealth >= .75f)
        {
            targetColor = slimeColors[0];
        }
        else if (CurrentHealth / (float)maxHealth >= .5f)
        {
            targetColor = slimeColors[1];
        }
        else if (CurrentHealth / (float)maxHealth >= .25f)
        {
            targetColor = slimeColors[2];
        }
        else if (CurrentHealth / (float)maxHealth >= 0f)
        {
            targetColor = slimeColors[3];
        }

        updateColor = true;
    }

    //  implemented to satisfy IHealable
    public void Heal(int _value)
    {
        //  if the healing value wont heal us past our max health
        if ((CurrentHealth + _value) <= maxHealth) { CurrentHealth += _value; }
        //  otherwise, it would put us over our maxHealth, assign currentHealth to maxhealth
        else { CurrentHealth = maxHealth; }

    }


    public void DeathCleanup()
    {
        Destroy(gameObject);
    }
}
