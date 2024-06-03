using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ShaderSyncToPlayer : MonoBehaviour
{
    //This scipt goes into the player
    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    [SerializeField] Material WallMaterial;
    [SerializeField] LayerMask Mask;
    
    private void Update()
    {
        var dir = Camera.main.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            WallMaterial.SetFloat(SizeID, 1);
        }
        else
        {
            WallMaterial.SetFloat(SizeID, 0);
        }
    }
}
