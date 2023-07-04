using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public enum MoveType
    {
        Idle,
        Walking,
        Running,
        Die
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
    public float turnSpeed = 240f;
    [Space]
    public float clampMinX;
    public float clampMaxX;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Interact")]
    public float pickupRange = 1.0f;

    [Header("Health")]
    public int maxHP;
    public int currentHP;
    [Space]
    public float invisFrameTime = 1.2f;

    [Header("Leaning")]
    public float leanAngle = 20f;
    public float leanSpeed = 10f;

    [Header("Crouching")]
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 10f;

    [Header("Inventory")]
    public List<InventorySlot> invSlots;

    public Transform holdingHand;

    [Header("AreaLoading")]
    [Tooltip("The level to load when interacting with the teleporter")]public string lvlToLoad;
    [Tooltip("The name of the music file to stop the ambience or music")]public string clipToStop;

    [Header("AmuletStats")]
    public int amuletHPBoost;
    public float amuletSpeedBoost;
    public float amuletStealthBoost;

    [Header("UI")]
    public GameObject minimapUI;
    public GameObject pauseMenu;
    [Space]
    public Slider hpBar;

    //Privates
    private Rigidbody rb;
    private float speed;
    private float currentLeanAngle = 0f;
    private float targetLeanAngle = 0f;
    private float leanInput = 0f;
    private float standingHeight = 0f;
    private float targetHeight = 0f;
    private float camXRot;
    public RaycastHit hit;
    private bool isCrouching = false;
    private bool isLookingAtInteractable = false;
    private bool invisFramesActive = false;
    private bool canInvokeInvisReset = true;
    private float originalTimescale;
    private float originalSensitivity;

    [HideInInspector] public bool hasHealthAmulet;
    [HideInInspector] public bool hasSpeedAmulet;
    [HideInInspector] public bool hasStealthAmulet;
    [HideInInspector] public int invIndex;
    [HideInInspector] public GameObject equippedItem;
    [HideInInspector] public Item currentItem;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public Vector3 teleporterPos, orbPos;
    [HideInInspector] public bool isPaused = false;

    private void Awake()
    {
        originalTimescale = Time.timeScale;
    }

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (playerControlData.canWalk && isWalking && moveType != MoveType.Die)
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
        hpBar.value = currentHP;

        if(hpBar.maxValue != maxHP)
        {
            hpBar.maxValue = maxHP;
        }

        if(moveType != MoveType.Die)
        {
            CheckSpeed();
        }

        if(playerControlData.canLean && !isLookingAtInteractable)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                leanInput = 1f;

                Lean();
            }
            else if (Input.GetKey(KeyCode.E))
            {
                leanInput = -1f;

                Lean();
            }
            else
            {
                leanInput = 0f;

                Lean();
            }
        }

        if (playerControlData.lockMouseInEditor && !isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        UpdateCamera();

        Interact();

        if(playerControlData.canCrouch)
        {
            Crouch();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            Pause();
        }

        if(TeleporterBehaviour.instance.inArea)
        {
            if(Input.GetButtonDown("Interact"))
            {
                AudioManager.instance.StopClip(clipToStop);

                SceneManager.LoadScene(lvlToLoad);
            }    
        }

        //Debug
        if(Input.GetKeyDown(KeyCode.T))
        {
            transform.position = teleporterPos;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            transform.position = new Vector3(orbPos.x, orbPos.y + 1, orbPos.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerControlData.detectCollision)
        {
            if (collision.gameObject.CompareTag(playerControlData.groundTag))
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (playerControlData.detectCollision)
        {
            if (collision.gameObject.CompareTag(playerControlData.groundTag))
            {
                isGrounded = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Unexplored"))
        {
            Destroy(other.gameObject);
        }
    }

    public void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime));

        if (Input.GetButton("Horizontal"))
        {
            transform.Rotate((transform.up * Input.GetAxis("Horizontal")) * turnSpeed * Time.fixedDeltaTime);
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

            if (cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain == 0)
            {
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = .96f;
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = .32f;
            }
        }
        else if (isWalking)
        {
            moveType = MoveType.Walking;

            if (cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain == 0)
            {
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = .48f;
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = .32f;
            }
        }
        else
        {
            moveType = MoveType.Idle;

            if (cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain != 0)
            {
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
            }
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

                            cam.transform.Rotate(-mouseY * sensitivity * Time.fixedDeltaTime);
                        }
                        break;

                    case ControllerData.Type.CamType.Cinemachine:
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

                            cmCam.transform.Rotate(-mouseY * sensitivity * Time.fixedDeltaTime);
                        }
                        break;
                }
                break;
        }
    }

    public void Lean()
    {
        switch (playerControlData.type.camType)
        {
            case ControllerData.Type.CamType.UnityStandard:
                targetLeanAngle = leanInput * leanAngle;

                currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLeanAngle, Time.deltaTime * leanSpeed);

                cam.localRotation = Quaternion.Euler(0, 0, currentLeanAngle);
                break;

            case ControllerData.Type.CamType.Cinemachine:
                targetLeanAngle = leanInput * leanAngle;

                currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLeanAngle, Time.deltaTime * leanSpeed);

                cmCam.transform.localRotation = Quaternion.Euler(0, 0, currentLeanAngle);
                break;
        }
    }

    public void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                targetHeight = crouchHeight;
            }
            else
            {
                targetHeight = standingHeight;
            }
        }

        float newHeight = Mathf.Lerp(transform.localScale.y, targetHeight, Time.deltaTime * crouchSpeed);
        transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

        float moveAmount = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"));
        float cameraOffset = moveAmount * 0.05f;

        Vector3 newCameraPosition;

        switch (playerControlData.type.camType)
        {
            case ControllerData.Type.CamType.UnityStandard:
                newCameraPosition = cam.localPosition;

                newCameraPosition.z = -cameraOffset;
                cam.localPosition = newCameraPosition;
                break;

            case ControllerData.Type.CamType.Cinemachine:
                newCameraPosition = cmCam.transform.localPosition;

                newCameraPosition.z = -cameraOffset;
                cmCam.transform.localPosition = newCameraPosition;
                break;
        }
    }

    public void Interact()
    {
        Collider[] c = Physics.OverlapSphere(transform.position, pickupRange);

        switch (playerControlData.type.camType)
        {
            case ControllerData.Type.CamType.UnityStandard:

                foreach (Collider col in c)
                {
                    if(col.gameObject.CompareTag("Item"))
                    {
                        print("Press 'E' to Pickup");

                        if (Input.GetButtonDown(playerControlData.interact))
                        {
                            for (int i = 0; i < invSlots.Count; i++)
                            {
                                if (invSlots[i].currentItem == null)
                                {
                                    invSlots[i].AddItem(GetComponent<Collider>().GetComponent<ItemRef>().item);

                                    if(col.gameObject.GetComponent<ArtifactBehaviour>().amuletType != ArtifactBehaviour.AmuletType.None)
                                    {
                                        switch(col.gameObject.GetComponent<ArtifactBehaviour>().amuletType)
                                        {
                                            case ArtifactBehaviour.AmuletType.Health:
                                                hasHealthAmulet = true;
                                                break;

                                            case ArtifactBehaviour.AmuletType.Speed:
                                                hasSpeedAmulet = true;
                                                break;

                                            case ArtifactBehaviour.AmuletType.Stealth:
                                                hasStealthAmulet = true;
                                                break;
                                        }

                                        UpdateAmuletStats(col.gameObject.GetComponent<ArtifactBehaviour>());
                                    }

                                    Destroy(col.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                }
                break;

            case ControllerData.Type.CamType.Cinemachine:
                foreach (Collider col in c)
                {
                    if (col.gameObject.CompareTag("Item"))
                    {
                        print("Press 'E' to Pickup");

                        if (Input.GetButtonDown(playerControlData.interact))
                        {
                            for (int i = 0; i < invSlots.Count; i++)
                            {
                                if (invSlots[i].currentItem == null)
                                {
                                    invSlots[i].AddItem(col.GetComponent<ItemRef>().item);

                                    if (col.gameObject.GetComponent<ArtifactBehaviour>().amuletType != ArtifactBehaviour.AmuletType.None)
                                    {
                                        switch (col.gameObject.GetComponent<ArtifactBehaviour>().amuletType)
                                        {
                                            case ArtifactBehaviour.AmuletType.Health:
                                                hasHealthAmulet = true;
                                                break;

                                            case ArtifactBehaviour.AmuletType.Speed:
                                                hasSpeedAmulet = true;
                                                break;

                                            case ArtifactBehaviour.AmuletType.Stealth:
                                                hasStealthAmulet = true;
                                                break;
                                        }

                                        UpdateAmuletStats(col.gameObject.GetComponent<ArtifactBehaviour>());
                                    }

                                    Destroy(col.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if(!invisFramesActive)
        {
            invisFramesActive = true;

            CheckHealth(damage);

            if(canInvokeInvisReset)
            {
                canInvokeInvisReset = false;

                Invoke("ResetInvisFrames", invisFrameTime);
            }
        }
    }

    void CheckHealth(int damage) 
    {
        if((currentHP -= damage) <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        cmCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;

        moveType = MoveType.Die;

        minimapUI.SetActive(false);

        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;

        sensitivity = 0;
    }

    void Pause()
    {
        if(!isPaused)
        {
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;

            sensitivity = 0;

            pauseMenu.SetActive(true);
            minimapUI.SetActive(false);
        }else
        {
            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = originalTimescale;

            sensitivity = originalSensitivity;

            minimapUI.SetActive(true);
            pauseMenu.SetActive(false);
        }

        isPaused = !isPaused;
    }

    void ResetInvisFrames()
    {
        if(invisFramesActive)
        {
            invisFramesActive = false;
            canInvokeInvisReset = true;
        }
    }

    public void UpdateAmuletStats(ArtifactBehaviour behaviour)
    {
        if(hasHealthAmulet)
        {
            maxHP = maxHP + amuletHPBoost;
            currentHP += amuletHPBoost;
        }else if(!hasHealthAmulet && behaviour.isEquiped)
        {
            maxHP = maxHP - amuletHPBoost;
            
            if(currentHP > amuletHPBoost)
            {
                currentHP -= amuletHPBoost;
            }else
            {
                currentHP = 1;
            }
        }

        if (hasSpeedAmulet && behaviour.isEquiped)
        {
            walkSpeed = walkSpeed + amuletSpeedBoost;
            runSpeed = runSpeed + amuletSpeedBoost;
        }
        else if (!hasSpeedAmulet && behaviour.isEquiped)
        {
            walkSpeed = walkSpeed - amuletSpeedBoost;
            runSpeed = runSpeed - amuletSpeedBoost;
        }

        if (hasStealthAmulet)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                EnemyBehaviour eb = go.GetComponent<EnemyBehaviour>();

                if(eb.followRange > 0)
                {
                    eb.followRange -= amuletStealthBoost;
                }

                if(eb.aggroRange > 0)
                {
                    eb.aggroRange -= amuletStealthBoost;
                }
            }
        }else if (!hasStealthAmulet)
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                EnemyBehaviour eb = go.GetComponent<EnemyBehaviour>();

                eb.followRange += amuletStealthBoost;
                eb.aggroRange += amuletStealthBoost;
            }
        }
    }

    public void Initialize()
    {
        GameManager.instance.pc = this;

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
                cmCam.Priority = 10;

                cmFreeLookCam.Priority = 0;
                cmFreeLookCam.gameObject.SetActive(false);
            }
            else if (playerControlData.type.cinemachineMode == ControllerData.Type.CinemachineType.FreeLookCam)
            {
                cmFreeLookCam.gameObject.SetActive(true);
                cmFreeLookCam.Priority = 10;

                cmCam.Priority = 0;
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

            cam.localPosition = new Vector3(0, .5f, .25f);
        }

        speed = walkSpeed;
        currentHP = maxHP;

        hpBar.maxValue = maxHP;

        originalSensitivity = sensitivity;

        isWalking = false;
        isRunning = false;
        isGrounded = true;
        isPaused = false;

        moveType = MoveType.Idle;

        standingHeight = transform.localScale.y;
        targetHeight = standingHeight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
