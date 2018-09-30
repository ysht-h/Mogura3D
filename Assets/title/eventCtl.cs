using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class eventCtl : MonoBehaviour 
{
    private GameObject logo;
    private Graphic logotxt;
    private float animecnt = 0.0f;
    private int animemode = 0;
    private int animewait = 0;

    private shareData sharemgr;	

    public void ChangeScene()
    {
        SceneManager.LoadScene("gameScene");
    }

    // Use this for initialization
    void Start () 
    {
        logo = GameObject.Find("logo");
        logotxt = logo.GetComponent<Graphic>();
        animecnt = 0.0f;
        animemode = 0;
        animewait = 0;

	sharemgr = shareData.Instance;
	sharemgr.isOnline = false;
    }
	
    // Update is called once per frame
    void Update () 
    {
        // 加算
        if(animemode == 0)
        {
            animecnt += Time.deltaTime;
            if (animecnt >= 1.0f)
            {
                animecnt = 1.0f;
                animemode = 1;
                animewait = 0;
            }
        }
        else
        if (animemode == 1)
        {
            animewait++;
            if (animewait >= 50)
            {
                animemode = 2;
            }
        }
        else
        if (animemode == 2)
        {
            animecnt -= Time.deltaTime;
            if (animecnt <= 0.0f)
            {
                animecnt = 0.0f;
                animemode = 3;
                animewait = 0;
            }
        }
        else
        if(animemode == 3)
        {
            animewait++;
            if (animewait >= 10)
            {
                animemode = 0;
            }
        }

        Color temp = logotxt.color;
        temp.a = animecnt;
        logotxt.color = temp;
/*
        if ( Input.GetMouseButtonDown(0) )
        {
            //msg.GetComponent<AudioSource>().Play();
            ChangeScene();

        }
*/
    }


    public void onSelOffline()
    {
	sharemgr.isOnline = false;
	ChangeScene();
    }

    public void onSelOnline()
    {
	sharemgr.isOnline = true;
	ChangeScene();
    }
}
