using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameMgr : MonoBehaviour
{

	private shareData sharemgr;	
	public GameObject[] moguraObj;
	public GameObject[] effObj;
	public GameObject   dangoObj;
	public GameObject   dangoEnemy;
	
	private GameObject score;
	private GameObject gamemsg;
	private GameObject timer;
	private GameObject camera;
	private GameObject larrow;
	private GameObject rarrow;
	private GameObject npc;

	private GameObject doro;
	private GameObject ball1;
	private GameObject ball2;
	private GameObject ball3;
	
	private Text scoretxt;
	private Text gamemsgtxt;
	private Text timetxt;
	
	private bool isPlay = false;
	private float time = 60.0f;
	private int countDown = 3;
	private bool isEnd = false;
	private float endtimer = 0.0f;
	private float counter = 0.0f;	

	private float ballmaketime = 0.0f;
	private int   ballcnt = 0;

	private float cameramove = 0.0f;

	private float doroTime = 0.0f;
	private int alphaAnime = 0;

	private GameObject[] holeobj;
	private mogura[] mogucmp;

	private bool isNetEnd = false;

// Network--
	enum OnlineState
	{
		None = -1,
		Connecting = 0,
		Connected,
		Disconnected,
		Error,
	}

	struct NpcData
	{
		public float x;
		public int usedango;
		public int score;
	};

	struct MoguData
	{
		public int no;
		public int score;
	};

	private List<NpcData> NpcDataList;
	private List<MoguData> NpcMoguraList;

	private TransportTcp m_tcp = null;
	private Thread m_thread = null;
	private bool   m_isStarted = false;
	private bool   m_threadLoop = true;
	private OnlineState m_ost = OnlineState.None;
#if UNITY_EDITOR
	private const string IPADDRESS = "192.168.3.5";
#elif UNITY_IOS
	private const string IPADDRESS = "192.168.3.5";
#else
	private const string IPADDRESS = "192.168.3.5";
#endif
	private const int PORT = 5963;

//----------
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
		npc = GameObject.Find("npc");

		doro = GameObject.Find("doro");
		ball1 = GameObject.Find("ball1");
		ball2 = GameObject.Find("ball2");
		ball3 = GameObject.Find("ball3");

		scoretxt = score.GetComponent<Text>();
		gamemsgtxt = gamemsg.GetComponent<Text>();
		timetxt = timer.GetComponent<Text>();
		
		isPlay = false;
		time = 60.0f;
		countDown = 3;
		isEnd = false;
		endtimer = 0.0f;
		counter = 0.0f;

		ballmaketime = 0.0f;
		ballcnt = 0;

		cameramove = 0.0f;

		doroTime = 0.0f;
		alphaAnime = 0;

		holeobj = new GameObject[12];
		mogucmp = new mogura[12];
		for( int i = 0; i < 12; i++)
		{
			GameObject hole = GameObject.Find("hole_" + i);

			if(hole)
			{

				foreach (Transform transform in hole.transform)
				{
					// Transformからゲームオブジェクト取得・削除
					holeobj[i] = transform.gameObject;

					if (holeobj[i])
					{
						if(holeobj[i].tag.StartsWith("holeparent",
							System.StringComparison.Ordinal))
						{
							mogucmp[i] = holeobj[i].GetComponent<mogura>();
							break;
						}
					}
				}
			}
		}

		isNetEnd = false;

		score.active = false;
		timer.active = false;

		larrow.active = false;
		rarrow.active = false;

		doro.active = false;
		ball1.active = false;
		ball2.active = false;
		ball3.active = false;

		if(!sharemgr.isOnline)
		{
			npc.active = false;
		}
		else
		{
			m_tcp = gameObject.AddComponent<TransportTcp>();

			Connect();
			LaunchThread();
		}

	}
	
	// Update is called once per frame
	void Update()
	{

		if(sharemgr.isOnline)
		{
			bool ret = NetWorkUpdate();

			ret = Recv();

			if(ret || m_ost == OnlineState.Connecting)
			{
				return;
			}
		}

		if (isEnd)
		{
			endtimer += Time.deltaTime;

			if (endtimer >= 2.0f)
			{
				score.active = false;
				timer.active = false;
				StartCoroutine("waitDraw");
				
				//Application.LoadLevel("gameScene");
			}
			else
			if (endtimer >= 1.0f)
			{
				if(sharemgr.isOnline && !isNetEnd)
				{
					DisConnect();
					isNetEnd = true;
				}
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

				MoguraOnline();

				cameraMoveAct();

				getClickObject();

				ballMaker();

				if(sharemgr.isOnline)
				{
					// 受信したデータをパースして叩き込む
					if(NpcDataList != null)
					{
						for( int i = 0; i < NpcDataList.Count; i++)
						{
							enemyMove(NpcDataList[i].x);
							if(NpcDataList[i].usedango == 1)
							{
								enemyShotDango();
							}

							sharemgr.NpcScore = NpcDataList[i].score;
						}

					        // リストを完全にクリア
						NpcDataList.Clear();
						NpcDataList.TrimExcess();
					}

					if(NpcMoguraList != null)
					{
						for( int i = 0; i < NpcMoguraList.Count; i++)
						{
							enemyHitMogura(NpcMoguraList[i].no);

							sharemgr.NpcScore = NpcMoguraList[i].score;
						}

						NpcMoguraList.Clear();
						NpcMoguraList.TrimExcess();
					}
				}


				doroAction();
			}

		}
	}

	private void MoguraOnline()
	{
		if(sharemgr.isOnline)
		{
			// サーバーはモグラの情報を送信
			if(m_tcp.m_isServer)
			{
				for(int i = 0; i < 12; i++)
				{
					int create = mogucmp[i].getIsCreate();
					float time = mogucmp[i].getWaitTime();

					if(create > -1)
					{
						Send("ENEM:" + i + "+" + create + "+" + time + ";");
					}
				}
			}
		}
	}

	private void ballMaker()
	{
		ball1.active = false;
		ball2.active = false;
		ball3.active = false;

		if(ballcnt >= 3)
		{
			ballmaketime = 0.0f;
			ballcnt = 3;

			ball1.active = true;
			ball2.active = true;
			ball3.active = true;
		}
		else
		{
			ballmaketime += Time.deltaTime;
			if (ballmaketime >= 5.0f)
			{
				ballmaketime = 0.0f;
				ballcnt++;
			}

			if(ballcnt >= 2)
			{
				ball1.active = true;
				ball2.active = true;
			}
			else
			if(ballcnt >= 1)
			{
				ball1.active = true;
			}
		}
	}

	public void shotDango()
	{
		if(ballcnt <= 0)
		{
			return;
		}

		GameObject dango = Instantiate(dangoObj) as GameObject;

		if(sharemgr.isOnline)
		{
			Send("DATA:" + camera.transform.position.x + "+" + 1 + "+" + sharemgr.scoreNum + ";");
		}
				
		if(ballcnt == 3)
		{
			ball3.active = false;
		}
		else
		if(ballcnt == 2)
		{
			ball2.active = false;
		}
		else
		if(ballcnt == 1)
		{
			ball1.active = false;
		}

		ballcnt--;
		
		if(ballcnt < 0)
		{
			ballcnt = 0;
		}

/*
	        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        	Collider2D coll = hit.collider;
		
		Debug.Log("shot1");

		if(coll)
		{
			Debug.Log("shot2");
			GameObject obj = coll.gameObject;

		        if (obj.tag.StartsWith("ball", System.StringComparison.Ordinal))
			{
				Debug.Log("shot3");

				GameObject dango = Instantiate(dangoObj) as GameObject;
				
				if(ballcnt == 3)
				{
					ball3.active = false;
				}
				else
				if(ballcnt == 2)
				{
					ball2.active = false;
				}
				else
				{
					ball1.active = false;
				}

				ballcnt--;
				if(ballcnt < 0)
				{
					ballcnt = 0;
				}
			}
		}
*/

	}


	private void cameraMoveAct()
	{
		larrow.active = true;
		rarrow.active = true;		
		
		Vector3 tmppos = camera.transform.position;
#if UNITY_EDITOR
		if( Input.GetKey(KeyCode.LeftArrow)) 
		{
			cameramove -= 0.2f;
			tmppos.x -= 0.2f;
		}
		else
		if( Input.GetKey(KeyCode.RightArrow)) 
		{
			cameramove += 0.2f;
			tmppos.x += 0.2f;
		}
#else
		float tmp = (Input.acceleration.x * 4.0f);
		cameramove += tmp;
		tmppos.x += tmp;
#endif

		if (tmppos.x >= 30.0f)
		{
			cameramove = 30.0f;
			tmppos.x = 30.0f;
			rarrow.active = false;			
		}
		else
		if (tmppos.x <= -30.0f)
		{
			cameramove = -30.0f;
			tmppos.x = -30.0f;
			larrow.active = false;
		}

		camera.transform.position = tmppos;

		if(sharemgr.isOnline)
		{
			Send("DATA:" + tmppos.x + "+" + 0 + "+" + sharemgr.scoreNum + ";");
		}
	}

	private void doroAction()
	{
		if(doro.active)
		{
			doroTime += Time.deltaTime;

			if(alphaAnime == 0)
			{
				if (doroTime >= 4.0f)
				{
					alphaAnime = 1;
					doroTime = 0.0f;
				}
			}
			else
			if(alphaAnime == 1)
			{
				float alpha = 1.0f - doroTime;
				if(alpha <= 0.0f)
				{
					alpha = 0.0f;
				}

				selDoroChangeCol(alpha);

				if (doroTime >= 1.0f)
				{
					alphaAnime = 2;
					doroTime = 0.0f;
					selDoroChangeCol(0.0f);
				}
			}
			else
			{
				if (doroTime >= 1.0f)
				{
					alphaAnime = 0;
					doro.active = false;
					selDoroChangeCol(1.0f);
				}
			}
		}
	}

	private void selDoroChangeCol(float alpha)
    	{
		Graphic sprite = doro.GetComponent<Graphic>();
		Color basecol = sprite.color;
		sprite.color = new Color(basecol.r, basecol.g, basecol.b, alpha);
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
				int moguno = result.GetComponent<mogura>().onClick();

				if(sharemgr.isOnline && moguno > -1)
				{
					Send("MOGU:" + moguno + "+" + sharemgr.scoreNum + ";");
				}
			}
		}
	}

	private void enemyMove(float x)
	{
/*
		// 仮
		int ret = UnityEngine.Random.Range(0, 100);
		if(ret >= 2)
		{
			return;
		}
*/
		Vector3 tmppos = npc.transform.position;
//		tmppos.x = UnityEngine.Random.Range(-30.0f, 30.0f);
		tmppos.x = x;


		npc.transform.position = tmppos;
	}

	private void enemyHitMogura(int holeNo)
	{
/*
		// 仮
		holeNo = UnityEngine.Random.Range(0, 100);
		if(holeNo >= 12)
		{
			return;
		}
*/

/*
		GameObject hole = GameObject.Find("hole_" + holeNo);

		if(hole)
		{
			//Debug.Log(hole.name);

			foreach (Transform transform in holeobj[holeNo].transform)
			{
				// Transformからゲームオブジェクト取得・削除
				GameObject child = transform.gameObject;

				if (child)
				{
					if(child.tag.StartsWith("holeparent",
						System.StringComparison.Ordinal))
					{
						child.GetComponent<mogura>().npcHit();
						break;
					}
				}
			}
		}
*/


		if (mogucmp[holeNo])
		{
			mogucmp[holeNo].npcHit();
		}
	}

	private void enemyShotDango()
	{
/*
		// 仮
		int ene = UnityEngine.Random.Range(0, 100);
		if(ene >= 5)
		{
			return;
		}
*/
		GameObject dangoEne = Instantiate(dangoEnemy) as GameObject;
	}				

	public void setDoro()
	{
		if(!doro.active)
		{
			doro.active = true;
			doroTime = 0.0f;
			alphaAnime = 0;
			selDoroChangeCol(1.0f);
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
	
	public float getCameraMove()
	{
		return cameramove;
	}


    private IEnumerator waitDraw()
    {
        yield return new WaitForEndOfFrame();

        sharemgr.gamebg.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        sharemgr.gamebg.Apply();

	if(sharemgr.isOnline)
	{
	        SceneManager.LoadScene("resultOnlineScene");
	}
	else
	{
	        SceneManager.LoadScene("resultScene");
	}
    }

// Network--



	private void Connect()
	{
#if UNITY_EDITOR
		Debug.Log("Client!");
		bool ret = m_tcp.Connect(IPADDRESS, PORT);
		if(ret)
		{
			m_ost = OnlineState.Connecting;
		}
		else
		{
			m_ost = OnlineState.Error;
		}
#elif UNITY_IOS
		Debug.Log("Server!");
		// iosは強制的にserverとして起動する
		m_tcp.StartServer(PORT, 1);
		m_tcp.m_isServer = true;
		m_ost = OnlineState.Connecting;
#else
		Debug.Log("Client!");
		bool ret = m_tcp.Connect(IPADDRESS, PORT);
		if(ret)
		{
			m_ost = OnlineState.Connecting;
		}
		else
		{
			m_ost = OnlineState.Error;
		}
#endif
	}

	private bool NetWorkUpdate()
	{
		switch(m_ost)
		{
			// 繋がったことをしめす 200:OK を送信
			case OnlineState.Connecting:

				//Debug.Log("connecting...");

				Send("200:OK;");

				return true;
				break;

			case OnlineState.Connected:

				//Debug.Log("connected");

				break;

			case OnlineState.Disconnected:
			case OnlineState.Error:

				Debug.Log("disconnect");
				DisConnect();

				break;			
		}



		return false;
	}

	private void DisConnect()
	{
		if(m_tcp.m_isServer)
		{
			m_tcp.StopServer();
		}
		else
		{
			m_tcp.Disconnect();
		}

		m_ost = OnlineState.None;
	}

	private void Send(string mess)
	{
//		Debug.Log("--send:" + mess);
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(mess);
		m_tcp.Send(buffer, buffer.Length);
	}

	private bool Recv()
	{

		byte[] buffer = new byte[1024];
		string mess;

		//Debug.Log("recv1");

		int recvSize = m_tcp.Receive(ref buffer, buffer.Length);
		if(recvSize > 0)
        	{
			mess = System.Text.Encoding.UTF8.GetString(buffer);
		}
		else
		{
			return false;
		}

		//Debug.Log("recv2");

		bool loop = true;
		string targetstr = ";";
		//string cal = mess;

		NpcDataList = new List<NpcData>();
		NpcMoguraList = new List<MoguData>();

		while(loop)
		{
			if(mess.Length <= 0)
			{
				break;
			}

			int index = mess.IndexOf(targetstr);

			if(index == -1)
			{
				break;
			}

			// ; の前まで
			string tmpmess = mess.Substring(0, index);
			mess = mess.Substring(index + 1);

//			Debug.Log("----recv:" + tmpmess);

			switch(m_ost)
			{
				// 繋がったことをしめす 200:OK を送信
				case OnlineState.Connecting:

					if(tmpmess.StartsWith("200:OK",
						System.StringComparison.Ordinal))
					{
						m_ost = OnlineState.Connected;
					}

					break;

				case OnlineState.Connected:

					if(tmpmess.StartsWith("DATA:",
						System.StringComparison.Ordinal))
					{
						string t1 = tmpmess.Substring(tmpmess.IndexOf(":") + 1);
						string t2 = t1.Substring(0, t1.IndexOf("+"));
						string t3 = t1.Substring(t1.IndexOf("+") + 1);
						string t4 = t3.Substring(0, t3.IndexOf("+"));
						string t5 = t3.Substring(t3.IndexOf("+") + 1);

						NpcData newnpcdata;
						newnpcdata.x = float.Parse(t2);
						newnpcdata.usedango = int.Parse(t4);
						newnpcdata.score = int.Parse(t5);

						NpcDataList.Add(newnpcdata);
					}
					else
					if(tmpmess.StartsWith("MOGU:",
						System.StringComparison.Ordinal))
					{
						string t1 = tmpmess.Substring(tmpmess.IndexOf(":") + 1);
						string t2 = t1.Substring(0, t1.IndexOf("+"));
						string t3 = t1.Substring(t1.IndexOf("+") + 1);

						MoguData mogudata;
						mogudata.no = int.Parse(t2);
						mogudata.score = int.Parse(t3);

						NpcMoguraList.Add(mogudata);
					}
					else
					if(tmpmess.StartsWith("ENEM:",
						System.StringComparison.Ordinal))
					{

						string t1 = tmpmess.Substring(tmpmess.IndexOf(":") + 1);
						string t2 = t1.Substring(0, t1.IndexOf("+"));
						string t3 = t1.Substring(t1.IndexOf("+") + 1);
						string t4 = t3.Substring(0, t3.IndexOf("+"));
						string t5 = t3.Substring(t3.IndexOf("+") + 1);

						int mogurano = int.Parse(t2);
						int create = int.Parse(t4);
						float time = float.Parse(t5);

						if(create > -1)
						{
							mogucmp[mogurano].setOnlineParam(create, time);
						}
					}

					break;
		
			}
		}

		return false;

	}


	private bool LaunchThread()
	{
		try
		{
			m_thread = new Thread(new ThreadStart(Dispatch));
			m_thread.Start();
			Debug.Log("thread start");
		}
		catch
		{
			Debug.Log("Connect launch thread");
			return false;
		}

		m_isStarted = true;

		return true;
	}

	private void Dispatch()
	{
		while(m_threadLoop)
		{
			m_tcp.AcceptClient();

			if(m_tcp.m_socket != null && m_tcp.m_isConnected == true)
			{

				//Debug.Log("Dispatch");
				
				DispatchSend();
				
				DispatchReceive();
			}

			Thread.Sleep(1);
		}
	}


	private void DispatchSend()
	{

		if(m_tcp.m_socket.Poll(0, SelectMode.SelectWrite))
		{
			byte[] buffer = new byte[512];

			int sendSize = m_tcp.m_sendQueue.Dequeue(buffer, buffer.Length);

			while(sendSize > 0)
			{
				m_tcp.m_socket.Send(buffer, sendSize, SocketFlags.None);
				sendSize = m_tcp.m_sendQueue.Dequeue(buffer, buffer.Length);
			}
		}
	}


	private void DispatchReceive()
	{	

		while(m_tcp.m_socket.Poll(0, SelectMode.SelectRead))
		{
			byte[] buffer = new byte[512];

			int recvSize = m_tcp.m_socket.Receive(buffer, buffer.Length, SocketFlags.None);
/*
			if(recvSize == 0)
			{
				m_tcp.Disconnect();
			}
			else
*/
			if(recvSize > 0)
			{
				m_tcp.m_recvQueue.Enqueue(buffer, recvSize);
			}
		}
	}

	public bool isOnlineMode()
	{
		return sharemgr.isOnline;
	}

	public bool isServerMode()
	{
		return m_tcp.m_isServer;
	}

//----------
	
}
