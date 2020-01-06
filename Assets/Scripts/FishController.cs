using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.States;
using Rhinotap.Toolkit;

public class FishController : StateMachine
{
    #region Inspector Fields
    //==============| INSPECTOR |============//

    [Header("Fish Movement")]
    [Space(20)]
    [Range(0.1f, 100f)]
    [SerializeField]
    private float speed = 0.1f;

    [Range(0.1f, 100f)]
    [SerializeField]
    private float timeBetweenMovement = 1f;


    //will be applied on Y axis when the object is moving to move it on a curved path
    [SerializeField]
    private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Fish movement limiters on each axis")]
    [Space(10)]
    [Range(0f, 20f)]
    [SerializeField]
    private float radiusY = 1f;
    
    [Range(0f, 20f)]
    [SerializeField]
    private float radiusX = 6f;

    [Header("Fish Level")]
    [Space(20)]
    [SerializeField]
    private int level = 1;
    #endregion



    public int Level { get { return level; } }

    public int XpValue { get { return level; } }

    public float TimeBetweenMovement { get { return timeBetweenMovement; } }


    //Fish limits
    [HideInInspector]
    public Vector2 FishMovementLimit { get { return new Vector2(radiusX, radiusY); } }


    //============|Internal Trackers|===========//
    bool isLookingRight = true;
    Vector2 destination = Vector2.zero;

    bool isPaused = false;

    //Movement coroutine holder
    Coroutine currentMovementRoutine;
    //is the object currently moving
    public bool isMoving { get; private set; } = false;


    AudioSource audioSource;


    //===========| Public API |===============//
    public bool isAlive { get; private set; } = true;

    public void ResetFish()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Setup State Machine
        AddState(new Fishes.Standard.StateIdle(), true);
        StartMachine();
        destination = transform.position;

        EventManager.StartListening<int>("onLevelUp", (playerLevel) => {
            speed = speed * Mathf.Pow(1.3f, playerLevel);
        });


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Reverse scale on X axis
    /// </summary>
    public void Flip()
    {
        
        isLookingRight = !isLookingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Flip when needed
    /// </summary>
    void ManageOrientation()
    {
        
        if(destination.x < transform.position.x )
        {//Will move towards left
            if (isLookingRight)
                Flip();
        }
        else if( destination.x > transform.position.x)
        {//Will move towards right
            if (!isLookingRight)
                Flip();
        }
    }


    /// <summary>
    /// Moves the current object from a to b with easing and adds curve to path on Y axis
    /// </summary>
    /// <param name="from">Starting position</param>
    /// <param name="to">Destination</param>
    /// <param name="curvedPath">Add curve to path?</param>
    /// <returns></returns>
    IEnumerator EaseMoveTo(Vector2 from, Vector2 to, bool curvedPath = false)
    {
        


        //if (curvedPath) Debug.Log("Starting to move on a curved path");
        //else Debug.Log("Starting to move on a lineer path");

        //Track total time
        float time = 0f;
        //track total loops
        int loopCounter = 0;
        //animation percentage
        float percentage;

        //Flip if needed
        ManageOrientation();
        
        //Loop till we reach destination
        while( Vector2.Distance(transform.position, to) > 0.1f)
        {
            //Set moving to true.
            isMoving = true;
            //un-eased percentage of animation
            percentage = time * speed;
            //eased percentage of animation
            float tweenPercentage = RTween.EaseInOut(percentage);
            //calculate current position that we are supposed to be at
            Vector2 currentPos = Vector2.Lerp(from, to, tweenPercentage);
            //apply curved path on y axis if needed
            if( curvedPath == true)
                currentPos.y += movementCurve.Evaluate(time);
            //Move to object to calculated position
            transform.position = currentPos;
            //Increment passed time
            time += Time.deltaTime;
            //Increment loop counter
            loopCounter++;
            //Let 1 frame to pass
            yield return null;
            //Emergency exit
            if (loopCounter > 1000)
            {
                isMoving = false;
                yield break;
            }
        }
        //After movement is done, set is moving to false
        isMoving = false;
        yield return null;
    }



    
    /// <summary>
    /// Starts movement to destination if the object is not currently moving
    /// </summary>
    /// <param name="_destination">Target destination to move to</param>
    public void SetDestination(Vector2 _destination)
    {
        if (!isAlive)
            return;

        if (isMoving)
            return;

        destination = _destination;

        if (currentMovementRoutine != null)
        {
            StopCoroutine(currentMovementRoutine);
        }


        if( Vector2.Distance(transform.position, destination) < 1f)
        {
            //No need for curve
            currentMovementRoutine = StartCoroutine(EaseMoveTo(transform.position, destination, false));
        }
        else
        {
            //Curved move
            currentMovementRoutine = StartCoroutine(EaseMoveTo(transform.position, destination, true));
        }
    }



    public void Die()
    {
        if(currentMovementRoutine != null)
            StopCoroutine(currentMovementRoutine);

        isAlive = false;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
