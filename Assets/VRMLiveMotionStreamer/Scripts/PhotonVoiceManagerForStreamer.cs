using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonVoiceManagerForStreamer : Photon.Pun.MonoBehaviourPunCallbacks
{
	public GameObject PhotonVoicePrefab;

    private Photon.Realtime.RaiseEventOptions raiseEventOptions = new Photon.Realtime.RaiseEventOptions
    {
        CachingOption = Photon.Realtime.EventCaching.AddToRoomCache,
        Receivers = Photon.Realtime.ReceiverGroup.Others,
    };
    private ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions
    {
        Reliability = true,
    };

    public override void OnJoinedRoom()
    {
        // Instantiate photon voice object
		if(this.PhotonVoicePrefab != null)
        {
			GameObject go = PhotonVoicePrefab;
			go = Photon.Pun.PhotonNetwork.Instantiate(PhotonVoicePrefab.name, Vector3.zero, Quaternion.identity, 0);

            RaiseEventToSetLipSync(go);
		}

        Photon.Voice.Unity.Recorder recorder = GetComponent<Photon.Voice.Unity.Recorder>();
        recorder.TransmitEnabled = true;
    }

    void RaiseEventToSetLipSync(GameObject photonVoiceObject)
    {
        PhotonView photonView = photonVoiceObject.GetComponent<PhotonView>();
        if (photonView != null)
        {
            int sendData = photonView.ViewID;
            PhotonNetwork.RaiseEvent((byte)VRMLiveMotionEventCode.SetLipSync, sendData, raiseEventOptions, sendOptions);
            Debug.LogFormat("PhotonNetwork.RaiseEvent: {0}", (byte)VRMLiveMotionEventCode.SetLipSync);
        }
    }
}
