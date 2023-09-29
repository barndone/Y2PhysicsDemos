
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePen : MonoBehaviour
{
    [SerializeField] LayerMask slimeMask;
    List<GameObject> containedSlimes = new();

    [SerializeField] float timeBetweenTicks = 0.5f;
    float timer = 0.0f;

    [SerializeField] int healthPerTick = 1;

    private AudioSource source;

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == slimeMask)
        {
            containedSlimes.Add(other.gameObject);

            if (other.TryGetComponent<SlimeMotor>(out var slime))
            {
                slime.SetDestination(slime.transform);
            }
            Debug.Log(other.gameObject.name + " has entered the pen!", other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (1 << other.gameObject.layer == slimeMask)
        {
            containedSlimes.Remove(other.gameObject);
            Debug.Log(other.gameObject.name + " has left the pen!", other.gameObject);
        }
    }

    private void Awake()
    {
        if (TryGetComponent<AudioSource>(out source))
        {
            //  do nothing
        }

        else { Debug.LogError("No AudioSource component attached to " + this.name, this); }
    }

    private void Update()
    {
        if (containedSlimes.Count > 0)
        {
            if (timer < timeBetweenTicks)
            {
                timer += Time.deltaTime;
            }
            else
            {
                //  TODO Visual Indicator for healing
                timer = 0.0f;

                bool healed = false;

                foreach (var go in containedSlimes)
                {
                    if (go.TryGetComponent<Slime>(out var slime)) 
                    
                    {
                        var healedSlime = slime.Heal(healthPerTick);
                        
                        if (healedSlime && !healed)
                        {
                            healed = true;
                        }
                    }
                }

                if (healed)
                {
                    source.PlayOneShot(AudioManager.instance.GetHealingTickSound());
                }
            }
        }
    }
}
