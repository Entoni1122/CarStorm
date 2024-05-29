using UnityEngine;
using TMPro;


public class RoomSettings : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    HostEnterPun lobbyManager;

    public TMP_Text RoomName
    {
        get
        {
            return roomName;
        }
        set
        {
            RoomName = roomName;
        }
    }
    private void Start()
    {
        lobbyManager = FindObjectOfType<HostEnterPun>();
    }

    public void OnClickRoom()
    {
        lobbyManager.JoinRoomFromContent(RoomName.text);
    }
}
