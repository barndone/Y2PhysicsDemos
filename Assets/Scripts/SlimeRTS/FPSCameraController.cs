using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCameraController : MonoBehaviour
{
    public GameObject player;

    float xRotation;
    float yRotation;

    float yCameraOffset;

    Vector2 mouseMovement;
    Vector3 position;

    [SerializeField] float mouseSens = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        //  cache the Y-offset of the camera
        yCameraOffset = transform.position.y - player.transform.position.y;

        //  lock the cursor to the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        position = player.transform.position;
        position.y += yCameraOffset;
        transform.position = position;

        mouseMovement = Mouse.current.delta.ReadValue();

        xRotation -= mouseMovement.y * Time.deltaTime * mouseSens;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        yRotation += mouseMovement.x * Time.deltaTime * mouseSens;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        player.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}
