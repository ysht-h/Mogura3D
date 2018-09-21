using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class mogura : MonoBehaviour
{
	private GameObject gameCtl;
	private gameMgr gamMgrcmp;
	private GameObject tempMogura;

	private float waitTime = 0.0f;
	private float baseWaitTime = 0.0f;
	private int mode = 0;
	private int type = 0;
	private bool isHitTiming = false;
	private float speed = 0.0f;

	// 生成待ち関連
	private const float RANDOM_WAIT_MIN = 0.5f;
	private const float RANDOM_WAIT_MAX = 3.0f;

	// 穴の上での待ち関連
	private const float RANDOM_PUSH_MIN = 0.5f;
	private const float RANDOM_PUSH_MAX = 1.5f;	
	
	// Use this for initialization
	void Start ()
	{		
		gameCtl = GameObject.Find("gameCtl");
		gamMgrcmp = gameCtl.GetComponent<gameMgr>();
		
		waitTime = 0.0f;
		mode = -1;
		type = 0;
		isHitTiming = false;
		speed = 1.5f;		

		baseWaitTime = Random.Range(RANDOM_WAIT_MIN, RANDOM_WAIT_MAX);
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (gamMgrcmp.isEndFlag() || !gamMgrcmp.isPlayFlag())
		{
			return;
		}
		
		// 生成待ち
		if (mode == -1)
		{
			waitTime += Time.deltaTime;
			if (waitTime >= baseWaitTime)
			{
				mode = 0;
				isHitTiming = false;
			}
		}
		else
		// 生成するかどうかの判定
		if (mode == 0)
		{
			int isCreate = Random.Range(0, 10);

			// 生成
			if (isCreate <= 3 || isCreate == 5)
			{
				mode = 1;
				speed = 1.5f;

                if( isCreate == 5)
                {
                	type = 2;
                }
                else
                {
                    // 偶数は味方
                    type = (isCreate % 2 == 0) ? 0 : 1;
                }
				
				tempMogura = Instantiate( gamMgrcmp.moguraObj[type] ) as GameObject;

				Vector3 temppos = new Vector3(transform.position.x - 0.1f, transform.position.y -1.5f, 200.0f);
				tempMogura.transform.position = temppos;
				
				tempMogura.GetComponent<Animator>().Play("out");
			}
			else
			// 生成しない→生成待ちへ			
			{
				baseWaitTime = Random.Range(RANDOM_WAIT_MIN, RANDOM_WAIT_MAX);
				mode = -1;
			}
			waitTime = 0.0f;			
		}
		else
		// 出現アニメ
		if(mode == 1)
		{
//			waitTime += Time.deltaTime;

			Vector3 temppos = tempMogura.transform.position;
			temppos.z -= speed;
			speed *= 2.0f;			
			if (temppos.z < 100.0f)
			{
				temppos.z = 100.0f;
				
				isHitTiming = true;
/*				
			}

			if (waitTime >= 1.0f)
			{
				isHitTiming = true;
			}
			
			if (waitTime >= 3.0f)
			{
*/				
				float basewait = Random.Range(RANDOM_PUSH_MIN, RANDOM_PUSH_MAX);
				baseWaitTime = basewait;

				//temppos.z = 105.0f;

				waitTime = 0.0f;
				mode = 2;
			}
			
			tempMogura.transform.position = temppos;
		}
		else
		// 穴のうえで待つ
		if(mode == 2)
		{
			waitTime += Time.deltaTime;
			if (waitTime >= baseWaitTime)
			{
				tempMogura.GetComponent<Animator>().Play("in");
				waitTime = 0.0f;
				mode = 3;
				speed = 0.375f;
			}			
		}
		else
		// 退場アニメ
		if(mode == 3)
		{
//			waitTime += Time.deltaTime;
			
			Vector3 temppos = tempMogura.transform.position;
			temppos.z += speed;
			speed *= 2.0f;			
			if (temppos.z > 200.0f)
			{
				temppos.z = 200.0f;
				isHitTiming = false;				
/*				
			}
			tempMogura.transform.position = temppos;
			
			if (waitTime >= 2.0f)
			{
				isHitTiming = false;
			}
			
			if (waitTime >= 3.0f)
			{
*/			
				baseWaitTime = Random.Range(RANDOM_WAIT_MIN, RANDOM_WAIT_MAX);

				if (tempMogura)
				{
					Destroy(tempMogura);
				}

				waitTime = 0.0f;
				mode = -1;
			}
		}
	}
/*	
	private GameObject getClickObject() 
	{
		GameObject result = null;
		// 左クリックされた場所のオブジェクトを取得
		if(Input.GetMouseButtonDown(0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit))
			{
				Debug.Log("hit2");
				result = hit.collider.gameObject;
			}
		}
		return result;
	}
*/
	
	
	public void onClick()
	{	
		if (isHitTiming)
		{			
			if (tempMogura)
			{
				GameObject eff = null;

				if (tempMogura.tag.StartsWith("mogura1", System.StringComparison.Ordinal))
				{
					gamMgrcmp.AddScore(100);
					eff = Instantiate(gamMgrcmp.effObj[0]) as GameObject;
				}
				else
				if (tempMogura.tag.StartsWith("mogura3", System.StringComparison.Ordinal))
				{
					gamMgrcmp.AddScore(500);
					eff = Instantiate(gamMgrcmp.effObj[0]) as GameObject;
				}
				else
				{
					gamMgrcmp.AddScore(-50);
					eff = Instantiate(gamMgrcmp.effObj[1]) as GameObject;
				}
				
				if (eff)
				{
					//Destroy(eff, eff.GetComponent<ParticleSystem>().main.duration);
					Vector3 temppos = 
						new Vector3(transform.position.x - 0.1f, transform.position.y - 1.5f, 86.5f);
					eff.transform.position = temppos;
				}				

				Destroy(tempMogura);

				waitTime = 0.0f;
				mode = -1;
			}
		}
	}
}
