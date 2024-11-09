using System.Collections;
using UnityEngine;
using Terresquall;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float boostMultiplier = 2f;
    public float boostDuration = 0.5f;
    public float polarityToggleInterval = 4f;

    [Header("References")]
    public CharacterController characterController;

    [Header("Magnetic Properties")]
    public Material northMaterial;
    public Material southMaterial;
    private bool isNorthPole = true;

    private float nextPolarityToggleTime;
    private Vector3 velocity;
    private Vector3 horizontalMovement;


    void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = 100; // Set to 60 or higher for your requirement

        TogglePolarity();
        nextPolarityToggleTime = Time.time + polarityToggleInterval;
    }

    void Update()
    {
        HandleInput();
        TogglePolarityIfNeeded();
        ApplyGravityAndJump();
    }

    private void HandleInput()
    {
        // Get joystick input from the first touch (touch with fingerId 0)
        if (Input.touchCount > 0)
        {
            // Get joystick input and calculate horizontal movement direction
            float inputX = -VirtualJoystick.GetAxis("Vertical");
            float inputZ = VirtualJoystick.GetAxis("Horizontal");

            horizontalMovement = new Vector3(inputX, 0, inputZ).normalized * moveSpeed;
        }
        else
        {
            horizontalMovement = Vector3.zero; // Reset movement if no touch for joystick
        }

        // Check for jump input only if there is a second touch
        if (Input.touchCount > 1)
        {
            Touch jumpTouch = Input.GetTouch(1); // Second touch for jump detection

            if (jumpTouch.phase == TouchPhase.Ended)
            {
                // Detect upward swipe for jump
                if (jumpTouch.deltaPosition.y > 5 && characterController.isGrounded)
                {
                    Jump();
                }
            }
        }
    }



    private void FixedUpdate()
    {
        // Combine horizontal movement with vertical velocity
        //Vector3 move = horizontalMovement * Time.fixedDeltaTime;
        Vector3 move = horizontalMovement * Time.deltaTime;
        move.y = velocity.y * Time.deltaTime; // Apply vertical velocity for jumping and gravity

        // Apply the movement vector to CharacterController
        characterController.Move(move);
        RotateToFaceMovement();
    }

    private void RotateToFaceMovement()
    {
        // Check if there is any horizontal movement to rotate
        if (horizontalMovement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravityAndJump()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // Small value to ensure grounding
        }
        else
        {
            // Apply gravity over time
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void TogglePolarityIfNeeded()
    {
        if (Time.time >= nextPolarityToggleTime)
        {
            TogglePolarity();
            nextPolarityToggleTime = Time.time + polarityToggleInterval;
        }
    }

    private void TogglePolarity()
    {
        isNorthPole = !isNorthPole;
        GetComponent<Renderer>().material = isNorthPole ? northMaterial : southMaterial;
    }

    public bool IsNorthPole()
    {
        return isNorthPole; // Returns current polarity state of the player
    }

    public void ApplyMagneticForce(Vector3 force)
    {
        // Applies magnetic force to player?s movement
        characterController.Move(force * Time.deltaTime);
    }

}
