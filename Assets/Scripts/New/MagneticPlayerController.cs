using System.Collections;
using UnityEngine;
using Terresquall;

public class MagneticPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpStrength = 5f;
    public float gravityForce = -9.81f;
    public float boostFactor = 2f;
    public float boostTime = 0.5f;
    public float polaritySwitchInterval = 4f;

    [Header("References")]
    public CharacterController controller;
    public Rect joystickArea = new Rect(0, 0, 300, 300); // Define joystick area (e.g., bottom left corner)

    [Header("Magnetic Properties")]
    public Material northPoleMaterial;
    public Material southPoleMaterial;
    private bool isNorthPoleActive = true;

    private float polaritySwitchTime;
    private Vector3 movementVelocity;
    private Vector3 movementInput;

    // Double-Tap Detection Variables
    private float lastTapTime = 0f;
    private bool isBoostActive = false;
    private float boostEndTime;

    void Start()
    {
        QualitySettings.vSyncCount = 0; // Disable VSync
        Application.targetFrameRate = 100; // Adjust for performance

        SwitchPolarity();
        polaritySwitchTime = Time.time + polaritySwitchInterval;
    }

    void Update()
    {
        ProcessInput();
        DetectDoubleTap(); // Separate method for double-tap detection
        CheckPolaritySwitch();
        ApplyGravityAndJump();
        CheckBoostEnd(); // Check if boost duration has ended
    }

    private void ProcessInput()
    {
        // Joystick-controlled movement
        float horizontal = -VirtualJoystick.GetAxis("Vertical");
        float vertical = VirtualJoystick.GetAxis("Horizontal");

        // Set movement speed based on whether boost is active
        float currentSpeed = isBoostActive ? speed * boostFactor : speed;
        movementInput = new Vector3(horizontal, 0, vertical).normalized * currentSpeed;

        // Jump control with a second touch
        if (Input.touchCount > 1)
        {
            Touch jumpTouch = Input.GetTouch(1);
            if (jumpTouch.phase == TouchPhase.Ended && jumpTouch.deltaPosition.y > 5 && controller.isGrounded)
            {
                Jump();
            }
        }
    }

    private void DetectDoubleTap()
    {
        // Iterate through touches and detect double-tap outside of joystick area
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPosition = touch.position;

            // Check if touch is outside joystick area
            if (!joystickArea.Contains(touchPosition))
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    if (Time.time - lastTapTime < 0.3f) // Adjustable interval for double-tap detection
                    {
                        ActivateBoost();
                        //Debug.Log("Double-tap detected: Boost activated!");
                    }
                    else
                    {
                        //Debug.Log("Single tap detected outside joystick area, waiting for second tap...");
                    }
                    lastTapTime = Time.time;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Combine movement and vertical velocity
        Vector3 move = movementInput * Time.deltaTime;
        move.y = movementVelocity.y * Time.deltaTime;

        controller.Move(move);
        AlignRotationWithMovement();
    }

    private void AlignRotationWithMovement()
    {
        // Smooth rotation towards movement direction
        if (movementInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void Jump()
    {
        if (controller.isGrounded)
        {
            movementVelocity.y = Mathf.Sqrt(jumpStrength * -2f * gravityForce);
        }
    }

    private void ApplyGravityAndJump()
    {
        if (controller.isGrounded && movementVelocity.y < 0)
        {
            movementVelocity.y = -2f;
        }
        else
        {
            movementVelocity.y += gravityForce * Time.deltaTime;
        }
    }

    private void CheckPolaritySwitch()
    {
        if (Time.time >= polaritySwitchTime)
        {
            SwitchPolarity();
            polaritySwitchTime = Time.time + polaritySwitchInterval;
        }
    }

    private void SwitchPolarity()
    {
        isNorthPoleActive = !isNorthPoleActive;
        GetComponent<Renderer>().material = isNorthPoleActive ? northPoleMaterial : southPoleMaterial;
    }

    public bool IsNorthPoleActive()
    {
        return isNorthPoleActive;
    }

    public void ApplyMagneticForce(Vector3 force)
    {
        controller.Move(force * Time.deltaTime);
    }

    private void ActivateBoost()
    {
        isBoostActive = true;
        boostEndTime = Time.time + boostTime;
    }

    private void CheckBoostEnd()
    {
        if (isBoostActive && Time.time >= boostEndTime)
        {
            isBoostActive = false;
            //Debug.Log("Boost ended.");
        }
    }
}
