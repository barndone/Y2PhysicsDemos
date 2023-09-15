using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI selectedSlimesIndicator;
    [SerializeField] TextMeshProUGUI totalSlimesIndicator;
    [SerializeField] TextMeshProUGUI objectiveIndicator;

    public void UpdateUI()
    {
        selectedSlimesIndicator.text = "Selected Slimes: " + SlimePicker.instance.SelectedSlimes;
        totalSlimesIndicator.text = "Available Slimes: " + GameManager.instance.AvailableSlimes;
    }

    public void Update()
    {
        UpdateUI();
    }
}
