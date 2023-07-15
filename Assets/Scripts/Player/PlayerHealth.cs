using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public bool isBleeding;
    private float bleedDamage = 2f; // Damage per second
    private Vector3 deathPos = Vector3.zero;
    public Animator animator;
    public bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("<color=red> Player health: " + currentHealth + "</color>");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void StartBleeding()
    {
        if (!isBleeding)
        {
            StartCoroutine(BleedOverTime());
        }
    }

    private IEnumerator BleedOverTime()
    {
        isBleeding = true;

        while (isBleeding)
        {
            TakeDamage(bleedDamage * Time.deltaTime);
            Debug.Log("<color=orange> Bleeding - Player health: " + currentHealth + "</color>");
            yield return null; // Wait until the next frame
        }
    }

    public void StopBleeding()
    {
        Debug.Log("Bleeding Stopped");
        isBleeding = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Min(currentHealth, maxHealth); // Make sure health doesn't go above maxHealth
        Debug.Log("<color=green> Player health: " + currentHealth + "</color>");
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Add additional logic here for what should happen when the player dies
        animator.SetTrigger("Dead");
        isDead = true;
        
    }
}
