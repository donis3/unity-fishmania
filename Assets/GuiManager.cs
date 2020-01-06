using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rhinotap.Toolkit;

public class GuiManager : MonoBehaviour
{
    [SerializeField]
    private Text LevelText;

    // Start is called before the first frame update
    void Start()
    {

        EventManager.StartListening<int>("onLevelUp", (param) => { LevelText.text = "Level " + param.ToString(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
