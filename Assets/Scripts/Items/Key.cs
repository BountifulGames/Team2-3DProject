using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    

    // In a script attached to the door:


    public void Interact()
    {
        Inventory.Instance.AddItem("Key");
        Debug.Log("Picked Up Key");
        Destroy(gameObject); // Destroy the Key after picking it up
    }
}
    

