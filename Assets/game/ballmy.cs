using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballmy : MonoBehaviour 
{
	private GameObject gameCtl;
	private gameMgr gamMgrcmp;

	private float speed = 4.0f;

	// Use this for initialization
	void Start () 
	{
		gameCtl = GameObject.Find("gameCtl");
		gamMgrcmp = gameCtl.GetComponent<gameMgr>();

		Vector3 tempos = transform.position;
		tempos.x += gamMgrcmp.getCameraMove();
		transform.position = tempos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//speed *= 1.2f;

		Vector3 tempos = transform.position;
		tempos.y += speed;
		transform.position = tempos;

	        transform.Rotate(new Vector3(360, 0, 0) * Time.deltaTime, Space.World);

		if(transform.position.y >= 70.0f)
		{
			Destroy(this.gameObject);
		}		
	}
}
