using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        for (int i = 0; i < wheelsToRotate.Length; i++)
        {
            if (i < 2)
            {
                //Front wheel rotation + left and write rotation with clamp
                wheelsToRotate[i].transform.Rotate(0, Time.deltaTime * steerInput * frontWheelRotation, 0, Space.Self);
                wheelsToRotate[i].transform.localEulerAngles = new Vector3(wheelsToRotate[i].transform.localEulerAngles.x, wheelsToRotate[i].transform.localEulerAngles.y, steerAngle);

            }
            //Back wheel rotation
            wheelsToRotate[i].transform.Rotate(0, 0, Time.deltaTime * moveInput * rotationSpeed, Space.Self);
        }
        if (steerInput != 0)
        {
            foreach (var trail in trails)
            {
                trail.emitting = true;
            }
            smoke.Play();
        }
        else
        {
            foreach (var trail in trails)
            {
                trail.emitting = false;
            }
            smoke.Play();
        }
        smoke.Stop();
    }
}
