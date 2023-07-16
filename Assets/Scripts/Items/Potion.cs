using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Inventory.Instance.AddItem("HealthPotion");
        Debug.Log("Picked Up Potion");
        Destroy(gameObject); // Destroy the potion after picking it up
    }
}
