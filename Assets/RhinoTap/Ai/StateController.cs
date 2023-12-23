using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.StateMachine;

using Rhinotap.Toolkit;

/// <summary>
/// This class can be customized according to your AI needs, 
/// There may be only one StateController in the project
/// This file must stay in its original folder
/// </summary>

public class StateController : StateControllerBase
{
    [Header("General Settings")]
    [Space(20)]
    private Fish fishController;
    public Fish FishController => fishController;

    //Will move according to this
    private Vector2 currentDirection = Vector2.zero;
    //Target to chase
    private Transform currentTarget = null;

    private void Awake()
    {
        fishController = GetComponent<Fish>();
        if (fishController == null)
            Debug.Log("Fish component missing from an enemy.");

        EventManager.StartListening<bool>("gamePaused", (isPaused) => {
            if (isPaused)
                Pause();
            else
                Resume();
                });
    }

    private new void Update()
    {
        base.Update();
    }


}

