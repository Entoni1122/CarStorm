using UnityEditor;
using UnityEngine;
using Photon;
using Photon.Pun;


public class CarSpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform spawnPoint;
    private void Start()
    {
        PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);
    }
}
