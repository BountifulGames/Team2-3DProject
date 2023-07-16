using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public bool isBleeding;
    private float bleedDamage = 2f; // Damage per second
    private Vector3 deathPos = Vector3.zero;
    public Animator animator;
    public bool isDead;
    public RawImage deathFade;
    public AudioSource heartAudio;
    public AudioSource deathHitAudio;
    public AudioSource deathBreathAudio;
    public PlayerController player;

    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        deathFade.gameObject.SetActive(false);

    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("<color=red> Player health: " + currentHealth + "</color>");

        if (currentHealth <= 0)
        {
            if (heartAudio.isPlaying)
            {
                heartAudio.Stop();
                player.walkingAudio.Stop();
                player.crouchAudio.Stop();
                player.runningAudio.Stop();
            };
            Die();
        };
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
        heartAudio.Play();

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
        isBleeding  = false;
        isDead = true;
        deathFade.gameObject.SetActive(true);

        if (!deathBreathAudio.isPlaying)
        {
            deathBreathAudio.Play();
        }
        if (!deathHitAudio.isPlaying)
        {
            deathHitAudio.Play();
        }

    }
}
