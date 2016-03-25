using UnityEngine;
using System.Collections;
using MiniIT;
using MiniIT.Snipe;

public class MainApp : MonoBehaviour
{
	private const string SERVER_HOST = "192.168.0.100";
	private const int    SERVER_PORT = 2501;

	private SnipeClient mClient;

	void Start ()
	{
		Log("Start");

		// Creating client
		mClient = SnipeClient.Instance; //new SnipeClient();

		// Adding events listeners
		mClient.ConnectionSucceeded += OnConnected;
		mClient.ConnectionFailed += OnConnectionFailed;
		mClient.ConnectionLost += OnConnectionLost;
		mClient.DataReceived += OnServerResponse;

		// Trying to connect
		Log("Trying to connect");

		//mClient.Connect(SERVER_HOST, SERVER_PORT);  // connect using TCP Client
		mClient.ConnectWebSocket(SERVER_HOST, SERVER_PORT);    // connect using WebSocket
		//mClient.ConnectWebSocket("ws://192.168.0.100:2501/");  // connect using WebSocket
	}

	void OnConnected (DataEventArgs data)
	{
		Log("Connected successfully");

		// trying to send request
		ExpandoObject parameters = new ExpandoObject();
		parameters["name"] = "testname";
		parameters["password"] = "";
		mClient.SendRequest("user.login", parameters);
	}

	void OnConnectionFailed (DataEventArgs data)
	{
		Log("Connection failed");
	}

	void OnConnectionLost (DataEventArgs data)
	{
		Log("Connection lost");
	}

	void OnServerResponse (DataEventArgs data)
	{
		Log("Server says: " + data.Data.ToJSONString());
	}

	void OnApplicationQuit()
	{
		if (mClient != null)
			mClient.Disconnect();
	}

	#region Log
	
	private string mLog = "";
	
	void Log(string text)
	{
		Debug.Log(text);
		mLog += text + "\n";
	}
	
	void OnGUI()
	{
		mLog = GUI.TextArea(new Rect (10, 10, Screen.width-20, Screen.height-20), mLog);
	}
	
	#endregion
}
