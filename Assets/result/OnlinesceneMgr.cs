using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlinesceneMgr : MonoBehaviour 
{

    private int animemode = 0;
    private float animetime = 0.0f;

    private shareData sharemgr;
    private GameObject bg;
    private GameObject basemask;
    private GameObject ranking;
    private GameObject second;
    private GameObject third;
    private GameObject msg1;
    private GameObject button;

    private Image baseimg;
    private int updatemode = 0;

    // Use this for initialization
    void Start () 
    {
        animemode = 0;
        animetime = 0.0f;
        updatemode = 0;

        sharemgr = shareData.Instance;

        bg = GameObject.Find("bg");
        basemask = GameObject.Find("base");

        ranking = GameObject.Find("ranking");
        second = GameObject.Find("2nd");
        third = GameObject.Find("3rd");
        msg1 = GameObject.Find("msg1");
        button = GameObject.Find("Button");

        baseimg = basemask.GetComponent<Image>();

        second.active = false;
        third.active = false;
        ranking.active = false;
        msg1.active = false;
        button.active = false;

        // UI/Imageだと画面と同じサイズであるにも関わらずサイズにズレがでる。
        // そのためSpriteとして扱いピッタリのサイズで扱えるようにする
        /*
        Image bgimg = bg.GetComponent<Image>();
        bgimg.sprite = Sprite.Create(sharemgr.gamebg, new Rect(0, 0, sharemgr.gamebg.width, sharemgr.gamebg.height), Vector2.zero);

        RectTransform t = bg.GetComponent<RectTransform>();

        // 設定
        Vector2 sizeDelta = t.sizeDelta;
        sizeDelta = new Vector2(Screen.width, Screen.height);
        t.sizeDelta = sizeDelta;
        */

        Vector2 pivot = new Vector2(0.5f, 0.5f);
        SpriteRenderer bgimg = bg.GetComponent<SpriteRenderer>();
        bgimg.sprite = Sprite.Create(sharemgr.gamebg, new Rect(0, 0, sharemgr.gamebg.width, sharemgr.gamebg.height), pivot);

        bg.GetComponent<dynamicSprite>().FillScreen();
    }
	
    // Update is called once per frame
    void Update () 
    {
        // basmask alphaanime
	if(animemode == 0)
        {
            animetime += Time.deltaTime / 2.0f;

            Color temp = baseimg.color;
            temp.a = animetime;
            baseimg.color = temp;

            if (animetime >= 0.5f)
            {
                temp.a = 0.5f;
                baseimg.color = temp;
                animetime = 0.0f;
                animemode = 1;
            }

        }
        else
        // ranking 
        if (animemode == 1)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 2;

                Text temp2 = second.GetComponent<Text>();
                temp2.text = "Your Score:" + sharemgr.scoreNum.ToString();

                Text temp3 = third.GetComponent<Text>();
                temp3.text = "Rival Score:" + sharemgr.NpcScore.ToString();

                second.active = true;
                third.active = true;
                ranking.active = true;
            }

        }
        else
        // Congura,update 
        if (animemode == 2)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 3;
		Text temp = msg1.GetComponent<Text>();

                if (sharemgr.scoreNum < sharemgr.NpcScore)
                {
        	        temp.text = "You Lose...";
			temp.color = new Color(255, 0, 0, 255);
                }
		else
                if (sharemgr.scoreNum == sharemgr.NpcScore)
                {
        	        temp.text = "Draw!";
			temp.color = new Color(255, 255, 255, 255);
                }		

                msg1.active = true;

            }

        }
        else
        // score 
        if (animemode == 3)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 4;

                button.active = true;
            }

        }
    }

    public void changeScene()
    {
        SceneManager.LoadScene("gameScene");
    }
}
