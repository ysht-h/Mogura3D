using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballenemy : MonoBehaviour 
{
	private GameObject npc;
	private GameObject doro;
	private GameObject gameCtl;
	private gameMgr gamMgrcmp;

	private float speed = 4.0f;

	// Use this for initialization
	void Start () 
	{
		npc = GameObject.Find("npc");
		gameCtl = GameObject.Find("gameCtl");
		gamMgrcmp = gameCtl.GetComponent<gameMgr>();

		Vector3 tempos = transform.position;
		tempos.x = npc.transform.position.x;
		tempos.y = npc.transform.position.y;
		//tempos.x += gamMgrcmp.getCameraMove();
		transform.position = tempos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//speed *= 1.2f;

		Vector3 tempos = transform.position;
		tempos.y -= speed;
		transform.position = tempos;

	        transform.Rotate(new Vector3(360, 0, 0) * Time.deltaTime, Space.World);

		if(transform.position.y <= -65.0f)
		{
			Destroy(this.gameObject);
		}		
	}

	void OnCollisionEnter (Collision col)
    	{
		if(col == null)
		{
			return;
		}

        	if (col.gameObject.name.StartsWith("Main Camera", System.StringComparison.Ordinal))
        	{
			gamMgrcmp.setDoro();


            		Destroy(this.gameObject);			
        	}
    	}

}
