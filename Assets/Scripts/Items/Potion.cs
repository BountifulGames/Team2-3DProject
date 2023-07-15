using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.inventory.AddItem("HealthPotion");
            Debug.Log("Picked Up Potion");
            Destroy(gameObject); // Destroy the potion after picking it up
        }
    }
}
