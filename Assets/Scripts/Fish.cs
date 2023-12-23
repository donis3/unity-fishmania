using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fish : MonoBehaviour
{
    private bool hasError = false;
    private Transform gfx;
    private Sprite image;

    [SerializeField]
    private int level = 0;
    public int Level => level;

    [SerializeField]
    private int xp = 0;
    public int Xp => xp;

    [SerializeField]
    private bool isFacingRight = true;
    public bool IsFacingRight => isFacingRight;

    [SerializeField]
    private float speed = 1f;
    public float Speed => speed;



    private void Awake()
    {
        gfx = GetComponentInChildren<SpriteRenderer>()?.transform;
        if (gfx != null)
        {
            image = gfx.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            Debug.Log("Cant find child sprite renderer in fish");
            hasError = true;
        }


        EvaluateFish();
    }



    public void Die()
    {
        Destroy(gameObject);
    }



    private void EvaluateFish()
    {
        if (hasError) return;
        if (level <= 0) return;

        //scale to level
        Vector3 scale = new Vector3(transform.localScale.x * Level, transform.localScale.y * Level, 1);
        transform.localScale = scale;

    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
    }

    public void TurnLeft()
    {
        //Already looking left
        if (!isFacingRight) return;

        Flip();
    }

    public void TurnRight()
    {
        //Already looking right
        if (isFacingRight) return;
        Flip();
    }

    public void FlipTowardsDestination(Vector2 _destination, bool localSpace = true)
    {
        if(localSpace)
        {
            if (_destination.x < transform.localPosition.x)
                TurnLeft();
            else if(_destination.x > transform.localPosition.x)
                TurnRight();

            return;
        }
        else
        {
            if (_destination.x < transform.position.x)
                TurnLeft();
            else if (_destination.x > transform.position.x)
                TurnRight();
        }

        
    }


}

