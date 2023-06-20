using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 15f;
    [SerializeField] float jumpHeight = 6f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] float accelerationDecelerationSpeed = 0.1f;

    Transform cameraTransform;
    Animator animator;
    CharacterController controller;
    Vector3 velocity; 
    float turnSmoothVelocity;
    float currentSpeed = 0;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 moveDirection = GetMoveDirection();

        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            else if (velocity.y < 0)
                velocity.y = -2f;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;

        controller.Move((velocity + moveDirection) * Time.deltaTime);
        //print(new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude);
    }

    Vector3 GetMoveDirection()
    {
        Vector3 moveDirection;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Get the angle between the current rotation and the direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Rotate the player to the new angle smoothly
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            if (controller.isGrounded)
            {
                bool isRunning = Input.GetKey(KeyCode.LeftShift);
                float targetSpeed = isRunning ? runSpeed : walkSpeed;

                currentSpeed = CalculateNewCurrentSpeed(targetSpeed, accelerationDecelerationSpeed);
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward * currentSpeed;

                animator.SetBool("IsWalking", !isRunning);
                animator.SetBool("IsRunning", isRunning);
            } else
            {
                // Divide by 5 because it tkaes more times to decelerate in air
                currentSpeed = CalculateNewCurrentSpeed(0, accelerationDecelerationSpeed / 5);
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward * currentSpeed;

                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
            }
        }
        else
        {
            currentSpeed = CalculateNewCurrentSpeed(0, accelerationDecelerationSpeed);
            moveDirection = transform.forward * currentSpeed;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        return moveDirection;
    }

    private float CalculateNewCurrentSpeed(float targetSpeed, float smoothTime)
    {
        float newCurrentSpeed;
        if (currentSpeed < targetSpeed)
        {
            newCurrentSpeed = currentSpeed + smoothTime * Time.deltaTime;
            if (newCurrentSpeed > targetSpeed)
                newCurrentSpeed = targetSpeed;
        } else if (currentSpeed > targetSpeed)
        {
            newCurrentSpeed = currentSpeed - smoothTime * Time.deltaTime;
            if (newCurrentSpeed < targetSpeed)
                newCurrentSpeed = targetSpeed;
        } else
        {
            newCurrentSpeed = targetSpeed;
        }

        return newCurrentSpeed;
    }
}
