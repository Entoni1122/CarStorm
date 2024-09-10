using Cinemachine;
using Photon.Pun;
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

    PhotonView photonView;

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    [Header("Cameras")]
    [SerializeField] GameObject cameraToSpawn;
    [SerializeField] GameObject cameraOffSet;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] public Camera _camera;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        SetUpCameras();
    }
    void Start()
    {
        sphereRB.transform.parent = null;
        carRb.transform.parent = null;
        normalDrag = sphereRB.drag;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MovemntUpdate();
        }
    }

    public void SetUpCameras()
    {
        GameObject cinemachineBrain = Instantiate(cameraToSpawn);
        _camera = cinemachineBrain.GetComponent<Camera>();
        GameObject cameraOff = Instantiate(cameraOffSet);
        cameraOff.GetComponent<CameraFollower>().Init(this.gameObject.transform, ref photonView);

        if (!photonView.IsMine)
        {
            _camera.gameObject.SetActive(false);
            cinemachineVirtualCamera.gameObject.SetActive(false);
        }
        else
        {
            cinemachineVirtualCamera.Follow = cameraOff.transform;
            cinemachineVirtualCamera.LookAt = transform;
        }
    }
    void MovemntUpdate()
    {
        InputReader();
        AccelerationCalculation();
        CheckGrounded();
        float velocityConstaint = Mathf.Clamp(1f - (sphereRB.velocity.magnitude * 0.01f), 1f, 2f);
        float newRot = turnInput * turnSpeed * Time.deltaTime * velocityConstaint;
        print(sphereRB.velocity.magnitude * 0.01f);
        if (isCarGrounded)
        {
            transform.Rotate(0, newRot, 0, Space.World);
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

    void InputReader()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        turnInput = Input.GetAxisRaw("Horizontal");
    }
    void AccelerationCalculation()
    {
        //Acceleration if movemnt button is pressed
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
            //Move with input if the car is touching the ground
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);

        }
        else
        {
            //Pull the car towards the ground if not grounded
            sphereRB.AddForce(Vector3.down * gravity);
        }
        carRb.MoveRotation(transform.rotation);
    }
}
