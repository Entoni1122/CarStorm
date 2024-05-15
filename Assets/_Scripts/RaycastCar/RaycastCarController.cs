using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RaycastCarController : MonoBehaviour
{
    [Header("Base Vars")]
    [SerializeField] Transform[] raycastPoints;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask ground;
    [SerializeField] Transform accelerationPoint;
    [SerializeField] GameObject[] tires = new GameObject[4];
    [SerializeField] GameObject[] tiresBack = new GameObject[2];

    [Header("Car Handling")]
    [SerializeField] float maxSpringDistance; //Max distance the spring can go either fully streached or fully compressed
    [SerializeField] float springStiffness; //The force the spring make after being streached
    [SerializeField] float baseSpringLenght; //Spring lenght when not stressed in any way
    [SerializeField] float wheelRadius;
    [SerializeField] float damperdStiffness; //Makes the car more boucy the lower the value is
    [SerializeField] Vector3 CenterOfMass; //Poco sotto il pivot per fare che non flippi

    [Header("Inputs")]
    [SerializeField] float moveInput = 0;
    [SerializeField] float steerInput = 0;

    [Header("CarStats")]
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float deceleraion;
    [SerializeField] AnimationCurve turnCurve;
    [SerializeField] float steerStregth;
    [SerializeField] float dragCoefficient = 1; //The more the less drift

    [Header("Visual")]
    [SerializeField] float tireRotationSpeed;//Visual rapresentation of the tires turning
    [SerializeField] float maxSteerAngle = 30f;

    Vector3 currentVelocity = Vector3.zero;
    float carVelRation = 0;

    int[] wheelIsGrounded = new int[4];
    bool isGrounded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        TireVisual();
    }
    private void Update()
    {
        GetPlayerInput();
    }


    void Suspension()
    {
        for (int i = 0; i < raycastPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLenght = baseSpringLenght + maxSpringDistance;
            if (Physics.Raycast(raycastPoints[i].transform.position, -raycastPoints[i].up, out hit, maxLenght + wheelRadius, ground))
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLenght = hit.distance - wheelRadius;
                float springCompression = (baseSpringLenght - currentSpringLenght) / maxSpringDistance;

                float springVel = Vector3.Dot(rb.GetPointVelocity(raycastPoints[i].position), raycastPoints[i].up);
                float dampForce = damperdStiffness * springVel;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                rb.AddForceAtPosition(netForce * raycastPoints[i].up, raycastPoints[i].position);

                SetTirePosition(tires[i], hit.point + raycastPoints[i].up * wheelRadius);

                Debug.DrawLine(raycastPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;

                SetTirePosition(tires[i], raycastPoints[i].position - raycastPoints[i].up * maxLenght);


                Debug.DrawLine(raycastPoints[i].position, raycastPoints[i].position + (wheelRadius + maxLenght) * -raycastPoints[i].up, Color.green);
            }

        }
    }


    #region CheckCalculations
    void GroundCheck()
    {
        int tempGround = 0;

        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGround += wheelIsGrounded[i];
        }


        if (tempGround > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void CalculateCarVelocity()
    {
        currentVelocity = transform.InverseTransformDirection(rb.velocity);
        carVelRation = currentVelocity.z / maxSpeed;
    }
    void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }
    #endregion

    #region Visuals

    void SetTirePosition(GameObject tires, Vector3 tirePos)
    {
        tires.transform.position = tirePos;
    }

    void TireVisual()
    {
        float steerAngle = maxSteerAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * carVelRation * Time.deltaTime, Space.Self);

                tiresBack[i].transform.localEulerAngles = new Vector3(tiresBack[i].transform.localEulerAngles.x, steerAngle, tiresBack[i].transform.localEulerAngles.z);

            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotationSpeed * moveInput * Time.deltaTime, Space.Self);
            }
        }
    }
    #endregion

    #region MVM
    void Acceleration()
    {
        rb.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    void Deceleration()
    {
        rb.AddForceAtPosition(deceleraion * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    void SideWay()
    {
        float currentSideSpeed = currentVelocity.x;
        float dragMagnitude = -currentSideSpeed * dragCoefficient;

        Vector3 dragForce = transform.right * dragMagnitude;

        rb.AddForceAtPosition(dragForce, rb.worldCenterOfMass, ForceMode.Acceleration);

    }
    void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SideWay();
        }
    }

    void Turn()
    {
        rb.AddTorque(steerStregth * steerInput * turnCurve.Evaluate(carVelRation) * Mathf.Sign(carVelRation) * transform.up, ForceMode.Acceleration);
    }
    #endregion   //Movemnt Input and acceleraion-deceleration
}