using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class VirtualJoystick : MonoBehaviour
{
    #region instance
    private static VirtualJoystick _instance; //instance of this object
    public static VirtualJoystick instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VirtualJoystick>();//find the instance in the scene
                if (_instance == null)
                    Debug.LogError("Mobile Input Manager Not available in the scene");
            }
            return _instance;
        }
    }
    #endregion

    //===============| Public API |==================//
    //Current Movement Clamped
    public static float GetAxis(string axisName)
    {
        if( !instance.touchActivated)
        {
            return 0f;
        }

        if(axisName == "Horizontal" || axisName == "horizontal")
        {
            return instance.currentDirection.x;
        }else
        {
            return instance.currentDirection.y;
        }
    }
    //Direction Vector 
    public static Vector2 Direction(bool sticky = false)
    {
        if( sticky)
        {
            if (instance.touchActivated)
                return instance.currentDirection;
            else
                return instance.lastDirection;
        }else
        {
            return instance.currentDirection;
        }
    }
    //Joystick Rotation (Z axis)
    public static Quaternion Rotation(bool sticky = false)
    {
        if( sticky)
        {
            if (instance.touchActivated)
                return instance.currentRotation;
            else
                return instance.lastRotation;
        } else
        {
            return instance.currentRotation;
        }
    }
    //Touch starting position (Screen Space)
    public static Vector2 StartPosition(bool sticky = false)
    {
        if (sticky)
        {
            if (instance.touchActivated)
                return instance.currentStartPosition;
            else
                return instance.lastStartPosition;
        }
        else
        {
            return instance.currentStartPosition;
        }
    }

    

    public static bool isGoingRight { get { return !instance.isGoingLeft; } }

    //=============| Internal Vars |=================//
    private Vector2 currentStartPosition = Vector2.zero;
    private Vector2 lastStartPosition = Vector2.zero;
    private Vector2 currentDirection = Vector2.zero;
    private Vector2 lastDirection = Vector2.zero;
    private Quaternion currentRotation = Quaternion.identity;
    private Quaternion lastRotation = Quaternion.identity;
    private bool isGoingLeft = false;
    private bool touchActivated = false;



    private void Update()
    {
        CalculateTouch();

        CheckGoingLeft();
        CheckGoingRight();
    }


    //===============| Visualizer |==================//




    //===============| Calculations |==================//

    void CalculateTouch()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);


        if (touch.phase == TouchPhase.Began)
        {
            InitializeJoystickState(touch);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            JoystickMovedState(touch);
        }
        else if (touch.phase == TouchPhase.Stationary)
        {
            //Do nothing
        }else if( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            ResetJoystickState();
        }
    }

    //State: user is not touching screen
    void ResetJoystickState()
    {
        //Memory
        lastDirection = currentDirection;
        lastRotation = currentRotation;
        lastStartPosition = currentStartPosition;

        //Reset currents
        currentDirection = Vector2.zero;
        currentStartPosition = Vector2.zero;
        currentRotation = Quaternion.identity;

        //touch active?
        touchActivated = false;

    }

    //State: user just started pressing on the screen
    void InitializeJoystickState(Touch touch)
    {

        //Save starting position of the touch (Screen Space)
        currentStartPosition = touch.position;
        currentRotation = lastRotation;
        

        //Activate touch toggle
        touchActivated = true;

    }

    //State: user continues to touch the screen and moves their finger
    void JoystickMovedState(Touch touch)
    {
        //Calculate Direction and clamp to 0-1
        Vector2 deltaMovement = Vector2.ClampMagnitude( touch.position - currentStartPosition , 1f);

        float angle = Mathf.Atan2(deltaMovement.y, deltaMovement.x) * Mathf.Rad2Deg;

        currentDirection = deltaMovement;
        currentRotation = Quaternion.Euler(0f, 0f, angle);
    }



    //===============| Flip with timer |==================//
    public static void SetFlipTimer(float timer = 1f)
    {
        if (timer > 0.1f)
            instance.flipTimer = timer;
        else
            instance.flipTimer = 0.1f;
    }
    private float flipTimer = 1f;
    float goingLeftForSeconds = 0f;
    void CheckGoingLeft()
    {
        if (isGoingLeft == true)
        {
            goingLeftForSeconds = 0f;
            return;
        }
        Vector2 directionToCheck = currentDirection;
        if (!touchActivated)
            directionToCheck = lastDirection;

        
        float goingLeft = Vector2.Dot(directionToCheck, Vector2.left);

        if (goingLeft > 0.8f)
            goingLeftForSeconds += Time.deltaTime;
        else
            goingLeftForSeconds = goingLeftForSeconds > 0f ? goingLeftForSeconds - Time.deltaTime : 0f;

        if ( goingLeftForSeconds > flipTimer)
        {
            isGoingLeft = true;
            //Debug.Log("Flipped to left");
            goingLeftForSeconds = 0f;
        }
    }

    float goingRightForSeconds = 0f;
    void CheckGoingRight()
    {
        if (isGoingLeft == false)
        {
            goingRightForSeconds = 0f;
            return;
        }
        Vector2 directionToCheck = currentDirection;
        if (!touchActivated)
            directionToCheck = lastDirection;


        float goingRight = Vector2.Dot(directionToCheck, Vector2.right);

        if (goingRight > 0.8f)
            goingRightForSeconds += Time.deltaTime;
        else
            goingRightForSeconds = goingRightForSeconds > 0f ? goingRightForSeconds - Time.deltaTime : 0f;

        if (goingRightForSeconds > flipTimer)
        {
            isGoingLeft = false;
            //Debug.Log("Flipped to right");
            goingRightForSeconds = 0f;
        }
    }

}

