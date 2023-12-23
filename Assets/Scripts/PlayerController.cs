using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.Toolkit;


public class PlayerController : MonoBehaviour
{
    #region INSPECTOR
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float turnSpeed = 1f;

    [SerializeField]
    private int maxLevel = 5;

    [SerializeField]
    private int baseXpRequirement = 100;
    [Range(0.05f, 2f)]
    [SerializeField]
    private float levelXpIncreasePercentage = 0.3f;

    [Header("Sounds")]
    [Space(20)]
    [SerializeField]
    private AudioClip[] biteSounds;
    [SerializeField]
    private AudioClip deathSound;
    #endregion

    #region Internal Vars
    //====================| Components
    Transform playerGraphics;
    AudioSource audioSource;
    TrailRenderer trail;

    //====================| Game Mechanics
    private bool isAlive = true;
    private bool isPaused = false;

    
    private int currentLevelXp = 100;
    private int currentXp = 0;

    public int Level { get; private set; } = 1;

    private int score = 0;


    //====================| Control Input
    //Toggle Pc Controls
    private bool debugOnPc = true;
    //Direction for keyboard controls
    Vector2 direction = Vector2.right;//initial direction
    #endregion

    #region Mono Behaviour
    // Start is called before the first frame update
    void Start()
    {
        playerGraphics = transform.Find("PlayerGraphics");

        trail = GetComponent<TrailRenderer>();

        //Listen to game pause event to update isPaused
        EventManager.StartListening<bool>("gamePaused", (param) => { isPaused = param; });

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.Log(gameObject.name + " is missing audio source component");

        if (Input.touchSupported == true)
            debugOnPc = false;

        

    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        if( isPaused == false )
        {
            /*
            //Input Cycles
            if (debugOnPc == false)
            {
                MobileControls();
            }
            else
            {
                KeyboardControls();
            }
            */
            KeyboardControls();
        }
        
        //Level Management
        if( currentXp >= currentLevelXp)
        {
            //Level up
            LevelUp();
        }

        //DebugCanvas.Set("Xp: " + currentXp.ToString() + " / " + currentLevelXp.ToString(), 2);

    }
    #endregion
    //==============================| Controls |========================//
    void MobileControls()
    {
        MoveTowards(VirtualJoystick.Rotation(true));

        VirtualJoystick.SetFlipTimer(3f);//seconds to pass for flipping
        if (VirtualJoystick.isGoingRight)
        {
            playerGraphics.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            playerGraphics.localScale = new Vector3(1, -1, 1);
        }
    }

    void KeyboardControls()
    {
        //KEYBOARD OVERRIDE
        if (Input.GetAxisRaw("Vertical") != 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        MoveTowards(Quaternion.Euler(0f, 0f, angle));
    }

    //==============================| Movement |========================//

    /// <summary>
    /// Move the player object towards this rotation
    /// </summary>
    /// <param name="rotation"></param>
    public void MoveTowards(Quaternion rotation)
    {
        if (!isAlive) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);

        transform.Translate(Vector2.right * Time.deltaTime * moveSpeed);
    }


    //==============================| Game Mechanics |========================//

    /// <summary>
    /// Run on death event
    /// </summary>
    void Death()
    {
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        GameManager.instance.CameraShake(0.2f, 7f, 2.5f);

        EventManager.Trigger("playerDeath");
        EventManager.Trigger<int>("GameOver", score);
        isAlive = false;
        playerGraphics.gameObject.SetActive(false);
        Destroy(gameObject, 1f);

    }

    void Eat(Fish fish)
    {
        if (fish == null) return;

        //Play sound
        if( audioSource != null && biteSounds.Length > 0)
            audioSource.PlayOneShot(biteSounds[Random.Range(0, biteSounds.Length)]);


        //Shake Camera
        GameManager.instance.CameraShake(0.13f, 5f, 2.5f);

        //Kill the referenced fish

        fish.Die();
        currentXp += fish.Xp;

        score += (fish.Xp * Level);

        GuiManager.instance.SetXp(currentXp, currentLevelXp);

    }

    /// <summary>
    /// Run when game starts
    /// </summary>
    private void GameStart()
    {
        
    }

    private void LevelUp()
    {
        if (Level >= maxLevel)
        {
            //Cant level up anymore
            return;
        }

        currentLevelXp = RequiredXpForLevel(Level, baseXpRequirement);

        currentXp = 0;
        GuiManager.instance.SetXp(currentXp, currentLevelXp);

        Level++;

        transform.localScale = new Vector3(Level, Level, 1);
        EventManager.Trigger<int>("onLevelUp", Level);

        //zoom camera out for each level
        //0.2 for each level
        float cameraMultiplier = 1f + (Level * 0.2f);

        GameManager.instance.CameraZoom(cameraMultiplier, 0.5f);


        //Increase speed
        moveSpeed = moveSpeed * 1.3f;
        turnSpeed = turnSpeed * 1.3f;

        //increase trail
        trail.widthMultiplier += 0.3f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;
        Fish collidedFish = collision.gameObject.GetComponent<Fish>();
        if (collidedFish != null)
        {
            int fishLevel = collidedFish.Level;

            if( fishLevel > Level)
            {
                Death();
            } else
            {
                //Eat
                Eat(collidedFish);
            }
        }
    }

    //==============================| Calculations  |========================//

    private int RequiredXpForLevel(int currentLevel, int xpForFirstLevel)
    {
        if(currentLevel < 1 || xpForFirstLevel < 1)
        {
            return baseXpRequirement;
        }
        float result;
        //Incrmeent xp requirement by %10 each level

        result = (float)xpForFirstLevel * (Mathf.Pow(1f + levelXpIncreasePercentage, currentLevel));

        return Mathf.RoundToInt(result);
    }



}
