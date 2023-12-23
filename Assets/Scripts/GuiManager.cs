using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rhinotap.Toolkit;

public class GuiManager : Singleton<GuiManager>
{
    

    [SerializeField]
    private Image XpBar;


    [SerializeField]
    private GameObject pauseBtn;
    [SerializeField]
    private GameObject resumeBtn;
    [SerializeField]
    private GameObject pausedBg;

    [SerializeField]
    private GameObject ScoreScreen;
    [SerializeField]
    private Text ScoreText;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("GameStart", () => {
            SetXp(0, 1);
            HideScore();
        });
        

        EventManager.StartListening<bool>("gamePaused", (isPaused) => {
            TogglePauseBtn();
        });


        EventManager.StartListening<int>("GameOver", (score) => {
            ShowScore(score);
        });
    }

    public void SetXp(int currentXP, int maxXp)
    {
        if (XpBar == null) return;
        float percentage = (float)currentXP / (float)maxXp;

        XpBar.fillAmount = percentage;

    }

    private void TogglePauseBtn()
    {
        if( pauseBtn == null || resumeBtn == null)
        {
            Debug.Log("Missing pause/resume btns");
            return;
        }

        
        if( pauseBtn.activeSelf == true)
        {
            //game paused
            pauseBtn.SetActive(false);
            resumeBtn.SetActive(true);
            pausedBg.SetActive(true);
        }else
        {
            //Resume
            pauseBtn.SetActive(true);
            resumeBtn.SetActive(false);
            pausedBg.SetActive(false);
        }
    }


    private void ShowScore(int score = 0)
    {
        if (ScoreScreen == null || ScoreText == null) return;
        ScoreScreen.SetActive(true);
        ScoreText.text = score.ToString();

    }

    private void HideScore()
    {
        if (ScoreScreen == null || ScoreText == null) return;
        ScoreScreen.SetActive(false);
        ScoreText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
