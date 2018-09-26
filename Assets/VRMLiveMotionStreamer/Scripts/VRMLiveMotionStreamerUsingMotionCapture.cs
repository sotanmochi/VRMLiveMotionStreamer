using UnityEngine;
using Photon.Pun;

public class VRMLiveMotionStreamerUsingMotionCapture : MonoBehaviourPunCallbacks
{
    public GameObject Avatar;
    public int SerializationRate = 30;
    public GameObject HumanPoseSynchronizerPrefab;
    public Vector3 SynchronizerPosition;

    private UniHumanoid.HumanPoseTransfer m_target;
    private UniHumanoid.HumanPoseTransfer m_source;

    private Photon.Realtime.RaiseEventOptions raiseEventOptions = new Photon.Realtime.RaiseEventOptions
    {
        CachingOption = Photon.Realtime.EventCaching.AddToRoomCache,
        Receivers = Photon.Realtime.ReceiverGroup.Others,
    };
    private ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions
    {
        Reliability = true,
    };

    void Awake()
    {
        // Defines how many times per second OnPhotonSerialize should be called on PhotonViews.
        PhotonNetwork.SendRate = 2 * SerializationRate;
        PhotonNetwork.SerializationRate = SerializationRate;

		Debug.LogFormat("PhotonNetwork.SendRate: {0}", PhotonNetwork.SendRate);
		Debug.LogFormat("PhotonNetwork.SerializationRate: {0}", PhotonNetwork.SerializationRate);
    }

    void Start()
    {
        if (Avatar != null)
        {
            var humanPoseTransfer = Avatar.AddComponent<UniHumanoid.HumanPoseTransfer>();
            m_source = humanPoseTransfer;
        }
    }

    public override void OnJoinedRoom()
    {
        if (HumanPoseSynchronizerPrefab != null)
        {
            Debug.LogFormat("Instantiate: {0}", this.HumanPoseSynchronizerPrefab.name);
            
            GameObject humanPoseSynchronizer = PhotonNetwork.Instantiate(this.HumanPoseSynchronizerPrefab.name, SynchronizerPosition, Quaternion.identity, 0);
            humanPoseSynchronizer.GetComponent<Renderer>().enabled = false;

            SetHumanPoseTransferTarget(humanPoseSynchronizer);
            RaiseEventToSetHumanPoseTransferSource(humanPoseSynchronizer);
        }
    }

    void SetHumanPoseTransferTarget(GameObject humanPoseSynchronizer)
    {
        var humanPoseTransfer = humanPoseSynchronizer.AddComponent<UniHumanoid.HumanPoseTransfer>();
        m_target = humanPoseTransfer;
        SetupTarget();
    }

    void RaiseEventToSetHumanPoseTransferSource(GameObject humanPoseSynchronizer)
    {
        PhotonView photonView = humanPoseSynchronizer.GetComponent<PhotonView>();
        if (photonView != null)
        {
            int sendData = photonView.ViewID;
            PhotonNetwork.RaiseEvent((byte)VRMLiveMotionEventCode.SetHumanPoseTransferSource, sendData, raiseEventOptions, sendOptions);
            Debug.LogFormat("PhotonNetwork.RaiseEvent: {0}", (byte)VRMLiveMotionEventCode.SetHumanPoseTransferSource);
        }
    }

    void SetupTarget()
    {
        if (m_target != null)
        {
            m_target.Source = m_source;
            m_target.SourceType = UniHumanoid.HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;
        }
    }
}