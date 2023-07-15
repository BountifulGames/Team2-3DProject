using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Injured : MonoBehaviour
{
    public RawImage image;
    public PlayerHealth playerHealth;
    public float pulseSpeed = 1f;
    public float maxAlpha = 1f;
    public float minAlpha = 0.5f;


    void Start()
    {
        image.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerHealth.isBleeding)
        {
            image.gameObject.SetActive(true);
            StartCoroutine(Pulse());
        }
        else
        {
            StopAllCoroutines();
            // Reset the image's alpha after pulsing
            Color imageColor = image.color;
            imageColor.a = maxAlpha;
            image.color = imageColor;
            image.gameObject.SetActive(false);
        }
    }
    IEnumerator Pulse()
    {

        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, maxAlpha - minAlpha) + minAlpha;
            Color imageColor = image.color;
            imageColor.a = alpha;
            image.color = imageColor;
            yield return null;
        }
    }
}
