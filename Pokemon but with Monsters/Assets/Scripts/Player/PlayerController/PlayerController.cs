using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public enum MoveType
    {
        Idle,
        Walking,
        Running
    }

    public ControllerData playerControlData;

    [Header("Cams")]
    public Transform cam;
    public CinemachineVirtualCamera cmCam;
    public CinemachineFreeLook cmFreeLookCam;

    [Header("Movement")]
    public MoveType moveType;
    [Space]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float sensitivity = 180f;
    [Space]
    public float clampMinX;
    public float clampMaxX;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Interact")]
    public float interactRayDst = 1.0f;

    [Header("Cinemachine Camera's")]

    //Privates
    private Rigidbody rb;
    private float speed;
    public RaycastHit hit;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isRunning;

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (playerControlData.canWalk && isWalking)
        {
            Move();
        }

        if (playerControlData.canWalk && moveType == MoveType.Idle)
        {
            if (Input.GetButton("Horizontal"))
            {
                if (!isRunning)
                {
                    rb.MovePosition(transform.position + (transform.right * Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime));
                }
                else if (isRunning)
                {
                    rb.MovePosition(transform.position + (transform.right * Input.GetAxis("Horizontal") * runSpeed * Time.deltaTime));
                }
            }
        }

        if (playerControlData.canJump && isGrounded)
        {
            if (Input.GetButtonDown(playerControlData.jump))
            {
                Jump();
            }
        }
    }

    private void Update()
    {
        CheckSpeed();

        UpdateCamera();

        if (playerControlData.lockMouseInEditor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetButtonDown(playerControlData.interact))
        {
            Interact();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerControlData.detectCollision)
        {
            if (collision.gameObject.tag == playerControlData.groundTag)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (playerControlData.detectCollision)
        {
            if (collision.gameObject.tag == playerControlData.groundTag)
            {
                isGrounded = false;
            }
        }
    }

    public void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime));

        if (Input.GetButton("Horizontal"))
        {
            transform.Rotate((transform.up * Input.GetAxis("Horizontal")) * sensitivity * Time.fixedDeltaTime);
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void CheckSpeed()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            isWalking = true;
        }
        else if (Input.GetButtonUp("Vertical"))
        {
            isWalking = false;
            isRunning = false;
        }

        if (Input.GetButton(playerControlData.sprint))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (isWalking && isRunning)
        {
            moveType = MoveType.Running;
        }
        else if (isWalking)
        {
            moveType = MoveType.Walking;
        }
        else
        {
            moveType = MoveType.Idle;
        }

        ChangeSpeed();
    }

    public void ChangeSpeed()
    {
        switch (moveType)
        {
            case MoveType.Idle:
                speed = 0;
                break;

            case MoveType.Walking:
                speed = walkSpeed;
                break;

            case MoveType.Running:
                speed = runSpeed;
                break;
        }
    }

    public void UpdateCamera()
    {
        switch (playerControlData.type.controlMode)
        {
            case ControllerData.Type.ControlMode.ThirdPerson:
                print("Implement this you lazy fuck");
                break;

            case ControllerData.Type.ControlMode.FirstPerson:
                switch (playerControlData.type.camType)
                {
                    case ControllerData.Type.CamType.UnityStandard:
                        if (playerControlData.canLookLeftAndRight)
                        {
                            Vector3 mouseX = new Vector3(0, Input.GetAxis("Mouse X"), 0);

                            transform.Rotate((mouseX * sensitivity * Time.fixedDeltaTime));
                        }

                        if (playerControlData.canLookUpAndDown)
                        {
                            Vector3 mouseY = new Vector3(Input.GetAxis("Mouse Y"), 0, 0);

                            if (playerControlData.camClamped)
                            {
                                print("Fucking stop dying");
                            }

                            cam.Rotate(-mouseY * sensitivity * Time.fixedDeltaTime);
                        }
                        break;
                }
                break;
        }
    }

    public void Interact()
    {
        if (Input.GetButtonDown(playerControlData.interact))
        {

        }
    }

    public void Initialize()
    {
        if (playerControlData == null)
        {
            Debug.LogError("No Control Data, Player Controller wont be working. Please create or assign a Control Data Asset");
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        else if (GetComponent<Rigidbody>() == null)
        {
            Debug.LogError("Error, No Rigidbody on the GameObject. Please Manually add one by pressing the AddComponentButton");
        }

        if (playerControlData.type.camType == ControllerData.Type.CamType.UnityStandard)
        {
            cmCam.gameObject.SetActive(false);
            cmFreeLookCam.gameObject.SetActive(false);

            cam.GetComponent<CinemachineBrain>().enabled = false;
        }
        else if (playerControlData.type.camType == ControllerData.Type.CamType.Cinemachine)
        {
            if (playerControlData.type.cinemachineMode == ControllerData.Type.CinemachineType.VirtualCam)
            {
                cmCam.gameObject.SetActive(true);
                cmFreeLookCam.gameObject.SetActive(false);
            }
            else if (playerControlData.type.cinemachineMode == ControllerData.Type.CinemachineType.FreeLookCam)
            {
                cmFreeLookCam.gameObject.SetActive(true);
                cmCam.gameObject.SetActive(false);
            }
            else
            {
                playerControlData.type.camType = ControllerData.Type.CamType.UnityStandard;

                cmCam.gameObject.SetActive(false);
                cmFreeLookCam.gameObject.SetActive(false);
            }
        }

        if (playerControlData.type.controlMode == ControllerData.Type.ControlMode.FirstPerson)
        {
            cam.SetParent(this.transform);

            cam.localPosition = new Vector3(0, .5f, .2f);
        }

        speed = walkSpeed;

        isWalking = false;
        isRunning = false;
        isGrounded = true;

        moveType = MoveType.Idle;
    }
}
