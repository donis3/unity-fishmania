using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rhinotap.Toolkit;
using Cinemachine;
public class GameManager : MonoBehaviour
{
    #region Game Wide Singleton
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //==============| STATIC API |=====================//
    public static bool Paused { get { return instance.isPaused; } }


    //==============| INSTANCED API |======================//
    public bool isPaused { get; private set; } = false;


    public  void PlayPause()
    {
        isPaused = !isPaused;

        EventManager.Trigger<bool>("gamePaused", isPaused);

        return;
        if( isPaused)
        {
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
        }
    }



    //Mono
    private void Start()
    {
        //Get Virtual Camera Defaults
        GetVcamComponents();
        if (Vcam != null)
            CameraDefaultSize = Vcam.m_Lens.OrthographicSize;

        
    }



    //==============| Cinemachine |======================//
    CinemachineVirtualCamera Vcam;
    CinemachineBasicMultiChannelPerlin VcamNoise;
    Coroutine cameraShake;
    Coroutine cameraZoom;
    float CameraDefaultSize = 7f;
    /// <summary>
    /// Shake the camera for a duration with given settings. Requires cinemachine Virtual Camera in the scene
    /// </summary>
    /// <param name="time">Duration</param>
    /// <param name="amplitudeGain"></param>
    /// <param name="frequencyGain"></param>
    public void CameraShake(float time = 1f, float amplitudeGain = 4f, float frequencyGain = 4f)
    {
        //Verify components
        GetVcamComponents();
        if (Vcam == null || VcamNoise == null) return;

        //Verify duration
        if (time <= 0f) { time = 0f; }

        //Stop old coroutine
        if (cameraShake != null)
        {
            StopCoroutine(cameraShake);
        }
        //if time is 0, no need to shake
        if( time <= 0f)
        {
            return;
        }
        //start new coroutine
        cameraShake = StartCoroutine(CinemachineShake(time, amplitudeGain, frequencyGain));

    }
    private void GetVcamComponents()
    {
        if (Vcam == null)
            Vcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        if (VcamNoise == null && Vcam != null)
            VcamNoise = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (Vcam == null)
            Debug.Log("Missing game object: Cinemachine Virtual Camera");
        if (VcamNoise == null)
            Debug.Log("Missing Cinemachine Camera Component: Noise");
    }

    
    
    IEnumerator CinemachineShake( float time, float amplitude, float frequency)
    {
        if (Vcam == null || VcamNoise == null) yield break;

        //Start shaking
        VcamNoise.m_AmplitudeGain = amplitude;
        VcamNoise.m_FrequencyGain = frequency;

        //Wait for duration
        yield return new WaitForSeconds(time);

        //revert
        VcamNoise.m_AmplitudeGain = 0f;
        VcamNoise.m_FrequencyGain = 0f;
        yield break;
    }


    public void ResetCameraSize()
    {
        Vcam.m_Lens.OrthographicSize = CameraDefaultSize;
    }

    /// <summary>
    /// Original camera size will be multiplied with the param. If size is 10 and multiplier is 1.2 new size will be 12
    /// </summary>
    /// <param name="multiplier"></param>
    /// <param name="transitionTime">How long it should take to transition</param>
    public void CameraZoom(float multiplier, float transitionTime = 0.5f)
    {
        if( multiplier <= 0f) { multiplier = 0.1f; } //min
        if(multiplier >= 3f) { multiplier = 3f; } //max

        float currentSize = Vcam.m_Lens.OrthographicSize;
        float newSize = CameraDefaultSize * multiplier;

        if (cameraZoom != null)
            StopCoroutine(cameraZoom);

        cameraZoom = StartCoroutine(CameraZoomRoutine(currentSize, newSize, transitionTime));

    }
    IEnumerator CameraZoomRoutine(float currentSize, float targetSize, float duration)
    {
        float time = 0f;
        while( time < duration)
        {
            time += Time.deltaTime;
            float percentage = time / duration;
            float size = Mathf.Lerp(currentSize, targetSize, percentage);
            Vcam.m_Lens.OrthographicSize = size;
            yield return null;
        }
    }

    //==============| /Cinemachine |======================//
}
