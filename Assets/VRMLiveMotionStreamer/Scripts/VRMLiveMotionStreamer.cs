using UnityEngine;
using Photon.Pun;

public class VRMLiveMotionStreamer : MonoBehaviourPunCallbacks
{
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

    void OnGUI()
    {
        if (m_target != null && GUI.Button(new Rect(30, 30, 100, 30),"Load BVH"))
        {
            LoadBVHClicked();
        }
    }

    void LoadBVHClicked()
    {
#if UNITY_STANDALONE_WIN
        var path = VRM.FileDialogForWindows.FileDialog("open BVH", ".bvh");
        if (!string.IsNullOrEmpty(path))
        {
            LoadBvh(path);
        }
#endif
    }

    void LoadBvh(string path)
    {
        Debug.LogFormat("ImportBvh: {0}", path);
        var context = new UniHumanoid.ImporterContext
        {
            Path = path
        };
        UniHumanoid.BvhImporter.Import(context);

        if (m_source != null)
        {
            GameObject.Destroy(m_source.gameObject);
        }
        m_source = context.Root.GetComponent<UniHumanoid.HumanPoseTransfer>();

        SetupTarget();
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