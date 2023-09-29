using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    //  list containing all of the available (alive) slimes
    public List<SlimeMotor> livingSlimes = new();
    //  list containing all the unavailable (dead) slimes
    [SerializeField] private List<SlimeMotor> deadSlimes = new();

    //  return the number of entries is living slimes
    public int AvailableSlimes { get { return livingSlimes.Count; } }
    //  return the number of entries in dead slimes
    public int DeadSlimes { get { return deadSlimes.Count; } }

    //  singleton instnace
    public static GameManager instance;

    //  tracks if the game is over
    public bool gameOver = false;
    private bool gameOverHandled;

    [SerializeField] Animator animator;

    public static event Action<bool> panelFadeEvent;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (gameOver && !gameOverHandled)
        {
            HandleGameOver();
            gameOverHandled = true;
        }
    }

    //  add a given slime to the living slimes list
    public void AddLivingSlime(SlimeMotor _slime) 
    {
        //  is this slime already in the living slimes list?
        if (!livingSlimes.Contains(_slime))
        {
            //  if not, add it!
            livingSlimes.Add(_slime);
        }
    }

    //  remove a given slime from the living or selected slimes lists and add to the dead slimes list
    public void AddDeadSlime(SlimeMotor _slime)
    {
        if (!gameOver)
        {        
            //  cache the list of selected slimes in the slime picker instance
            var selectedSlimes = SlimePicker.instance.slimeList;
            _slime.alive = false;

            // check that no slimes were following this gameobject
            foreach (var slime in livingSlimes)
            {
                if (slime.Target == _slime.gameObject)
                {
                    slime.SetDestination(_slime.transform.position);
                }
            }

            //  if the selected slimes list contains this now dead slime
            if (selectedSlimes.Contains(_slime))
            {
                //  remove it
                selectedSlimes.Remove(_slime);
            }

            //  if the living slimes list contains this slime
            if (livingSlimes.Contains(_slime))
            {
                //  remove it
                livingSlimes.Remove(_slime);
            }

            //  if this slime isn't already counted as dead
            if (!deadSlimes.Contains(_slime))
            {
                //  remove it
                deadSlimes.Add(_slime);
            }

            //  if the available slimes list is empty
            if (AvailableSlimes == 0)
            {
                //  game over
                gameOver = true;
            }
        }
    }

    public void HandleGameOver()
    {
        bool condition = AvailableSlimes <= 0;
        //  case 1: all of our slimes are dead
        //  loss.meme
        if (condition)
        {
            RagdollUtils.EnableRagdoll();
            animator.enabled = false;
        }

        //  otherwise, we have at LEAST 1 slime and all the joints are defeated
        //  winner_winner_chicken_dinner.wav
        else
        {
            
        }

        Cursor.lockState = CursorLockMode.Confined;
        //  TODO: implement end screen UI
        panelFadeEvent.Invoke(condition);

    }
    public void JointBugHeadShot()
    {
        gameOver = true;
    }
}
