using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TransportTcp : MonoBehaviour
{
	public Socket m_socket = null;
	public Socket m_listener = null;
	public bool   m_isServer = false;
	public bool   m_isConnected = false;
	public PacketQueue m_sendQueue = new PacketQueue();
	public PacketQueue m_recvQueue = new PacketQueue();
	
	private EventHandler m_handler;


	public void RegisterEventHandler(EventHandler handler)
	{
		m_handler += handler;
	}

	public void UnregisterEventHandler(EventHandler handler)
	{
		m_handler -= handler;
	}

	public void StartServer(int port, int connectionNum)
	{

		m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_listener.Bind(new IPEndPoint(IPAddress.Any, port));
		m_listener.Listen(connectionNum);
		m_isServer = true;
	}

	public void StopServer()
	{

		m_listener.Close();
		m_listener = null;
		m_isServer = false;
	}

	public bool Connect(string address, int port)
	{
		m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_socket.NoDelay = true;
		m_socket.SendBufferSize = 1024;
		m_socket.Connect(address, port);
		m_isConnected = true;

		return true;
	}

	public bool Disconnect()
	{

		if(m_socket != null)
		{

			bool blockingState = m_socket.Blocking;
			try
			{
		  	  	byte [] tmp = new byte[1];

		   	 	m_socket.Blocking = false;
		   	 	m_socket.Send(tmp, 0, 0);
			}
			catch (SocketException e) 
			{
		    		// 10035 == WSAEWOULDBLOCK
	    	            	if (e.NativeErrorCode.Equals(10035))
			    	{
        				Debug.Log("Still Connected, but the Send would block");
			    	}
    		  		else
    	            		{
		        		Debug.Log("Wait Disconnect: error code:" + e.NativeErrorCode);
    		    		}
			}
			finally
			{
		    		m_socket.Blocking = blockingState;
			}


			if(m_socket.Connected)
			{
				try
				{
					m_socket.Shutdown(SocketShutdown.Both);
				}
				catch (SocketException e)
				{
					Debug.Log("Shutdown: error code:" + e.NativeErrorCode);
				}
				finally
				{
					m_socket.Disconnect(true);
					m_socket = null;
				}
			}
		}

		if(m_handler != null)
		{
			NetEventState state = new NetEventState();
			state.type = NetEventType.Disconnect;
			state.result = NetEventResult.Success;
			m_handler(state);
		}

		m_isConnected = false;

		return true;
	}

	public int Send(byte[] data, int size)
	{
		return m_sendQueue.Enqueue(data, size);
	}

	public int Receive(ref byte[] buffer, int size)
	{
		return m_recvQueue.Dequeue(buffer, size);
	}

	public void AcceptClient()
	{
		//Debug.Log("AcceptClient1");

		if(m_listener != null && m_listener.Poll(0, SelectMode.SelectRead))
		{
			//Debug.Log("AcceptClient2");

			m_socket = m_listener.Accept();
			m_isConnected = true;

			if(m_handler != null)
			{
				//Debug.Log("AcceptClient3");

				NetEventState state = new NetEventState();
				state.type = NetEventType.Connect;
				state.result = NetEventResult.Success;
				m_handler(state);
			}
		}
	}

}
