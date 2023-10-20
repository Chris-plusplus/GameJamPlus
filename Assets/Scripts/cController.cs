using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class cController : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] public bool canLook = true;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode flashlightKey = KeyCode.T;
    [SerializeField] private KeyCode perspChangeKey = KeyCode.Q;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float slopeSlideSpeed = 10.0f;

    [Header("Cinemashine")]
    [SerializeField] private GameObject cinemashineCameraTarget;
    [SerializeField] private float topCameraClamp = 89.5f;
    [SerializeField] private float bottomCameraClamp = -89.5f;
    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera fpsAimCamera;
    [SerializeField] private CinemachineVirtualCamera tpsCamera;
    [SerializeField] private CinemachineVirtualCamera tpsAimCamera;

    [Header("Movement Sounds")]
    [SerializeField] private AudioClip multiJumpSound;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultipier = 0.6f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;

    private Camera playerCamera;
    private CharacterController characterController;

    public Vector3 moveDirection;
    public Vector2 currentInput;
    private float rotationX = 0;

    public bool CanMove { get; private set; } = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private float GetCurrentOffset => IsSprinting ? baseStepSpeed * sprintStepMultipier : baseStepSpeed;

    private void Update()
    {
        
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
    }
}
