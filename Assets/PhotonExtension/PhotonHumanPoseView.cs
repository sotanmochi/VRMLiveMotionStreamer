// -------------------------------------
// PhotonHumanPoseView.cs
// Copyright (c) 2018 sotan.
// Licensed under the MIT License.
// -------------------------------------

using UnityEngine;
using Photon.Pun;

[AddComponentMenu("Photon Networking/Photon HumanPose View")]
[RequireComponent(typeof(PhotonView))]
public class PhotonHumanPoseView : MonoBehaviour, IPunObservable
{
    private PhotonView m_PhotonView;

    private Avatar m_Avatar;
    private HumanPoseHandler m_PoseHandler;
    private HumanPose m_Pose;

    private int m_SynchronizeMusclesCount;

    public void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        var animator = GetComponent<Animator>();
        if (animator != null && animator.avatar != null)
        {
            m_Avatar = animator.avatar;
            m_PoseHandler = new HumanPoseHandler(m_Avatar, transform);
            m_PoseHandler.GetHumanPose(ref m_Pose);
            m_SynchronizeMusclesCount = m_Pose.muscles.Length;
        }
    }

    public void Update()
    {
        if (!this.m_PhotonView.IsMine)
        {
            if (m_PoseHandler != null)
            {
                m_PoseHandler.SetHumanPose(ref m_Pose);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            m_PoseHandler.GetHumanPose(ref m_Pose);

            stream.SendNext(m_Pose.bodyPosition);
            stream.SendNext(m_Pose.bodyRotation);
            for (int i = 0; i < m_SynchronizeMusclesCount; i++)
            {
                stream.SendNext(m_Pose.muscles[i]);
            }           
        }
        else
        {
            m_Pose.bodyPosition = (Vector3)stream.ReceiveNext();
            m_Pose.bodyRotation = (Quaternion)stream.ReceiveNext();
            for (int i = 0; i < m_SynchronizeMusclesCount; i++)
            {
                m_Pose.muscles[i] = (float)stream.ReceiveNext();
            }
        }
    }
}