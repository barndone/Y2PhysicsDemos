using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> jumpSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> impactSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> slimeImpactSounds = new List<AudioClip>();

    [SerializeField] AudioSource musicSource;

    public static AudioManager instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        else
        {
            Destroy(this);
        }
    }


    public AudioClip GetRandomJumpSound()
    {
        return jumpSounds[Random.Range(0, jumpSounds.Count)];
    }

    public AudioClip GetRandomImpactSound()
    {
        return impactSounds[Random.Range(0, impactSounds.Count)];
    }

    public AudioClip GetSlimeImpactSound()
    {
        return slimeImpactSounds[Random.Range(0, slimeImpactSounds.Count)];
    }
}
