using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI selectedSlimesIndicator;
    [SerializeField] TextMeshProUGUI totalSlimesIndicator;
    [SerializeField] TextMeshProUGUI objectiveIndicator;

    [SerializeField] TextMeshProUGUI tutorialText;

    [TextArea (5,10)]
    [SerializeField] List<string> tutorialPrompts = new List<string>();

    [TextArea(5, 10)]
    [SerializeField] List<string> objectivePrompts = new List<string>();

    [SerializeField] GameObject tutorialObj;

    [SerializeField] bool movementTutorialShown = false;
    [SerializeField] bool slimeTutorialShown = false;
    [SerializeField] bool healingTutorialShown = false;

    [SerializeField] private bool fadeEndPanelIn = false;

    [SerializeField] Image endPanelImage;
    private Color startColor;
    private Color destColor;

    [SerializeField] float fadeInTime = 1.0f;
    [SerializeField] private bool doneFading = false;

    [SerializeField]
    [Range(0.0f, 1.0f)] float targetAlpha = 1.0f;

    private bool fail = false;

    private float counter = 0.0f;

    [SerializeField] string victoryText = "Victory!";
    [SerializeField] string defeatText = "Defeat...";

    [SerializeField] List<GameObject> endScreenObjects = new List<GameObject>();
    [SerializeField] List<GameObject> baseGameUIObjects = new List<GameObject>();

    [SerializeField] TextMeshProUGUI resultsText;
    [SerializeField] TextMeshProUGUI rankText;

    [SerializeField] TextMeshProUGUI endScrnSlimesRemainingText;
    [SerializeField] TextMeshProUGUI endScrnTotalSlimesText;
    [SerializeField] TextMeshProUGUI endScrnAdviceText;

    public void UpdateUI()
    {
        selectedSlimesIndicator.text = "Selected Slimes: " + SlimePicker.instance.SelectedSlimes;
        totalSlimesIndicator.text = "Available Slimes: " + GameManager.instance.AvailableSlimes;
    }

    public void Update()
    {
        UpdateUI();

        if (fadeEndPanelIn)
        {
            destColor = new Color(endPanelImage.color.r, endPanelImage.color.g, endPanelImage.color.b, targetAlpha);

            if (counter < fadeInTime)
            {
                counter += Time.deltaTime;

                var c = Color.Lerp(startColor, destColor, counter / fadeInTime);
                endPanelImage.color = c;
            }
            else
            {
                fadeEndPanelIn = false;
                doneFading = true;
                counter = 0.0f;
            }
        }

        if (doneFading)
        {
            InitEndScreen();
        }
    }

    private void Awake()
    {
        HideTutorials();
        GameManager.panelFadeEvent += FadeInEndScreen;
    }

    private void OnDestroy()
    {
        GameManager.panelFadeEvent -= FadeInEndScreen;
    }

    //  handle movement, aiming
    public void FirstTutorial()
    {
        if (!movementTutorialShown)
        {
            tutorialObj.SetActive(true);
            //  update the tutorial prompt to the first text prompt
            tutorialText.text = tutorialPrompts[0];

            movementTutorialShown = true;
        }

    }

    //  handle selection, issuing commands
    public void SlimeTutorial()
    {
        if (!slimeTutorialShown)
        {
            tutorialObj.SetActive(true);
            //  update the tutorial prompt to the next text prompt
            tutorialText.text = tutorialPrompts[1];

            slimeTutorialShown = true;
        }

    }

    //  show the player where they can heal their slimes when they take damage
    public void HealingTutorial()
    {
        if (!healingTutorialShown)
        {
            tutorialObj.SetActive(true);
            //  update the tutorial prompt to the next text prompt
            tutorialText.text = tutorialPrompts[2];

            healingTutorialShown = true;
        }
    }

    public void HideTutorials()
    {
        tutorialObj.SetActive(false);
    }

    //  show the objective
    public void ObjectiveIndicator()
    {
        //  TODO: implement
    }

    public void FadeInEndScreen(bool condition)
    {
        fadeEndPanelIn = true;
        fail = condition;
        startColor = endPanelImage.color;
    }

    public void InitEndScreen()
    {
        foreach(var go in baseGameUIObjects)
        {
            go.SetActive(false);
        }

        foreach(var go in endScreenObjects)
        {
            go.SetActive(true);
        }
        _ = fail ? (resultsText.text = defeatText) : (resultsText.text = victoryText);

        rankText.text = CalculateRank();

        endScrnSlimesRemainingText.text = "Slimes Remaining: " + GameManager.instance.AvailableSlimes;
        endScrnTotalSlimesText.text = "Out of " + (GameManager.instance.DeadSlimes + GameManager.instance.AvailableSlimes) + " total slimes.";

        doneFading = false;
    }

    public string CalculateRank()
    {
        string result = "";

        float percentageOfSlimesAlive = (float)GameManager.instance.AvailableSlimes / (float)(GameManager.instance.DeadSlimes + GameManager.instance.AvailableSlimes);

        switch (percentageOfSlimesAlive)
        {
            case float x when x is 1.0f:
                result = "S Rank Slime Corraler";
                endScrnAdviceText.text = "Perfect!\nYou saved every slime!";
                break;
            case float x when x is < 1f and >=0.9f:
                result = "A Rank Slime Corraler";
                endScrnAdviceText.text = "Nicely Done!\nYou're close to saving every slime!\nI think if we focus the head after taking out the elbows you can do it!!";
                break;
            case float x when x is < 0.9f and >= 0.8f:
                result = "B Rank Slime Corraler";
                endScrnAdviceText.text = "Good Job!\nWith a little more work I'm sure you can save every slime!\nTry to stagger your from hitting the joints!.";
                break;
            case float x when x is < 0.8f and >= 0.7f:
                result = "C Rank Slime Corraler";
                endScrnAdviceText.text = "You made it!\nTry to focus your efforts on one joint at a time.\nYou can always send your slimes to a healing pen to reset their health.";
                break;
            case float x when x is < 0.7f and > 0.0f:
                result = "D Rank Slime Corraler";
                endScrnAdviceText.text = "Close fight!\nBe careful to move your slimes out of the slam if they're low health!\nDon't give up!";
                break;
            case float x when x is 0.0f:
                result = "F Rank Slime Corraler";
                endScrnAdviceText.text = "Almost...\nMake sure you're utilizing the healing pens!\nTry to direct your slimes away from the boss when their health gets low.";
                break;
        }

        return result;
    }
}
