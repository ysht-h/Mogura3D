using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npcmgr : MonoBehaviour 
{

	public GameObject hiteff;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnCollisionEnter (Collision col)
    	{
		if(col == null)
		{
			return;
		}

        	if (col.gameObject.name.StartsWith("dango_my", System.StringComparison.Ordinal))
        	{
            		Destroy(col.gameObject);

			GameObject eff = Instantiate(hiteff) as GameObject;
			//Destroy(eff, eff.GetComponent<ParticleSystem>().main.duration);
			Vector3 temppos = new Vector3(col.gameObject.transform.position.x, 10.0f, 65.0f);
			eff.transform.position = temppos;
        	}
    	}
}
