using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> jumpSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> impactSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> slimeImpactSounds = new List<AudioClip>();

    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip commandSound;
    [SerializeField] AudioClip clearSelectionSound;
    [SerializeField] AudioClip healTickSound;

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

    public AudioClip GetSelectionSound()
    {
        return selectSound;
    }

    public AudioClip GetCommandSound()
    {
        return commandSound;
    }

    public AudioClip GetClearSelectionSound()
    {
        return clearSelectionSound;
    }

    public AudioClip GetHealingTickSound()
    {
        return healTickSound;
    }
}
