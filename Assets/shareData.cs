using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shareData : MonoBehaviour 
{

    public static shareData Instance
    {
        get; private set;
    }

    public int scoreNum = 0;
    public Texture2D gamebg = null;
    public bool isOnline = false;
    public int NpcScore = 0;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        scoreNum = 0;
        gamebg = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
	isOnline = false;
	NpcScore = 0;
    }
}
