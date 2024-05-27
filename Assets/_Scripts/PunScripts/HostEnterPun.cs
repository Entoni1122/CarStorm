using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class HostEnterPun : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField hostTXT;
    [SerializeField] TMP_InputField joinTXT;


    [SerializeField] RoomSettings roomPrefab;
    List<RoomSettings> roomItemList = new List<RoomSettings>();
    public Transform contentOBJ;

    private void Start()
    {
        PhotonNetwork.JoinLobby(); //Need to be inside a lobby to create a room
    }

    public void CreateRoom()
    {
        if (hostTXT.text.Length >= 3)
        {
            PhotonNetwork.CreateRoom(hostTXT.text, new RoomOptions() { MaxPlayers = 4 }); //automatically joins the room u created

        }
        else
        {
            print("Room name : " + hostTXT.text + " is too short.");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }
    void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomSettings roomItem in roomItemList)
        {
            Destroy(roomItem.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo roomItem in roomList)
        {
            RoomSettings room = Instantiate(roomPrefab, contentOBJ);
            room.RoomName.text = roomItem.Name;
            roomItemList.Add(room);
        }
    }

    public void JoinRoomFromTEXT()
    {
        if (joinTXT.text != hostTXT.text)
        {
            print("Non esiste");
        }
        PhotonNetwork.JoinRoom(joinTXT.text);
    }

    public void JoinRoomFromContent(string InRoomName)
    {
        PhotonNetwork.JoinRoom(InRoomName);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("EntoniPlayGround");
    }
}
