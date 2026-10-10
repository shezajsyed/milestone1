using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 7f;

    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.25f;

    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float turnSpeed = 12f;

    [Header("Landing Impulse")]
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float minLandingSpeed = 4f;
    [SerializeField] float maxLandingSpeed = 12f;

    [Header("Crouch Collider")]
    [SerializeField] float crouchHeightRatio = 0.6f;
    [SerializeField] LayerMask ceilingMask;

    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBufferTime = 0.12f;
    
    Animator animator;
    Rigidbody rb;
    Vector2 moveInput;
    bool isRunning;
    bool isGrounded;
    bool isCrouching;
    float jumpCooldownTimer = 0f;
    bool wasGrounded;
    float peakFallSpeed;
    float coyoteTimer;
    float jumpBufferTimer;

    [Header("Landing Audio")]
    [SerializeField] AudioClip landingClip;
    [SerializeField, Range(0f, 1f)] float landingVolume = 0.8f;
    AudioSource audioSource;

    InputAction crouchAction;

    CapsuleCollider capsule;
    float standHeight;
    Vector3 standCenter;
    bool crouchHeld;

    void ApplyCrouchCollider()
    {
        float targetHeight = isCrouching ? standHeight * crouchHeightRatio : standHeight;
        float bottom = standCenter.y - standHeight * 0.5f;   // keep the feet fixed

        capsule.height = targetHeight;
        capsule.center = new Vector3(standCenter.x, bottom + targetHeight * 0.5f, standCenter.z);
    }
    
    bool CanStandUp()
    {
        float s = transform.lossyScale.y;
        float r = capsule.radius * s * 0.95f;
        float bottomLocal = standCenter.y - standHeight * 0.5f;

        Vector3 p1 = transform.TransformPoint(new Vector3(standCenter.x, bottomLocal, standCenter.z))
                 + Vector3.up * (r + 0.05f);
        Vector3 p2 = transform.TransformPoint(new Vector3(standCenter.x, bottomLocal + standHeight, standCenter.z))
                 - Vector3.up * r;

        return !Physics.CheckCapsule(p1, p2, r, ceilingMask, QueryTriggerInteraction.Ignore);
    }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animator.applyRootMotion = false;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (rb != null)
        {
            rb.WakeUp();
        }

        crouchAction = GetComponent<PlayerInput>().actions["Crouch"];

        capsule = GetComponent<CapsuleCollider>();
        standHeight = capsule.height;
        standCenter = capsule.center;

        audioSource = GetComponent<AudioSource>();
    }
    
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    void OnSprint(InputValue value)
    {
        isRunning = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed) jumpBufferTimer = jumpBufferTime;
    }

    void Update()
    {
        
        crouchHeld = crouchAction.IsPressed();
 
        if (crouchHeld)
        {
            isCrouching = true;
        }
        else if (isCrouching && CanStandUp())
        {
            isCrouching = false;
        }
        
        ApplyCrouchCollider();
        animator.SetBool("Crouch", isCrouching);

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
        
        if (jumpCooldownTimer <= 0f)
        {
            if (groundCheck != null)
            {
                isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
            }
            else
            {
                isGrounded = true; // Fallback safety
            }
        }  

        if (isGrounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;
        
        jumpBufferTimer -= Time.deltaTime;

        // Track how fast we fell, because physics zeroes velocity on the landing frame
        if (!isGrounded)
        {
            peakFallSpeed = Mathf.Max(peakFallSpeed, -rb.linearVelocity.y);
        }
        
        if (isGrounded && !wasGrounded)
        {
            if (peakFallSpeed >= minLandingSpeed)
            {
                float force = Mathf.InverseLerp(minLandingSpeed, maxLandingSpeed, peakFallSpeed);
                
                if (impulseSource != null)
                impulseSource.GenerateImpulseWithForce(Mathf.Lerp(0.2f, 1f, force));
                
                if (audioSource != null && landingClip != null)
                {
                    audioSource.pitch = Random.Range(0.92f, 1.08f);
                    audioSource.PlayOneShot(landingClip, Mathf.Lerp(0.4f, 1f, force) * landingVolume);
                }
            }
            peakFallSpeed = 0f;
        }
        wasGrounded = isGrounded;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f && jumpCooldownTimer <= 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpCooldownTimer = 0.2f;
            isGrounded = false;
            
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
            animator.SetTrigger("Jump");
        }

        float blendMultiplier = 0f;
        if (moveInput.magnitude > 0.1f)
        {
            blendMultiplier = isRunning ? 1f : 0.5f;
        }

        if (animator != null)
        {
            animator.SetFloat("MoveX", moveInput.x * blendMultiplier);
            animator.SetFloat("MoveY", moveInput.y * blendMultiplier);
        }

    }

    void FixedUpdate()
    {
        // Movement is done with MovePosition, so any leftover horizontal velocity is unwanted
        Vector3 v = rb.linearVelocity;
        v.x = 0f;
        v.z = 0f;
        rb.linearVelocity = v;

        // Camera-relative directions, flattened onto the ground plane
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight   = cameraTransform.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        // Turn the character to face where the camera looks
        if (camForward.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(camForward);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            float speed = isRunning ? runSpeed : walkSpeed;
            Vector3 dir = camForward * moveInput.y + camRight * moveInput.x;
            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
        }

    }
} 