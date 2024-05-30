using UnityEngine;
using Photon.Pun;
using Cinemachine;


[RequireComponent(typeof(Rigidbody))]
public class RaycastCarController : MonoBehaviour
{
    [Header("Joystick")]
    [SerializeField] Joystick TouchJoystick;
    [SerializeField] float YawOffset = 45;

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
    [SerializeField] Vector3 centerOfMass; //Poco sotto il pivot per fare che non flippi

    [Header("Inputs")]
    [SerializeField] float moveInput = 0;
    [SerializeField] float steerInput = 0;

    [Header("CarStats")]
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] AnimationCurve turnCurve;
    [SerializeField] float steerStregth;
    [SerializeField] float dragCoefficient = 1; //The more the less drift

    [Header("Visual")]
    [SerializeField] float tireRotationSpeed;//Visual rapresentation of the tires turning
    [SerializeField] float maxSteerAngle = 30f;
    [SerializeField] float breakForce;

    Vector3 currentVelocity = Vector3.zero;
    [SerializeField] float velocityMagitude;
    [SerializeField] float gravity;
    [SerializeField] bool shouldTireRotateY;
    float carVelRation = 0;

    int[] wheelIsGrounded = new int[4];
    bool isGrounded = false;
    PhotonView punView;
    [SerializeField] float tirePositionLerpSpeed;

    [Header("Cameras")]
    [SerializeField] GameObject cameraToSpawn;
    [SerializeField] GameObject cameraOffSet;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] GameObject canvas;

    [Header("GameProperty")]
    private int ID;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        punView = GetComponent<PhotonView>();

        SetUpCameras();

        ID = GameManager.Self.JoinLobby(this);
    }

    public void SetUpCameras()
    {
        GameObject cinemachineBrain = Instantiate(cameraToSpawn);
        GameObject cameraOff = Instantiate(cameraOffSet);
        cameraOff.GetComponent<CameraFollower>().Init(this.gameObject.transform, ref punView);

        if (!punView.IsMine)
        {
            cinemachineBrain.SetActive(false);
            canvas.SetActive(false);
            cinemachineVirtualCamera.gameObject.SetActive(false);
        }
        else
        {
            cinemachineVirtualCamera.Follow = cameraOff.transform;
            cinemachineVirtualCamera.LookAt = transform;
        }
    }


    void FixedUpdate()
    {
        if (punView.IsMine)
        {
            MovementUpdate();
        }
    }

    public void MovementUpdate()
    {
        GroundCheck();
        Suspension();
        CalculateCarVelocity();
        Movement();
        if (!shouldTireRotateY)
        {
            TireVisualX();
        }
        else
        {
            TireVisualY();
        }
    }
    private void Update()
    {
        GetPlayerInput();
        dragCoefficient = 3 - (currentVelocity.magnitude / maxSpeed) * 3;
        velocityMagitude = currentVelocity.magnitude;
    }

    void Suspension()
    {
        for (int i = 0; i < raycastPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLenght = baseSpringLenght + maxSpringDistance;
            if (Physics.Raycast(raycastPoints[i].transform.position, Vector3.down, out hit, maxLenght + wheelRadius, ground))
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
        //moveInput = Input.GetAxis("Vertical");
        //steerInput = Input.GetAxis("Horizontal");
        moveInput = TouchJoystick.Direction.y;
        steerInput = TouchJoystick.Direction.x;
    }
    #endregion

    #region Visuals
    void SetTirePosition(GameObject tires, Vector3 tirePos)
    {
        tires.transform.position = Vector3.Lerp(tires.transform.position, tirePos, tirePositionLerpSpeed * Time.deltaTime);
    }

    void TireVisualY()
    {
        float steerAngle = maxSteerAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.forward, tireRotationSpeed * carVelRation * Time.deltaTime, Space.Self);

                tiresBack[i].transform.localEulerAngles = new Vector3(tiresBack[i].transform.localEulerAngles.x, steerAngle, tiresBack[i].transform.localEulerAngles.z);
            }
            else
            {
                tires[i].transform.Rotate(Vector3.forward, tireRotationSpeed * moveInput * Time.deltaTime, Space.Self);
            }
        }
    }


    void TireVisualX()
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
        rb.AddForceAtPosition(acceleration * TouchJoystick.MoveAmount * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    void Break()
    {
        if (currentVelocity.magnitude != 0)
        {
            breakForce += Time.deltaTime;
            rb.AddForceAtPosition(breakForce * -rb.velocity, accelerationPoint.position, ForceMode.Acceleration);
        }
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
            if (Input.GetKey(KeyCode.Space))
            {
                Break();
            }
            else
            {
                breakForce = 2;
            }
            Turn();
            SideWay();
        }
        else
        {
            ApplyGravity();
        }
    }

    void ApplyGravity()
    {
        rb.AddForceAtPosition(gravity * Vector3.down, transform.position, ForceMode.Acceleration);
    }

    void Turn()
    {
        //rb.AddTorque(steerStregth * steerInput * Mathf.Sign(carVelRation) * transform.up, ForceMode.Acceleration);

        if (TouchJoystick.MoveAmount != 0)
        {
            Vector3 ForwardFix = new Vector3(TouchJoystick.Direction.x, 0, TouchJoystick.Direction.y);
            Quaternion TargetRotation = Quaternion.LookRotation(ForwardFix, Vector3.up);
            TargetRotation.eulerAngles -= new Vector3(0, YawOffset, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, steerStregth * Time.fixedDeltaTime);
        }
    }
    #endregion   //Movemnt Input and acceleraion-deceleration
}