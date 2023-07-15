using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public PlayerHealth playerHealth;
    public Inventory inventory;
    private List<string> itemKeys;
    private int selectedItemIndex = 0;
    public float speed;
    public float runningSpeed;
    public float crouchingSpeed;
    public float gravity = -9.81f;
    public float crouchHeight = 0.5f;
    public float standHeight = 2.0f;
    public float respawnTime = 4.0f;
    private Vector3 respawnPoint;

    private Vector3 velocity;
    private float originalStepOffset;
    public Animator playerAnimator;

    private void Start()
    {
        originalStepOffset = controller.stepOffset;
        inventory = new Inventory();
        itemKeys = new List<string>();
        respawnPoint = transform.position;

    }

    private void Update()
    {

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (playerHealth.isDead)
        {
            StartCoroutine(Respawn());
            return;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Reset all triggers
        playerAnimator.ResetTrigger("Idle");
        playerAnimator.ResetTrigger("Walking");
        playerAnimator.ResetTrigger("Running");

        CycleItems();

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
            controller.Move(move * crouchingSpeed * Time.deltaTime);
            controller.height = crouchHeight;
            controller.stepOffset = 0;
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime);
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

    public void CycleItems()
    {
        // Update the itemKeys list to match the current inventory
        itemKeys = new List<string>(inventory.GetKeys());

        // Cycle through items with mouse wheel
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll > 0f)
        {
            // Scroll up
            selectedItemIndex++;
            if (selectedItemIndex >= itemKeys.Count)
            {
                selectedItemIndex = 0;  // Loop back to first item
            }
        }
        else if (mouseScroll < 0f)
        {
            // Scroll down
            selectedItemIndex--;
            if (selectedItemIndex < 0)
            {
                selectedItemIndex = itemKeys.Count - 1;  // Loop back to last item
            }
        }

        // Select item with number keys
        for (int i = 1; i <= 9; i++)  // Supports up to 9 items
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                if (i <= itemKeys.Count)
                {
                    selectedItemIndex = i - 1;
                }
            }
        }

        // Use the currently selected item (if there is one)
        if (itemKeys.Count > 0)
        {
            Debug.Log("Currently selected item: " + itemKeys[selectedItemIndex]);
            string selectedItemName = itemKeys[selectedItemIndex];
            if (Input.GetKeyDown(KeyCode.E))  // Press 'E' to use the item
            {
                if (selectedItemName == "HealthPotion" && playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    // Try to use the item. Only proceed if the item could be used (i.e., the item was in the inventory).
                    if (inventory.UseItem(selectedItemName))
                    {
                        Debug.Log("Used Potion");
                        playerHealth.Heal(50); // This heals the player
                        playerHealth.StopBleeding(); // stops the bleeding

                        // Check if item is still in the inventory
                        if (inventory.GetItemCount(selectedItemName) == 0)
                        {
                            selectedItemIndex = 0; // Reset the selected item index
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to use item: " + selectedItemName);
                    }
                }
                else if (selectedItemName == "Key" && inventory.UseItem(selectedItemName))
                {
                    Debug.Log("Unlocked Door");
                    // Perform any additional actions needed when the key is used.
                    if (inventory.GetItemCount(selectedItemName) == 0)
                    {
                        selectedItemIndex = 0; // Reset the selected item index
                    }
                }
            }
        }
    }

    IEnumerator Respawn()
    {

        // Wait for the respawn time
        yield return new WaitForSeconds(respawnTime);

        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the Scene
        SceneManager.LoadScene(currentScene.name);
    }
}