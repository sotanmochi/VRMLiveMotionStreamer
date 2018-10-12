using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonVoiceManager : Photon.Pun.MonoBehaviourPunCallbacks
{
	public GameObject PhotonVoicePrefab;

    public override void OnJoinedRoom()
    {
        // Connect photon voice network
        Debug.Log("PhotonVoiceNetwork.ClientState: " + PhotonVoiceNetwork.ClientState.ToString());
        if(PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.PeerCreated)
        {
            PhotonVoiceNetwork.Connect();
            Debug.Log("PhotonVoiceNetwork.Connect() is called.");
        }

        // Instantiate photon voice object
		if(this.PhotonVoicePrefab != null)
        {
			GameObject go = PhotonVoicePrefab;
			go = Photon.Pun.PhotonNetwork.Instantiate(PhotonVoicePrefab.name, Vector3.zero, Quaternion.identity, 0);
			PhotonVoiceRecorder recorder = go.GetComponent<PhotonVoiceRecorder>();
			recorder.enabled = true;
			recorder.Transmit = true;
		}
    }
}
