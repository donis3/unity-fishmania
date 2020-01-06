using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class RTween
{
    private static AnimationCurve easeinout = new AnimationCurve(
        new Keyframe(0f, 0f, 0.05f, 0.05f),
        new Keyframe(1f, 1f, .05f, .05f)
        );

    /// <summary>
    /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
    /// acceleration until halfway, then deceleration.
    /// </summary>
    /// <param name="t">Current time in seconds.</param>
    /// <param name="b">Starting value.</param>
    /// <param name="c">Final value.</param>
    /// <param name="d">Duration of animation.</param>
    /// <returns>The correct value.</returns>
    public static float SineEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1)
            return c / 2 * (Mathf.Sin(Mathf.PI * t / 2)) + b;

        return -c / 2 * (Mathf.Cos(Mathf.PI * --t / 2) - 2) + b;
    }


    public static float EaseInOut(float percentage)
    {
        if( percentage > 1f)
        {
            percentage = 1f;
        }else if( percentage < 0f)
        {
            percentage = 0f;
        }

        return easeinout.Evaluate(percentage);


        
    }
    
}
