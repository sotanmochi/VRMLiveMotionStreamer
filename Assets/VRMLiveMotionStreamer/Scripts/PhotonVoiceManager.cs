using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonVoiceManager : Photon.Pun.MonoBehaviourPunCallbacks
{
	public GameObject PhotonVoicePrefab;

    public override void OnJoinedRoom()
    {
        // Instantiate photon voice object
		if(this.PhotonVoicePrefab != null)
        {
			GameObject go = PhotonVoicePrefab;
			go = Photon.Pun.PhotonNetwork.Instantiate(PhotonVoicePrefab.name, Vector3.zero, Quaternion.identity, 0);
		}

        Photon.Voice.Unity.Recorder recorder = GetComponent<Photon.Voice.Unity.Recorder>();
        recorder.TransmitEnabled = true;
    }
}
