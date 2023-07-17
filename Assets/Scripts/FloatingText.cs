using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    Transform mainCam;
    Transform unit;
    Transform worldSpaceCanvas;

    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        worldSpaceCanvas = GameObject.FindObjectOfType<Canvas>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
