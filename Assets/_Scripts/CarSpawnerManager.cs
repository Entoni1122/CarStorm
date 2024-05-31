using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CarSpawnerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject prefab;
    [SerializeField] List<Transform> spawnPoints;
    private const string PlayerIDKey = "PlayerID";

    private bool isCarSpawned = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            AssignIDAndSpawn();
        }
    }

    public override void OnJoinedRoom()
    {
        AssignIDAndSpawn();
    }

    private void AssignIDAndSpawn()
    {
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Hashtable playerProperties = new Hashtable { { PlayerIDKey, playerID } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        if (!isCarSpawned)
        {
            SpawnCar(playerID);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int newID = newPlayer.ActorNumber - 1;
            Hashtable playerProperties = new Hashtable { { PlayerIDKey, newID } };
            newPlayer.SetCustomProperties(playerProperties);
            SpawnCar(newID);
        }
    }

    private void SpawnCar(int playerID)
    {
        if (!isCarSpawned)
        {
            GameObject spawnedCar = PhotonNetwork.Instantiate(prefab.name, spawnPoints[playerID].position, Quaternion.identity);
            if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == playerID)
            {
                spawnedCar.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            isCarSpawned = true;
        }
    }
}
