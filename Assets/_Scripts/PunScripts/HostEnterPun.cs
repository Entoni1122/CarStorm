using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class HostEnterPun : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField hostTXT;
    [SerializeField] TMP_InputField joinTXT;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(hostTXT.text);
    }

    public void JoinRoom()
    {
        if (joinTXT.text != hostTXT.text)
        {
            print("Non esiste");
        }
        PhotonNetwork.JoinRoom(joinTXT.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("EntoniPlayGround");
    }

}
