using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TargetHealth : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI targetName;
    [SerializeField] Image healthBar;

    private Joint targetJoint;
    private Slime targetSlime;

    private bool hasTarget = false;

    private void Awake()
    {
        Slime.onMouseOverEvent += SetTarget;
        Joint.onMouseOverEvent += SetTarget;

        Slime.onMouseExitEvent += ClearTarget;
        Joint.onMouseExitEvent += ClearTarget;

        healthBar.gameObject.SetActive(false);
        targetName.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Slime.onMouseOverEvent -= SetTarget;
        Joint.onMouseOverEvent -= SetTarget;

        Slime.onMouseExitEvent -= ClearTarget;
        Joint.onMouseExitEvent -= ClearTarget;
    }

    public void Update()
    {
        if (hasTarget)
        {
            FillHealthBar();
        }
    }

    public void FillHealthBar()
    {
        healthBar.fillAmount = GetTargetHealthPercentage();
    }

    public void SetTarget(Slime _slime)
    {
        healthBar.gameObject.SetActive(true);
        targetJoint = null;
        targetSlime = _slime;
        FillHealthBar();
        hasTarget = true;

        //  TODO: assign target name
    }

    public void SetTarget(Joint _joint)
    {
        healthBar.gameObject.SetActive(true);
        targetJoint = _joint;
        targetSlime = null;
        FillHealthBar();
        hasTarget = true;

        //  TODO: assign target name
    }

    public void ClearTarget(bool _)
    {
        hasTarget = false;
        healthBar.gameObject.SetActive(false);
        targetJoint = null;
        targetSlime = null;

        //  TODO: clear / set name text indicator to inactive
    }

    public float GetTargetHealthPercentage()
    {
        
        if (targetSlime)
        {
            return (float)targetSlime.CurrentHealth / targetSlime.MaxHealth;
        }
        else
        {
            return (float)targetJoint.CurrentHealth / targetJoint.MaxHealth;
        }
    }
}
