using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
public class VRMLiveMotionConnectAndJoin : MonoBehaviourPunCallbacks
{
	public string RoomName;

	/// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
	public bool AutoConnect = true;

	/// <summary>Used as PhotonNetwork.GameVersion.</summary>
	public string GameVersion = "1.0";

	/// <summary>Used as RoomOptions.MaxPlayers.</summary>
	public byte MaxPlayers = 10;

	void Start()
	{
		if (this.AutoConnect)
		{
			this.ConnectNow();
		}
	}

	public void ConnectNow()
	{
		Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
		PhotonNetwork.GameVersion = this.GameVersion;
		PhotonNetwork.ConnectUsingSettings();
	}

	public void JoinRoom()
	{
		if (!string.IsNullOrEmpty(RoomName))
		{
			Debug.Log("PhotonNetwork.JoinRandomRoom() was called at OnJoinedLobby().");
			PhotonNetwork.JoinOrCreateRoom(RoomName, new RoomOptions() { MaxPlayers = this.MaxPlayers }, TypedLobby.Default);
		}
		else
		{
			Debug.Log("PhotonNetwork.JoinRandomRoom() was called at OnJoinedLobby().");
			PhotonNetwork.JoinRandomRoom();
		}
	}

	// below, we implement some callbacks of the Photon Realtime API.
	// Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


	public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		this.JoinRoom();
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby() was called by PUN.");
		this.JoinRoom();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("OnJoinRandomFailed() was called by PUN.");
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.MaxPlayers }, null);
	}

	// the following methods are implemented to give you some context. re-implement them as needed.
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("OnDisconnected("+cause+")");
		if (cause == DisconnectCause.None || cause == DisconnectCause.TimeoutDisconnect)
		{
			this.ConnectNow();
		}
	}

	public override void OnJoinedRoom()
	{
		string currentRoom = PhotonNetwork.CurrentRoom.Name;
		Debug.LogFormat("OnJoinedRoom() called by PUN.");
		Debug.LogFormat("Current room: {0}", currentRoom);
	}
}