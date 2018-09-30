using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sceneMgr : MonoBehaviour 
{

    private int animemode = 0;
    private float animetime = 0.0f;

    private shareData sharemgr;
    private GameObject bg;
    private GameObject basemask;
    private GameObject score;
    private GameObject ranking;
    private GameObject first;
    private GameObject second;
    private GameObject third;
    private GameObject msg1;
    private GameObject msg2;
    private GameObject button;

    private Image baseimg;
    private int score1st = 0;
    private int score2nd = 0;
    private int score3rd = 0;
    private int updatemode = 0;

    const string SCORE_1ST_KEY = "Score1st";
    const string SCORE_2ND_KEY = "Score2nd";
    const string SCORE_3RD_KEY = "Score3rd";

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(SCORE_1ST_KEY, score1st);
        PlayerPrefs.SetInt(SCORE_2ND_KEY, score2nd);
        PlayerPrefs.SetInt(SCORE_3RD_KEY, score3rd);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        score1st = PlayerPrefs.GetInt(SCORE_1ST_KEY, 0);
        score2nd = PlayerPrefs.GetInt(SCORE_2ND_KEY, 0);
        score3rd = PlayerPrefs.GetInt(SCORE_3RD_KEY, 0);
    }

    // Use this for initialization
    void Start () 
    {
        animemode = 0;
        animetime = 0.0f;
        score1st = 0;
        score2nd = 0;
        score3rd = 0;
        updatemode = 0;

        sharemgr = shareData.Instance;

        bg = GameObject.Find("bg");
        basemask = GameObject.Find("base");
        score = GameObject.Find("score");
        ranking = GameObject.Find("ranking");
        first = GameObject.Find("1st");
        second = GameObject.Find("2nd");
        third = GameObject.Find("3rd");
        msg1 = GameObject.Find("msg1");
        msg2 = GameObject.Find("msg2");
        button = GameObject.Find("Button");

        baseimg = basemask.GetComponent<Image>();

        score.active = false;
        first.active = false;
        second.active = false;
        third.active = false;
        ranking.active = false;
        msg1.active = false;
        msg2.active = false;
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

        LoadHighScore();
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
        // score 
        if (animemode == 1)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 2;

                Text temp = score.GetComponent<Text>();
                temp.text = "score:" + sharemgr.scoreNum.ToString();
                score.active = true;
            }

        }
        else
        // ranking 
        if (animemode == 2)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = (score3rd < sharemgr.scoreNum) ? 3: 4;
                Text temp = first.GetComponent<Text>();
                temp.text = "1st:" + score1st.ToString();

                Text temp2 = second.GetComponent<Text>();
                temp2.text = "2nd:" + score2nd.ToString();

                Text temp3 = third.GetComponent<Text>();
                temp3.text = "3rd:" + score3rd.ToString();

                first.active = true;
                second.active = true;
                third.active = true;
                ranking.active = true;
            }

        }
        else
        // Congura,update 
        if (animemode == 3)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 4;

                if (score1st < sharemgr.scoreNum)
                {
                    score3rd= score2nd;
                    score2nd = score1st;
                    score1st = sharemgr.scoreNum;
                    updatemode = 1;
                }
                else
                if (score2nd < sharemgr.scoreNum)
                {
                    score3rd = score2nd;
                    score2nd = sharemgr.scoreNum;
                    updatemode = 2;
                }
                else
                if (score3rd < sharemgr.scoreNum)
                {
                    score3rd = sharemgr.scoreNum;
                    updatemode = 3;
                }

                Text temp = first.GetComponent<Text>();
                temp.text = "1st:" + score1st.ToString();

                Text temp2 = second.GetComponent<Text>();
                temp2.text = "2nd:" + score2nd.ToString();

                Text temp3 = third.GetComponent<Text>();
                temp3.text = "3rd:" + score3rd.ToString();

                Text temp4 = msg2.GetComponent<Text>();
                string numberstr = "";
                if (updatemode == 1) numberstr = "1st";
                else
                if (updatemode == 2) numberstr = "2nd";
                else
                if (updatemode == 3) numberstr = "3rd";

                temp4.text = numberstr + " Update!";

                msg1.active = true;
                msg2.active = true;

                SaveHighScore();

            }

        }
        else
        // score 
        if (animemode == 4)
        {
            animetime += Time.deltaTime;

            if (animetime >= 1.0f)
            {
                animetime = 0.0f;
                animemode = 5;

                button.active = true;
            }

        }
    }

    public void changeScene()
    {
        SceneManager.LoadScene("gameScene");
    }
}
