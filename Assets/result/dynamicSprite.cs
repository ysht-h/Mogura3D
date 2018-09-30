using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicSprite : MonoBehaviour 
{

    public void FillScreen()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

        // カメラの外枠をワールド座標で取得する
        float screenWH = Camera.main.orthographicSize * 2.0f;
        float screenWW = screenWH / Screen.height * Screen.width;

        // スプライトのスケールもワールド座標で取得する
        float w = sr.sprite.bounds.size.x;
        float h = sr.sprite.bounds.size.y;

        // 両方の比率をだしてspriteのローカル座標系に反映
        transform.localScale = new Vector3(screenWW / w, screenWH / h);

        // カメラの中心とスプライトの中心をあわせる
        Vector3 camPos = Camera.main.transform.position;
        camPos.z = 0.0f;
        transform.position = camPos;
    }

	// Use this for initialization
	void Start () 
	{
    }
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
