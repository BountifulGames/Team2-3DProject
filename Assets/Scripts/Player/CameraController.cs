using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }

    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] private float yRotationLimit = 45f;

    public Transform playerBody; // The player's body.

    private Vector2 rotation = Vector2.zero;
    private const string xAxis = "Mouse X";
    private const string yAxis = "Mouse Y";

    void Update()
    {
        if (playerHealth.isDead) return;
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        playerBody.localRotation = xQuat * yQuat; // Apply both rotations to the player's body.
        transform.localRotation = yQuat; // Apply vertical rotation to the camera.
    }
}
