using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCarController : MonoBehaviour
{
    [Header("Car General VALUE")]
    [SerializeField] float accelerationForce;
    [SerializeField] float breakForce;
    [SerializeField] float steerForce;
    [SerializeField] float maxSteerAngle;
    [SerializeField] float dragCoefficient;
    [SerializeField] Transform accelerationPoint;

    [Header("Car Visuals")]
    [SerializeField] GameObject[] tires = new GameObject[4];
    [SerializeField] GameObject[] tiresBck = new GameObject[2];
    [SerializeField] float tireRotationSpeed;

    Rigidbody rb;
    float forwardInput;
    float steerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadInput();

    }
    private void FixedUpdate()
    {
        if (GroundCheck())
        {
            Acceleration();
            Steer();
            if (Input.GetKey(KeyCode.Space))
            {
                Break();
            }
        }
    }

    bool GroundCheck()
    {
        Debug.DrawLine(transform.position, Vector3.down * 3f, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, 3f))
        {
            return true;
        }
        return false;
    }


    #region ReadValue
    void ReadInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }
    #endregion

    #region Movememnt
    void Acceleration()
    {
        rb.AddForceAtPosition(transform.forward * accelerationForce * forwardInput, accelerationPoint.position, ForceMode.Acceleration);
    }
    void Steer()
    {
        rb.AddTorque(transform.up * steerForce * steerInput, ForceMode.Acceleration);
    }
    void Break()
    {
        rb.AddForceAtPosition(-transform.up * accelerationForce, accelerationPoint.position, ForceMode.Acceleration);
    }
    void SideWay()
    {
        float currentSideSpeed = rb.velocity.x;

        float dragMagnitude = -currentSideSpeed * dragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        rb.AddForceAtPosition(dragForce, rb.worldCenterOfMass, ForceMode.Acceleration);
    }
    #endregion

    void TireVisualX()
    {
        float steerAngle = maxSteerAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * Time.deltaTime, Space.Self);

                tiresBck[i].transform.localEulerAngles = new Vector3(tiresBck[i].transform.localEulerAngles.x, steerAngle, tiresBck[i].transform.localEulerAngles.z);

            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * forwardInput * Time.deltaTime, Space.Self);
            }
        }
    }
}
