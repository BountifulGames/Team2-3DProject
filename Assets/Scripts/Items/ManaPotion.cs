using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Inventory.Instance.AddItem("ManaPotion");
        Debug.Log("Picked Up Mana Potion");
        Destroy(gameObject); // Destroy the potion after picking it up
    }
}
