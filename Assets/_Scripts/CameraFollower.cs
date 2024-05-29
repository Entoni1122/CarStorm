using UnityEngine;
using Photon;
using Photon.Pun;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 OffSet;
    [SerializeField] float lerpSpeed;
    private PhotonView PhotonView;

    public void Init(Transform InTarget, ref PhotonView InPhotonView)
    {
        target = InTarget;
        PhotonView = InPhotonView;
    }

    private void FixedUpdate()
    {
        if (PhotonView.IsMine)
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + OffSet, lerpSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
