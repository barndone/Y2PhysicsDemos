using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialEventHandler : MonoBehaviour
{
    [SerializeField] UnityEvent promptEvent;
    [SerializeField] UnityEvent exitEvent;
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            promptEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            exitEvent.Invoke();
        }
    }
}
