using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Rigidbody sphereRB;
    [SerializeField] Rigidbody carRb;
    [SerializeField] Transform models;

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
    [SerializeField] float modelTorque;

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
        AccelerationCalculation();
        CheckGrounded();

        float newRot = turnInput * turnSpeed * Time.deltaTime * (sphereRB.velocity.magnitude * 0.01f);

        if (isCarGrounded)
        {
            transform.Rotate(0, newRot, 0, Space.World);
            //BodyTorque();
        }
        else
        {
            transform.Rotate(moveInput * trickShotSpeed * Time.deltaTime, 0, turnInput * trickShotSpeed * Time.deltaTime, Space.World);
        }
        transform.position = sphereRB.transform.position;

        moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;

        //Change drag if in air
        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f, groundLayer) && !isCarGrounded)
        {
            print("Alligning to ground");
            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
        }
    }

    void BodyTorque()
    {
        float yes = moveInput * modelTorque * Time.deltaTime;
        models.Rotate(yes, 0, 0, Space.Self);

        models.eulerAngles = new Vector3(Mathf.Clamp(yes, -3, 3), models.eulerAngles.y, models.eulerAngles.z);
    }

    void InputReader()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }
    void AccelerationCalculation()
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