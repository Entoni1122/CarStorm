using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    [Header("RigidBodies")]
    [SerializeField] Rigidbody sphereRB;
    [SerializeField] Rigidbody carRb;

    [Header("GroundDrag")]
    [SerializeField] float normalDrag;
    [SerializeField] float modifiedDrag;
    [SerializeField] float alignToGroundTime;
    [SerializeField] LayerMask groundLayer;

    [Header("CarStats")]
    [SerializeField] float fwdSpeed;
    [SerializeField] float revSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float maxFwdSpeed = 200f;
    [SerializeField] float stoppingAccel = 200f;
    [SerializeField] float fwdAccel = 100f;
    [SerializeField] float gravity = 200f;
    [SerializeField] float trickShotSpeed = 200f;

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    void Start()
    {
        sphereRB.transform.parent = null;
        carRb.transform.parent = null;
        normalDrag = sphereRB.drag;
    }

    void Update()
    {
        InputReader();

        float newRot = turnInput * turnSpeed * Time.deltaTime * moveInput;
        Acceleration();

        if (isCarGrounded)
        {
            transform.Rotate(0, newRot, 0, Space.World);
        }
        else
        {
            transform.Rotate(moveInput * trickShotSpeed * Time.deltaTime, 0, turnInput * trickShotSpeed * Time.deltaTime, Space.World);
        }
        transform.position = sphereRB.transform.position;

        CheckGrounded();

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f, groundLayer) && !isCarGrounded)
        {
            print("Alligning to ground");
            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
        }
    }
    void InputReader()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }
    void Acceleration()
    {
        if (moveInput > 0)
        {
            if (fwdSpeed < maxFwdSpeed)
            {
                fwdSpeed += Time.deltaTime * fwdAccel;
            }
            else
            {
                fwdSpeed = maxFwdSpeed;
            }
        }
        else
        {
            if (fwdSpeed > 0)
            {
                fwdSpeed -= Time.deltaTime * stoppingAccel;
            }
        }
    }
    void CheckGrounded()
    {
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1.5f, groundLayer);

        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
        {
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);

        }
        else
        {
            sphereRB.AddForce(Vector3.down * gravity);
        }
        carRb.MoveRotation(transform.rotation);
    }
}
