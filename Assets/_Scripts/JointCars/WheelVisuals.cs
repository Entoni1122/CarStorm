using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class WheelVisuals : MonoBehaviour
{
    [Header("Reference to GameObject")]
    [SerializeField] GameObject[] wheelsToRotate;
    [SerializeField] TrailRenderer[] trails;
    [SerializeField] ParticleSystem smoke;

    [Header("Wheels Stats")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float frontWheelRotation;
    [SerializeField] float maxWheelRotation;

    float moveInput;
    float steerInput;

    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        steerInput = Input.GetAxisRaw("Horizontal");

        float steerAngle = maxWheelRotation * steerInput;
        if (moveInput != 0 && steerInput != 0)
        {
            smoke.Stop();
        }

        for (int i = 0; i < wheelsToRotate.Length; i++)
        {
            if (i < 2)
            {
                //Front wheel rotation + left and write rotation with clamp
                //wheelsToRotate[i].transform.Rotate(0, 0, Time.deltaTime * steerInput * frontWheelRotation, Space.Self);
                wheelsToRotate[i].transform.localEulerAngles = new Vector3(wheelsToRotate[i].transform.localEulerAngles.x, steerAngle + 90, wheelsToRotate[i].transform.localEulerAngles.z);

            }
            //Back wheel rotation
            wheelsToRotate[i].transform.Rotate(0, 0, Time.deltaTime * moveInput * rotationSpeed, Space.Self);
        }

        if (steerInput != 0)
        {
            GetComponent<PhotonView>().RPC("EnableTrail", RpcTarget.All);

            smoke.Play();
        }
        else
        {
            GetComponent<PhotonView>().RPC("DisableTrail", RpcTarget.All);
        }
    }

    [PunRPC]
    void EnableTrail()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            foreach (var trail in trails)
            {
                trail.emitting = true;
            }
        }
    }

    [PunRPC]
    void DisableTrail()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            foreach (var trail in trails)
            {
                trail.emitting = false;
            }
        }
    }
}
