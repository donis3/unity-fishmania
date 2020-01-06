using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof(CanvasGroup))]
public class DebugCanvas : MonoBehaviour
{
    #region Singleton Instance
    private static DebugCanvas _instance; //instance of this object
    public static DebugCanvas instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DebugCanvas>();//find the instance in the scene
                if (_instance == null)
                    Debug.LogError("Mobile Input Manager Not available in the scene");
            }
            return _instance;
        }
    }
    #endregion

    bool isRunning = false;
    CanvasGroup canvasGrp;
    Image panel;
    Text line1;
    Text line2;
    Text line3;
    Text title;

    // Start is called before the first frame update
    void Start()
    {
        canvasGrp = GetComponent<CanvasGroup>();
        panel = transform.Find("DebugBg")?.GetComponent<Image>();
        line1 = transform.Find("DebugBg/Line1")?.GetComponent<Text>();
        line2 = transform.Find("DebugBg/Line2")?.GetComponent<Text>();
        line3 = transform.Find("DebugBg/Line3")?.GetComponent<Text>();
        title = transform.Find("DebugBg/DebugTitle")?.GetComponent<Text>();

        if( canvasGrp == null ||
            panel == null ||
            line1 == null ||
            line2 == null ||
            line3 == null ||
            title == null
            )
        {
            //Missing Component
            Debug.LogError("DebugCanvas has a missing component. Please check");
            isRunning = false;
        } else
        {
            isRunning = true;
        }
    }

    void SetLine(string msg, int line = 1)
    {
        if (!isRunning)
            return;

        if( line < 1 || line > 3)
        {
            line = 3;
        }

        if (line == 1)
            line1.text = msg;
        else if (line == 2)
            line2.text = msg;
        else if (line == 3)
            line3.text = msg;

    }

    void SetTitle(string msg = "Debug Info")
    {
        if (!isRunning)
            return;

        title.text = msg;
    }

    

    public static void Set(string msg = null, int lineNumber = 1)
    {
        instance.SetLine(msg, lineNumber);
    }

    public static void Title(string msg = null)
    {
        instance.SetTitle(msg);
    }


}
