using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
public class VRMLiveMotionConnectAndJoinRandom : MonoBehaviourPunCallbacks
{
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

	// below, we implement some callbacks of the Photon Realtime API.
	// Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


	public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
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
		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running.");
	}
}