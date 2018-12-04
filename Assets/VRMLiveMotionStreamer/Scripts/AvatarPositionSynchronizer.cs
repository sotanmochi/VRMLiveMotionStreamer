using UnityEngine;
using Photon.Pun;

public class AvatarPositionSynchronizer : MonoBehaviour
{
	public Transform AvatarPositionTransform;

	private PhotonView m_PhotonView;

	void Start()
	{
		m_PhotonView = this.GetComponent<PhotonView>();
	}

	void Update()
	{
		if (AvatarPositionTransform != null)
		{
			if (m_PhotonView.IsMine)
        	{
				this.transform.position = AvatarPositionTransform.position;
			}
			else
			{
				AvatarPositionTransform.position = this.transform.position;
			}
		}
	}
}
