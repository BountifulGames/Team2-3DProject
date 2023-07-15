using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Picked Up Key");
            other.GetComponent<PlayerController>().inventory.AddItem("Key");
            Destroy(gameObject);  // Remove the key
        }
    }

    // In a script attached to the door:
    public void Interact(PlayerController player)
    {
        if (player.inventory.HasItem("Key"))
        {

        }
        else
        {
            // Key not found
        }
    }
}
