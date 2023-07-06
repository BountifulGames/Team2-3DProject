using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    public float bobbingSpeed = 0.15f;
    public float bobbingAmount = 0.2f;
    public float midpoint = 2.0f;

    public PlayerController playerController; // Reference to PlayerController script

    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cSharpConversion = transform.localPosition;

        float currentBobbingSpeed = bobbingSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentBobbingSpeed = bobbingSpeed * 1.5f; // Increase bobbing speed while sprinting
        }

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer = timer + currentBobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = midpoint + translateChange;
        }
        else
        {
            cSharpConversion.y = Mathf.Lerp(cSharpConversion.y, midpoint, Time.deltaTime * currentBobbingSpeed);
        }

        transform.localPosition = cSharpConversion;
    }
}