using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public RawImage image;
    public PlayerHealth playerHealth;
    public PlayerController playerController;
    public float animationDuration = 2.0f;

    void Start()
    {
        image.gameObject.SetActive(false);
    }
    private void Update()
    {
         UseManaPotion();
    }
    public void UseManaPotion()
    {
        if (playerController.usedMana)
        {
            image.gameObject.SetActive(true);
            StartCoroutine(ManaFx());
            playerController.usedMana = false;  
        }
    }

    private IEnumerator ManaFx()
    {
        float timeElapsed = 0;

        // Go from 0 -> 1
        while (timeElapsed < animationDuration / 2f)
        {
            timeElapsed += Time.deltaTime;

            float alpha = Mathf.SmoothStep(0, 1, timeElapsed / (animationDuration / 2f));

            // Set image color with new alpha
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            yield return null; // Wait until next frame
        }

        timeElapsed = 0;

        // Go from 1 -> 0
        while (timeElapsed < animationDuration / 2f)
        {
            timeElapsed += Time.deltaTime;

            float alpha = Mathf.SmoothStep(1, 0, timeElapsed / (animationDuration / 2f));

            // Set image color with new alpha
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            yield return null; // Wait until next frame
        }

        // Ensure the alpha is set back to 0 at the end of the animation
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        image.gameObject.SetActive(false);

    }
}
