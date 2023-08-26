using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePen : MonoBehaviour
{
    [SerializeField] int slimeCap = 1;
    [SerializeField] LayerMask slimeMask;
    List<GameObject> containedSlimes = new();



    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == slimeMask)
        {
            containedSlimes.Add(other.gameObject);
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
}
