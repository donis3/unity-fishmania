using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class RhinotapExtension
{
    //2D rotation
    public static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    public static void Spin(this Transform transform, float speed = 1f, bool clockwise = true)
    {
        if( !clockwise)
            transform.Rotate(Vector3.forward * speed * 20f * Time.deltaTime);
        else
            transform.Rotate(Vector3.back * speed * 20f * Time.deltaTime);
    }


    public static Sprite LoadResourceSprite(this Sprite original, string spritesheetPath, string spriteName)
    {
        Sprite[] sheet = Resources.LoadAll<Sprite>(spritesheetPath);
        Sprite targetImage;
        if( sheet.Length > 0)
        {
            targetImage = sheet.Single(s => s.name == spriteName);
            if( targetImage == null)
            {
                Debug.Log("Couldn't load " + spriteName + " from " + spritesheetPath);
            }
            original = targetImage;
            
            return targetImage;
        }
        // Get specific sprite
        return original;
        

    }
}
