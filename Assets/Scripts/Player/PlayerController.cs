using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public PlayerHealth playerHealth;
    public Mana mana;
    public Inventory inventory;
    private List<string> itemKeys;
    private int selectedItemIndex = 0;
    public float speed;
    public float runningSpeed;
    public float crouchingSpeed;
    public float gravity = -9.81f;
    public float crouchHeight;
    public float standHeight;
    public float respawnTime;
    public bool usedMana;
    public Petrify petrify;



    private Vector3 velocity;
    private float originalStepOffset;
    public Animator playerAnimator;

    public AudioSource walkingAudio;
    public AudioSource runningAudio;
    public AudioSource crouchAudio;
    public AudioSource potionAudio;

    public bool IsPlayerRunning { get; private set; }

    private void Start()
    {
        originalStepOffset = controller.stepOffset;
        inventory = new Inventory();
        itemKeys = new List<string>();
        usedMana = false;

    }

    private void Update()
    {

        CycleItems();

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (playerHealth.isDead)
        {
            StartCoroutine(Respawn());
            return;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Calculate speed multiplier based on direction
        float speedMultiplier = 1;
        if (z < 0) // moving backwards
        {
            speedMultiplier = 0.5f; // 50% of the original speed
        }
        else if (x != 0) // moving sideways
        {
            speedMultiplier = 0.75f; // 75% of the original speed
        }


        if (move != Vector3.zero && !playerHealth.isDead) // player is moving
        {
            if (Input.GetKey(KeyCode.LeftShift) && z > 0 && !Input.GetKey(KeyCode.LeftControl))
            {
                controller.Move(move * runningSpeed * speedMultiplier * Time.deltaTime);
                playerAnimator.SetTrigger("Running");
                IsPlayerRunning = true;
                if (!runningAudio.isPlaying)
                {
                    PlayAtRandomPoint(runningAudio);
                }
                if (walkingAudio.isPlaying)
                {
                    walkingAudio.Stop();
                }
            }
            else
            {
                controller.Move(move * speed * speedMultiplier * Time.deltaTime);
                playerAnimator.SetTrigger("Walking");
                IsPlayerRunning = false;
                if (!walkingAudio.isPlaying)
                {
                    PlayAtRandomPoint(walkingAudio);
                }
                if (runningAudio.isPlaying)
                {
                    runningAudio.Stop();
                }
            }
        }
        else
        {
            playerAnimator.SetTrigger("Idle");
            if (runningAudio.isPlaying)
            {
                runningAudio.Stop();
            }
            if (walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }
        }

        // Crouch
        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.Move(move * crouchingSpeed  * speedMultiplier * Time.deltaTime);
            controller.height = crouchHeight;
            controller.stepOffset = 0;
            if(walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }
            if (runningAudio.isPlaying)
            {
                runningAudio.Stop();
            }

            if (!crouchAudio.isPlaying)
            {
                PlayAtRandomPoint(crouchAudio);
            }
        }
        else
        {
            controller.Move(move * speed * speedMultiplier * Time.deltaTime);
            controller.height = standHeight;
            controller.stepOffset = originalStepOffset;
            if (crouchAudio.isPlaying)
            {
                crouchAudio.Stop();
            }
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void CycleItems()
    {
        // Update the itemKeys list to match the current inventory
        itemKeys = new List<string>(Inventory.Instance.GetKeys());

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
            // Debug.Log("Currently selected item: " + itemKeys[selectedItemIndex]);
            string selectedItemName = itemKeys[selectedItemIndex];
            if (Input.GetKeyDown(KeyCode.E))  // Press 'E' to use the item
            {
                if (selectedItemName == "HealthPotion" && playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    // Try to use the item. Only proceed if the item could be used
                    if (Inventory.Instance.UseItem(selectedItemName))
                    {
                        Debug.Log("Used Potion");
                        playerHealth.Heal(50); // This heals the player
                        playerHealth.StopBleeding(); // stops the bleeding
                        potionAudio.Play();

                        // Check if item is still in the inventory
                        if (Inventory.Instance.GetItemCount(selectedItemName) == 0)
                        {
                            selectedItemIndex = 0; // Reset the selected item index
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to use item: " + selectedItemName);
                    }
                } else if (selectedItemName == "ManaPotion" && playerHealth.currentMana < playerHealth.maxMana)
                {
                    // Try to use the item. Only proceed if the item could be used
                    if (Inventory.Instance.UseItem(selectedItemName))
                    {
                        Debug.Log("Used Mana Potion");
                        playerHealth.Mana(100); // This gives player mana
                        potionAudio.Play();
                        usedMana = true;

                        // Check if item is still in the inventory
                        if (Inventory.Instance.GetItemCount(selectedItemName) == 0)
                        {
                            selectedItemIndex = 0; // Reset the selected item index
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to use item: " + selectedItemName);
                    }
                }
                else if (selectedItemName == "Key" && Inventory.Instance.UseItem(selectedItemName))
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

    void PlayAtRandomPoint(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = Random.Range(0, audioSource.clip.length);
            audioSource.Play();
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