using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slime : MonoBehaviour, IDamageable, IHealable
{
    //  max possible health for the slime
    [SerializeField] int maxHealth = 5;
    //  starting mass of the slime
    [SerializeField] float startingMass = 10f;

    private int curHealth = 0;
    private float curMass = 0.0f;

    public string unitName;

    //  public get and private set field 
    public int CurrentHealth
    {
        get { return curHealth; }
        private set { curHealth = value; }
    }

    public int MaxHealth { get { return maxHealth; } }

    public float CurrentMass
    {
        get { return curMass; }
        private set { curMass = value; }
    }
    private SlimeMotor motor;

    [SerializeField] List<Color> slimeColors = new List<Color>();

    [SerializeField] Color currentColor;
    [SerializeField] Color targetColor;
    [SerializeField] float transitionSpeed = .5f;

    private bool updateColor = false;

    [SerializeField] MeshRenderer rend;
    [SerializeField] Animator animator;

    public static event Action<SlimeMotor> removeFromList;
    public static event Action<Slime> onMouseOverEvent;
    public static event Action<string> healthBarDeathCleanupEvent;


    Color startColor;
    float timer = 0;

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
        //  timer
        //  start time // duration

        if (updateColor)
        {

            if (timer < transitionSpeed)
            {
                timer += Time.deltaTime;
                currentColor = Color.Lerp(startColor, targetColor, timer / transitionSpeed);
                rend.material.color = currentColor;
            }
            else
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
            healthBarDeathCleanupEvent.Invoke(unitName);
            GameManager.instance.AddDeadSlime(motor);
            animator.enabled = true;
            animator.SetTrigger("death");
        }

        UpdateColor();
    }

    //  implemented to satisfy IHealable
    public void Heal(int _value)
    {
        //  if the healing value wont heal us past our max health
        if ((CurrentHealth + _value) <= maxHealth) { CurrentHealth += _value; }
        //  otherwise, it would put us over our maxHealth, assign currentHealth to maxhealth
        else { CurrentHealth = maxHealth; }

        UpdateColor();
    }

    public void UpdateColor()
    {
        startColor = rend.material.color;

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

        timer = 0.0f;
        updateColor = true;
    }


    public void DeathCleanup()
    {
        removeFromList.Invoke(motor);
        Destroy(gameObject);
    }

    public void OnMouseEnter()
    {
        onMouseOverEvent.Invoke(this);
    }
}
