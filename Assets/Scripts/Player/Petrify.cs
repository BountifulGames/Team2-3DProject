using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Petrify : MonoBehaviour
{
    public int magicCost = 100;
    public float magicRange = 10f;
    public float magicAngle = 30f;
    public PlayerController playerController;
    public PlayerHealth playerHealth;
    private float currentMana;
    private int enemyMask;

    private void Start()
    {
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
    }
    private void Update()
    {
        currentMana = playerHealth.currentMana;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            CastPetrify();
            Debug.Log("Current Mana: " + playerHealth.currentMana);
        }
    }

    private void CastPetrify()
    {
        Debug.Log("Attempting to cast Petrify");
        if (currentMana >= magicCost)
        {
            // Cast a ray forward from the player to detect enemies
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, magicRange, enemyMask))
            {      
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // Calculate angle to enemy
                    Vector3 toEnemy = (enemy.transform.position - transform.position).normalized;
                    float angleToEnemy = Vector3.Angle(transform.forward, toEnemy);
                    if (angleToEnemy <= magicAngle / 2f)
                    {
                        // Petrify enemy
                        enemy.GetComponentInChildren<Renderer>().material.color = Color.grey;
                        enemy.GetComponent<NavMeshAgent>().enabled = false;
                        enemy.GetComponent<Animator>().enabled = false;
                        enemy.isPetrified = true;
                        Debug.Log("Petrified Enemy");
                        // Use mana
                        ReduceMana(magicCost);
                    }
                }
            } else
            {
                Debug.Log("Failed to cast Petrify");
            }
        }
    }

    private void ReduceMana(int amount)
    {
        playerHealth.currentMana -= amount;
        // Make sure mana doesn't go below 0
        playerHealth.currentMana = Mathf.Max(currentMana, 0);
    }
}
