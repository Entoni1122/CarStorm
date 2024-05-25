using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointCarController : MonoBehaviour
{
    [SerializeField] float accelerationForce;
    [SerializeField] float breakForce;
    [SerializeField] float steerForce;
    [SerializeField] float maxSteerAngle;
    [SerializeField] GameObject[] tires = new GameObject[4];
    [SerializeField] GameObject[] tiresBck = new GameObject[2];
    [SerializeField] float tireRotationSpeed;

    [SerializeField] Transform accelerationPoint;
    [SerializeField] Rigidbody rb;

    [SerializeField] float forwardInput;
    [SerializeField] float steerInput;

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
        Acceleration();
        Steer();
        if (Input.GetKey(KeyCode.Space))
        {
            Break();
        }
        TireVisualX();
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
    #endregion

    void TireVisualX()
    {
        float steerAngle = maxSteerAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed* Time.deltaTime, Space.Self);

                tiresBck[i].transform.localEulerAngles = new Vector3(tiresBck[i].transform.localEulerAngles.x, steerAngle, tiresBck[i].transform.localEulerAngles.z);

            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * forwardInput * Time.deltaTime, Space.Self);
            }
        }
    }
}
