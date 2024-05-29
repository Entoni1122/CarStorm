using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    public GameObject[] wheelsToRotate;
    public TrailRenderer[] trails;
    public ParticleSystem smoke;

    public float rotationSpeed;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        foreach (var wheel in wheelsToRotate)
        {
            wheel.transform.Rotate(Time.deltaTime * verticalAxis * rotationSpeed, 0, 0, Space.Self);
        }

        if (horizontalAxis > 0)
        {
            anim.SetBool("goingLeft", false);
            anim.SetBool("goingRight", true);
        }
        else if (horizontalAxis < 0)
        {
            anim.SetBool("goingRight", false);
            anim.SetBool("goingLeft", true);
        }
        else
        {
            anim.SetBool("goingRight", false);
            anim.SetBool("goingLeft", false);
        }

        if (horizontalAxis != 0)
        {
            foreach (var trail in trails)
            {
                trail.emitting = true;
            }

            var emission = smoke.emission;
            emission.rateOverTime = 50f;
        }
        else
        {
            foreach (var trail in trails)
            {
                trail.emitting = false;
            }

            var emission = smoke.emission;
            emission.rateOverTime = 0f;
        }
    }
}
