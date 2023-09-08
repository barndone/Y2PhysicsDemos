using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSelection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (SlimePicker.instance.multiSelectWish)
        {
            if (other.TryGetComponent<SlimeMotor>(out var slime))
            {
                SlimePicker.instance.AddOverlappingSlimeMotor(slime);
            }   
        }
    }
}
