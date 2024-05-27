using UnityEditor;
using UnityEngine;
using Photon;
using Photon.Pun;


public class CarSpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    private void Start()
    {
        PhotonNetwork.Instantiate(prefab.name, new Vector3(0, 3, 0), Quaternion.identity);
    }
}
