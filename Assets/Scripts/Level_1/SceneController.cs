using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SceneController : MonoBehaviour
{
    [SerializeField] Volume levelOneVolume;

    // We will adjust this for fade in
    private float targetExposure = -1.83f;
    private float currentExposure = -20; // Assumes initial scene is completely black

    // Speed of the fade in effect
    [SerializeField] private float fadeInSpeed = .5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // Get ColorAdjustments component from the volume's shared profile
        if (levelOneVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            // Gradually increase exposure to fade in
            while (currentExposure < targetExposure)
            {
                currentExposure += fadeInSpeed;
                colorAdjustments.postExposure.value = currentExposure;
                yield return new WaitForSeconds(.05f); // Adjust this time to control fade in speed
            }
        }
    }
}
