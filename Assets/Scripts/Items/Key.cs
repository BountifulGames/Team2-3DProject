using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    

    // In a script attached to the door:


    public void Interact()
    {
        Inventory.Instance.AddItem("HealthPotion");
        Debug.Log("Picked Up Potion");
        Destroy(gameObject); // Destroy the Key after picking it up
    }
}
    

