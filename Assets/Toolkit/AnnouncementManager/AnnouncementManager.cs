using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementManager : MonoBehaviour
{
    bool hasError = false;
    Text announcementTxt;
    Image announcementIcon;
    CanvasGroup canvasGroup;
    

    Queue<AnnouncementItem> msgQue = new Queue<AnnouncementItem>();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        announcementIcon = transform.Find("announcementBg/announcementIcon").GetComponent<Image>();
        announcementTxt = transform.Find("announcementBg/announcementTxt").GetComponent<Text>();

        if (canvasGroup == null)
        {
            hasError = true;
        }


        DisableCanvas();
        announcementIcon.gameObject.SetActive(false);
        announcementTxt.gameObject.SetActive(false);

        

        
    }

    private void Start()
    {
        if(!hasError)
            StartCoroutine(manageAnnouncements());


        //Examples
        //Announce("selam!", "Sprites/icons2", "heart", 1f);
        //Announce("Bi git ak!!", "Sprites/check", 2f);
    }


    public void Announce(string text, Sprite image = null, float duration = 1f)
    {
        AnnouncementItem newMsg = new AnnouncementItem(text, image, duration);
        msgQue.Enqueue(newMsg);
    }

    public void Announce( string text, string spritePath, string spriteName, float duration = 1f)
    {
        AnnouncementItem newMsg = new AnnouncementItem(text, spritePath, spriteName, duration);
        msgQue.Enqueue(newMsg);
    }

    public void Announce(string text, string spritePath,  float duration = 1f)
    {
        AnnouncementItem newMsg = new AnnouncementItem(text, spritePath, duration);
        msgQue.Enqueue(newMsg);
    }


    IEnumerator announcementCoroutine( string text = "", Sprite image = null, float duration = 1f)
    {
        if( hasError)
        {
            yield break;
        }
        if( announcementTxt == null)
        {
            Debug.LogWarning("Announcement Manager: missing UI object: announcementBg/announcementTxt");
            yield break;
        }
        if( image != null && announcementIcon == null)
        {
            Debug.LogWarning("Announcement Manager: missing UI object: announcementBg/announcementIcon");
            yield break;
        }
        announcementTxt.gameObject.SetActive(true);
        announcementTxt.text = text;

        if( image != null && announcementIcon != null)
        {
            announcementIcon.gameObject.SetActive(true);
            announcementIcon.sprite = image;
        }
        

        while ( canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += 0.05f;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        DisableCanvas();
        announcementTxt.text = "";
        announcementTxt.gameObject.SetActive(false);
        announcementIcon.sprite = null;
        announcementIcon.gameObject.SetActive(false);
    }


    IEnumerator manageAnnouncements()
    {
        while(true)
        {
            if( msgQue.Count == 0)
                yield return new WaitUntil(() => msgQue.Count > 0);

            AnnouncementItem msg = msgQue.Dequeue();

            StartCoroutine(announcementCoroutine(msg.text, msg.img, msg.duration));

            yield return new WaitForSeconds(msg.duration + 2f);
        }
    }


    //Canvas Group Controls
    void EnableCanvas()
    {

        if( hasError)
        {
            Debug.LogWarning("Announcement Manager: Canvas Group component missing");
            return;
        }
        
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    void DisableCanvas()
    {
        if (hasError)
        {
            Debug.LogWarning("Announcement Manager: Canvas Group component missing");
            return;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}

public class AnnouncementItem
{
    public string text = null;
    public Sprite img = null;
    public float duration = 4f;

    public AnnouncementItem(string _text, Sprite _img = null, float _duration = 0f)
    {
        text = _text;
        if (_img != null)
            img = _img;
        if (_duration > 0.1f)
            duration = _duration;
    }

    public AnnouncementItem(string _text, string spritesheetPath, string spriteName, float _duration = 0f)
    {
        text = _text;

        Sprite[] sheet = Resources.LoadAll<Sprite>(spritesheetPath);

        if( sheet.Length > 0)
        {
            img =  sheet.Single(s => s.name == spriteName);
        }

        if (_duration > 0.1f)
            duration = _duration;
    }

    public AnnouncementItem(string _text, string spritePath, float _duration = 0f)
    {
        text = _text;

        img = Resources.Load<Sprite>(spritePath);

        if (_duration > 0.1f)
            duration = _duration;
    }

}