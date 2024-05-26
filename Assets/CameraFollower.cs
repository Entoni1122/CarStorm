using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 OffSet;
    [SerializeField] float lerpSpeed;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + OffSet, lerpSpeed * Time.fixedDeltaTime);
    }
}
