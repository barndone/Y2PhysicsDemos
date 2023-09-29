using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class TargetHealth : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI targetName;
    [SerializeField] Image healthBar;
    private Joint targetJoint;
    private Slime targetSlime;
    private bool hasTarget = false;
    private string currentTargetName = "";
    [SerializeField] float lerpSpeed = 3f;

    [SerializeField] GameObject healthBarParent;

    [SerializeField] GameObject enemyIcon;
    [SerializeField] GameObject slimeIcon;

    private void Awake()
    {
        Slime.onMouseOverEvent += SetTarget;
        Joint.onMouseOverEvent += SetTarget;

        Slime.healthBarDeathCleanupEvent += ClearTarget;
        Joint.healthBarDeathCleanupEvent += ClearTarget;

        healthBarParent.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Slime.onMouseOverEvent -= SetTarget;
        Joint.onMouseOverEvent -= SetTarget;

        Slime.healthBarDeathCleanupEvent -= ClearTarget;
        Joint.healthBarDeathCleanupEvent -= ClearTarget;
    }

    public void Update()
    {
        if (hasTarget)
        {
            FillHealthBar();
        }

        if (GameManager.instance.gameOver)
        {
            ClearTarget(currentTargetName);
        }
    }

    public void FillHealthBar()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, GetTargetHealthPercentage(), lerpSpeed * Time.deltaTime);
        ColorTransition();
    }

    public void ColorTransition()
    {
        var color = Color.Lerp(Color.red, Color.green, GetTargetHealthPercentage());
        healthBar.color = color;
    }

    public void SetTarget(Slime _slime)
    {
        healthBarParent.SetActive(true);
        targetJoint = null;
        targetSlime = _slime;
        FillHealthBar();
        hasTarget = true;
        targetName.gameObject.SetActive(true);
        currentTargetName = _slime.unitName;
        targetName.text = _slime.unitName;
        slimeIcon.SetActive(true);
        enemyIcon.SetActive(false);
    }

    public void SetTarget(Joint _joint)
    {
        healthBarParent.SetActive(true);
        targetJoint = _joint;
        targetSlime = null;
        FillHealthBar();
        hasTarget = true;
        targetName.gameObject.SetActive(true);
        currentTargetName = _joint.unitName;
        targetName.text = _joint.unitName;
        slimeIcon.SetActive(false);
        enemyIcon.SetActive(true);
    }

    public void ClearTarget(string _name)
    {
        if (_name.Equals(currentTargetName))
        {
            hasTarget = false;
            healthBarParent.SetActive(false);
            targetJoint = null;
            targetSlime = null;
            currentTargetName = "";
        }
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
