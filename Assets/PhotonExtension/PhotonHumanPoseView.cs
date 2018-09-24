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
    private HumanPose m_NextPose;

    private int m_SynchronizeMusclesCount;

    public bool m_LerpEnabled = true;

    private float m_LerpWeight;
    private HumanPose m_PreviousPose;
    private HumanPose m_CurrentPose;

    public void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();

        var animator = GetComponent<Animator>();
        if (animator != null && animator.avatar != null)
        {
            m_Avatar = animator.avatar;
            m_PoseHandler = new HumanPoseHandler(m_Avatar, transform);
            m_PoseHandler.GetHumanPose(ref m_NextPose);
            m_SynchronizeMusclesCount = m_NextPose.muscles.Length;

            m_PoseHandler.GetHumanPose(ref m_PreviousPose);
            m_PoseHandler.GetHumanPose(ref m_CurrentPose);
        }
    }

    public void Update()
    {
        if (!this.m_PhotonView.IsMine)
        {
            if (m_PoseHandler != null)
            {
                if (m_LerpEnabled)
                {
                    float fps = Mathf.Round(1.0f / Time.unscaledDeltaTime);
                    float sr = PhotonNetwork.SerializationRate;
                    m_LerpWeight += (sr / fps);

                    m_CurrentPose.bodyPosition = Vector3.Lerp(m_PreviousPose.bodyPosition, m_NextPose.bodyPosition, m_LerpWeight);
                    m_CurrentPose.bodyRotation = Quaternion.Lerp(m_PreviousPose.bodyRotation, m_NextPose.bodyRotation, m_LerpWeight);
                    for (int i = 0; i < m_SynchronizeMusclesCount; i++)
                    {
                        m_CurrentPose.muscles[i] = Mathf.Lerp(m_PreviousPose.muscles[i], m_NextPose.muscles[i], m_LerpWeight);
                    }

                    m_PoseHandler.SetHumanPose(ref m_CurrentPose);
                }
                else
                {
                    m_PoseHandler.SetHumanPose(ref m_NextPose);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            m_PoseHandler.GetHumanPose(ref m_NextPose);

            stream.SendNext(m_NextPose.bodyPosition);
            stream.SendNext(m_NextPose.bodyRotation);
            for (int i = 0; i < m_SynchronizeMusclesCount; i++)
            {
                stream.SendNext(m_NextPose.muscles[i]);
            }
        }
        else
        {
            m_PreviousPose.bodyPosition = m_NextPose.bodyPosition;
            m_PreviousPose.bodyRotation = m_NextPose.bodyRotation;
            for (int i = 0; i < m_SynchronizeMusclesCount; i++)
            {
                m_PreviousPose.muscles[i] = m_NextPose.muscles[i];
            }

            m_NextPose.bodyPosition = (Vector3)stream.ReceiveNext();
            m_NextPose.bodyRotation = (Quaternion)stream.ReceiveNext();
            for (int i = 0; i < m_SynchronizeMusclesCount; i++)
            {
                m_NextPose.muscles[i] = (float)stream.ReceiveNext();
            }

            m_LerpWeight = 0.0f;
        }
    }
}