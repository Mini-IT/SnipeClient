using UnityEngine;
using System.Collections;
using MiniIT;
using MiniIT.Snipe;

public class MainApp : MonoBehaviour
{
	private const string SERVER_HOST = "192.168.0.100";
	private const int    SERVER_PORT = 3010;

	private SnipeClient mClient;

	void Start ()
	{
		// Creating client
		mClient = new SnipeClient();

		// Adding events listeners
		mClient.ConnectionSucceeded += OnConnected;
		mClient.ConnectionFailed += OnConnectionFailed;
		mClient.ConnectionLost += OnConnectionLost;
		mClient.DataReceived += OnServerResponse;

		// Trying to connect
		mClient.Connect(SERVER_HOST, SERVER_PORT);
	}

	void OnConnected (DataEventArgs data)
	{
		Debug.Log("Connected successfully");

		// trying to send request
		ExpandoObject parameters = new ExpandoObject();
		parameters["name"] = "testname";
		parameters["password"] = "";
		mClient.SendRequest("user.login", parameters);
	}

	void OnConnectionFailed (DataEventArgs data)
	{
		Debug.Log("Connection failed");
	}

	void OnConnectionLost (DataEventArgs data)
	{
		Debug.Log("Connection lost");
	}

	void OnServerResponse (DataEventArgs data)
	{
		Debug.Log("Server says: " + data.Data.ToJSONString());
	}
}
