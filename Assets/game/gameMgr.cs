using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameMgr : MonoBehaviour
{

	private shareData sharemgr;	
	public GameObject[] moguraObj;
	public GameObject[] effObj;
	
	private GameObject score;
	private GameObject gamemsg;
	private GameObject timer;
	private GameObject camera;
	private GameObject larrow;
	private GameObject rarrow;
	
	private Text scoretxt;
	private Text gamemsgtxt;
	private Text timetxt;
	
	private bool isPlay = false;
	private float time = 60.0f;
	private int countDown = 3;
	private bool isEnd = false;
	private float endtimer = 0.0f;
	private float counter = 0.0f;	
	

	// Use this for initialization
	void Start () 
	{
		sharemgr = shareData.Instance;
		sharemgr.scoreNum = 0;

		score = GameObject.Find("score");
		gamemsg = GameObject.Find("gamemsg");
		timer = GameObject.Find("time");
		camera = GameObject.Find("Main Camera");
		larrow = GameObject.Find("larrow");
		rarrow = GameObject.Find("rarrow");


		scoretxt = score.GetComponent<Text>();
		gamemsgtxt = gamemsg.GetComponent<Text>();
		timetxt = timer.GetComponent<Text>();
		
		isPlay = false;
		time = 60.0f;
		countDown = 3;
		isEnd = false;
		endtimer = 0.0f;
		counter = 0.0f;

		score.active = false;
		timer.active = false;

		larrow.active = false;
		rarrow.active = false;

	}
	
	// Update is called once per frame
	void Update()
	{
		if (isEnd)
		{
			endtimer += Time.deltaTime;
			if (endtimer >= 2.0f)
			{
				//score.active = false;
				//StartCoroutine("waitDraw");
				
				Application.LoadLevel("gameScene");
			}


		}
		else
		{
			if (!isPlay)
			{
				gamemsgtxt.text = countDown.ToString();
				if (countDown > 0)
				{
					counter += Time.deltaTime;
					if (counter >= 1.0f)
					{
						counter = 0.0f;
						countDown -= 1;
					}
				}
				else
				{
					gamemsg.active = false;
					score.active = true;
					timer.active = true;
					larrow.active = true;
					rarrow.active = true;					
					isPlay = true;
					timetxt.text = "Time :" + time.ToString("F1");
				}
			}
			else
			{
				scoretxt.text = "Score:" + sharemgr.scoreNum.ToString();
				time -= Time.deltaTime;

				if (time < 0)
				{
					isPlay = false;
					isEnd = true;
					time = 0.0f;
					gamemsg.active = true;
					gamemsgtxt.text = "Finish!";
				}
				
				timetxt.text = "Time :" + time.ToString("F1");

				cameraMove();

				getClickObject();
			}

		}
	}


	private void cameraMove()
	{
		larrow.active = true;
		rarrow.active = true;		
		
		Vector3 tmppos = camera.transform.position;
#if UNITY_EDITOR
		if( Input.GetKey(KeyCode.LeftArrow)) tmppos.x -= 0.2f;
		else
		if( Input.GetKey(KeyCode.RightArrow)) tmppos.x += 0.2f;
#else		
		tmppos.x += (Input.acceleration.x * 4.0f);		
#endif
		if (tmppos.x >= 30.0f)
		{
			tmppos.x = 30.0f;
			rarrow.active = false;			
		}
		else
		if (tmppos.x <= -30.0f)
		{
			tmppos.x = -30.0f;
			larrow.active = false;
		}

		camera.transform.position = tmppos;
	}
	
	private void getClickObject() 
	{
		GameObject result = null;
		// 左クリックされた場所のオブジェクトを取得
		if(Input.GetMouseButtonDown(0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit))
			{
				result = hit.collider.gameObject;
			}
		}

		if (result)
		{
			if (result.tag.StartsWith("holeparent", System.StringComparison.Ordinal))
			{
				result.GetComponent<mogura>().onClick();
			}
		}
	}	

	public void AddScore(int num)
	{
		sharemgr.scoreNum += num;
		if (sharemgr.scoreNum < 0)
		{
			sharemgr.scoreNum = 0;
		}
	}

	public bool isPlayFlag()
	{
		return isPlay;
	}

	public bool isEndFlag()
	{
		return isEnd;
	}
	
	
}
