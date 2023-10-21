using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using GameManagment;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public enum Mods { None, Default, Sprint, Crouch }

    #region -camera-

    [Header("Camera controll")]
    //[SerializeField] private Camera playerCamera; //using cinemashineCameraTarget instead
    [SerializeField] public float mouseSensitivity = 8f;
    [SerializeField, Range(0, -90)] private float minPitch = -90f;
    [SerializeField, Range(0, 90)] private float maxPitch = 90f;

    private float yaw;
    private float pitch;

    [field: SerializeField, ReadOnly] public bool FreezCamera { get; set; } = false;

    #endregion

    #region -movment-

    [Header("Default")]
    [SerializeField, Min(1f)]
    private float defaultSpeed = 5f;

    [SerializeField, Min(0.5f)]
    private float defaultHeight = 1.9f;

    [SerializeField, Min(0f)]
    private float smoothMoveTime = 0.1f;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private float gravity = -18f;

    [SerializeField]
    private bool canJump = true;

    [SerializeField, ShowIf(nameof(canJump)), Min(1f)]
    private float defaultJumpHeight = 1.2f;

    [Header("Sprint")]
    [SerializeField]
    private bool canSprint = true;

    [SerializeField, ShowIf(nameof(canSprint)), Min(1f)]
    private float sprintSpeed = 9f;

    [SerializeField, ShowIf(EConditionOperator.And, nameof(canSprint), nameof(canJump)), Min(1f)]
    private float sprintJumpHeight = 1.4f;

    [Header("Crouch")]
    [SerializeField]
    private bool canCrouch = true;

    [SerializeField, ShowIf(nameof(canCrouch)), Min(1f)]
    private float crouchSpeed = 3f;

    [SerializeField, ShowIf(EConditionOperator.And, nameof(canCrouch), nameof(canJump)), Min(1f)]
    private float crouchJumpHeight = 2f;

    [SerializeField, ShowIf(nameof(canCrouch)), Min(0.5f)]
    private float crouchHeight = 1.2f;

    [Header("Cinemashine")]
    [SerializeField] private bool canChangePers = true;
    [SerializeField] private KeyCode persChangeKey = KeyCode.Q;
    [SerializeField] private KeyCode zoomKey = KeyCode.V;
    [SerializeField] private GameObject cinemashineCameraTarget;
    [SerializeField] private float topCameraClamp = 89.5f;
    [SerializeField] private float bottomCameraClamp = -89.5f;
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera fpsAimCamera;
    [SerializeField] private CinemachineVirtualCamera tpsCamera;
    [SerializeField] private CinemachineVirtualCamera tpsAimCamera;
    private bool isCameraInFpsState = true;

    [Header("Headbob Parameters")]
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.5f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 1.0f;
    [SerializeField] private float amplitude = 0.05f;
    [SerializeField] private float frequency = 15f;
    [SerializeField] private float defaultYPos = 1.75f;
    private float timer;

    [Header("Animation Parameters")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator shadowPlayerAnimator;
    [SerializeField] private Animator legAnimator;
    [SerializeField] private GameObject legZombiak;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultipier = 0.6f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;

    [Header("Inputs")]
    [SerializeField, ReadOnly] private bool inputJump = false;
    [SerializeField, ReadOnly] private bool inputSprint = false;
    [SerializeField, ReadOnly] private bool inputCrouch = false;
    [SerializeField, ReadOnly] private Vector2 inputMove = Vector2.zero;
    [SerializeField, ReadOnly] private Vector2 inputMouse = Vector2.zero;

    [Header("States")]
    [SerializeField, ReadOnly] private bool isGrounded = false;
    [SerializeField, ReadOnly] private Mods currentMod = Mods.None;
    [SerializeField, ReadOnly] private float currentSpeed;
    [SerializeField, ReadOnly] private float currentJumpHeight;

    private Vector3 smoothV;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float verticalVelocity;

    [field: SerializeField, ReadOnly] public bool FreezMovement { get; set; } = false;

    #endregion

    private CharacterController controller;
    private Tweener hightModifier;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GameManager.OnPauseGame += SetEnable;
        GameManager.OnResumeGame += SetDisable;

        SetMode(Mods.Default);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = cinemashineCameraTarget.transform.localEulerAngles.x;

        defaultYPos = cinemashineCameraTarget.transform.localPosition.y;
    }
    private void OnDestroy()
    {
        GameManager.OnPauseGame -= SetEnable;
        GameManager.OnResumeGame -= SetDisable;
    }

    private void Update()
    {
        float radius = controller.radius * 0.9f;
        Vector3 groundCheck = controller.bounds.center - (controller.bounds.extents.y - radius + 0.2f) * Vector3.up;
        isGrounded = Physics.CheckSphere(groundCheck, radius, groundMask);

        GetInput();

        MoveCamera();

        ChoseMode();
        MovePlayer();

        Shader.SetGlobalVector("_PlayerPos", transform.position);
        if (canChangePers)
        {
            HandlePersChange();
        }
        if (canUseHeadBob)
        {
            HandleHeadbob();
        }
        HandleZoom();
    }

    private void SetMode(Mods mod)
    {
        if (currentMod == mod)
            return;

        switch (mod)
        {
            case Mods.Default:
                currentSpeed = defaultSpeed;
                currentJumpHeight = defaultJumpHeight;
                SetHeight(defaultHeight, 0.3f);
                break;
            case Mods.Sprint:
                currentSpeed = sprintSpeed;
                currentJumpHeight = sprintJumpHeight;
                SetHeight(defaultHeight, 0.3f);
                break;
            case Mods.Crouch:
                currentSpeed = crouchSpeed;
                currentJumpHeight = crouchJumpHeight;
                SetHeight(crouchHeight, 0.5f);
                break;
            default:
                currentSpeed = 0;
                currentJumpHeight = 0;
                SetHeight(defaultHeight);
                break;
        }

        currentMod = mod;
    }

    private void ChoseMode()
    {
        if (canSprint && inputSprint)
            SetMode(Mods.Sprint);
        else if (canCrouch && inputCrouch)
            SetMode(Mods.Crouch);
        else
            SetMode(Mods.Default);
    }

    private void SetHeight(float targetHeight, float speed)
    {
        hightModifier?.Kill();
        float time = Math.Abs(controller.height - targetHeight) * speed;
        hightModifier = DOVirtual.Float(controller.height, targetHeight, time, v => SetHeight(v, false));
    }
    private void SetHeight(float targetHeight, bool kill = true)
    {
        if (kill)
            hightModifier?.Kill();

        if (isGrounded)
            controller.Move(transform.up * (targetHeight - controller.height));

        controller.height = targetHeight;
    }


    private void GetInput()
    {
        // movement
        inputMove.x = Input.GetAxisRaw("Horizontal");
        inputMove.y = Input.GetAxisRaw("Vertical");

        // mouse
        inputMouse.x = Input.GetAxisRaw("Mouse X");
        inputMouse.y = Input.GetAxisRaw("Mouse Y");

        //sprint on/off
        inputSprint = Input.GetKey(KeyCode.LeftShift);

        //Crouching controller
        inputCrouch = Input.GetKey(KeyCode.LeftControl);

        //jumping
        inputJump = Input.GetKey(KeyCode.Space);
    }

    private void MoveCamera()
    {
        // Verrrrrry gross hack to stop camera swinging down at start
        if (Time.unscaledTime < 6f && inputMouse.x * inputMouse.x + inputMouse.y * inputMouse.y > 50)
        {
            inputMouse.x = 0;
            inputMouse.y = 0;
        }

        if (FreezCamera)
            return;

        float settingsMultiplier = 1;//SettingsMenu.MouseSensitivity;
        yaw += inputMouse.x * mouseSensitivity * settingsMultiplier;
        pitch -= inputMouse.y * mouseSensitivity * settingsMultiplier;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.eulerAngles = Vector3.up * yaw;
        cinemashineCameraTarget.transform.localEulerAngles = Vector3.right * pitch;
    }

    private void MovePlayer()
    {
        if (FreezMovement)
            return;

        Vector3 inputDir = new Vector3(inputMove.x, 0, inputMove.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);
        
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);

        if (isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;

            if (canJump && inputJump && verticalVelocity < 1f)
                verticalVelocity = Mathf.Sqrt(currentJumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        controller.Move(velocity * Time.deltaTime);

        Vector3 animationVelocity = transform.TransformDirection(Vector3.forward);
        //playerAnimator.SetFloat("xVelocity", animationVelocity.x);
        //playerAnimator.SetFloat("yVelocity", animationVelocity.z);
        playerAnimator.SetFloat("xVelocity", inputDir.x * currentSpeed);
        playerAnimator.SetFloat("yVelocity", inputDir.z * currentSpeed);
        shadowPlayerAnimator.SetFloat("xVelocity", inputDir.x * currentSpeed);
        shadowPlayerAnimator.SetFloat("yVelocity", inputDir.z * currentSpeed);
        legAnimator.SetFloat("xVelocity", inputDir.x * currentSpeed);
        legAnimator.SetFloat("yVelocity", inputDir.z * currentSpeed);
    }


    public void SetPosition(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        velocity = Vector3.zero;
        smoothV = Vector3.zero;
        verticalVelocity = 0;
        controller.enabled = true;
    }
    public void SetRotation(float rotX, float rotY)
    {
        if (rotX > 180)
            rotX -= 360;

        inputMouse.y = 0;
        pitch = rotX;
        cinemashineCameraTarget.transform.localEulerAngles = Vector3.right * rotX;

        inputMouse.x = 0;
        yaw = rotY;
        transform.eulerAngles = Vector3.up * rotY;
    }

    private void HandlePersChange()
    {
        if (Input.GetKeyDown(persChangeKey))
        {
            isCameraInFpsState = !isCameraInFpsState;
            fpsCamera.gameObject.SetActive(isCameraInFpsState);
            //legZombiak.SetActive(isCameraInFpsState);
        }
    }

    private void HandleZoom()
    {
        if (Input.GetKey(zoomKey))
        {
            if (isCameraInFpsState)
            {
                fpsAimCamera.gameObject.SetActive(true);
            }
            else
            {
                tpsAimCamera.gameObject.SetActive(true);
            }
        }
        else
        {
            if (isCameraInFpsState)
            {
                fpsAimCamera.gameObject.SetActive(false);
            }
            else
            {
                tpsAimCamera.gameObject.SetActive(false);
            }
        }
    }

    private void HandleHeadbob()
    {

    }

    /*public void SetCameraBackgroundOnSkyBox(float viewRange)
    {
        playerCamera.clearFlags = CameraClearFlags.Skybox;
        playerCamera.farClipPlane = viewRange;
    }
    public void SetCameraBackgroundOnSolidColor(Color color, float viewRange)
    {
        playerCamera.farClipPlane = viewRange;
        playerCamera.clearFlags = CameraClearFlags.SolidColor;
        playerCamera.backgroundColor = color;
    }*/

    private void SetEnable() => SetEnable(true);
    private void SetDisable() => SetEnable(false);
    public void SetEnable(bool enableState)
    {
        if(enableState)
        {
            FreezCamera = true;
            enabled = false;
        }else
        {
            FreezCamera = false;
            enabled = true;
        }
    }
}
