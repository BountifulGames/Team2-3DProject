using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float runningSpeed = 12f;
    public float gravity = -9.81f;
    public float crouchHeight = 0.5f;
    public float standHeight = 2.0f;

    private Vector3 velocity;
    private float originalStepOffset;
    public Animator playerAnimator;

    private void Start()
    {
        originalStepOffset = controller.stepOffset;
    }

    private void Update()
    {
        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Reset all triggers
        playerAnimator.ResetTrigger("Idle");
        playerAnimator.ResetTrigger("Walking");
        playerAnimator.ResetTrigger("Running");

        if (move != Vector3.zero) // player is moving
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                controller.Move(move * runningSpeed * Time.deltaTime);
                playerAnimator.SetTrigger("Running");
            }
            else
            {
                controller.Move(move * speed * Time.deltaTime);
                playerAnimator.SetTrigger("Walking");
            }
        }
        else
        {
            playerAnimator.SetTrigger("Idle");
        }

        // Crouch
        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            controller.stepOffset = 0;
        }
        else
        {
            controller.height = standHeight;
            controller.stepOffset = originalStepOffset;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            playerAnimator.SetBool("isAttacking", true);

        }

        if (playerAnimator.GetBool("isAttacking"))
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
                playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                playerAnimator.SetBool("isAttacking", false);

            }
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}