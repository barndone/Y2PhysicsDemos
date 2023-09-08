using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeLauncher : MonoBehaviour
{
    //   the time it takes for the slime to reach its user provided destination
    [SerializeField] float travelTime = 2.0f;

    [SerializeField] LayerMask slimeLayer;

    Camera cam;

    public Vector3 destination = new Vector3(0, 0, 0);

    Vector3 mousePos = new Vector3(0, 0, 0);

    [SerializeField] Rigidbody activeSlime = null;

    bool selectWish = false;
    bool launchWish = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            selectWish = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (activeSlime != null)
            {
                launchWish = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (selectWish)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (activeSlime != null)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                   // Debug.Log("Destination changed from: " + destination);
                    destination = hit.point;
                   // Debug.Log("Destination changed to: " + destination);
                }
            }

            else
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, slimeLayer))
                {
                    Debug.Log("Hit " + hit.transform.gameObject.name, hit.transform.gameObject);

                    activeSlime = hit.transform.gameObject.GetComponent<Rigidbody>();
                }
            }

            selectWish = false;
        }

        if (launchWish)
        {
            if (activeSlime != null)
            {
                activeSlime.AddForce(CalculateImpulseVelocity(activeSlime.position, destination, travelTime, Physics.gravity.y), ForceMode.Impulse);
            }
            launchWish = false;
        }
    }

    private Vector3 CalculateImpulseVelocity(Vector3 initialPos, Vector3 finalPos, float travelTime, float gravity)
    {
        Vector3 displacement = finalPos - initialPos;

        float xVelocity = CalculateInitialHorizontalVelocity(initialPos.x, displacement.x, travelTime);
        //Debug.Log("X Velocity: "+ xVelocity);
        float yVelocity = CalculateInitialVerticalVelocity(initialPos.y, displacement.y, travelTime, gravity);
        //Debug.Log("Y Velocity: " + yVelocity);
        float zVelocity = CalculateInitialHorizontalVelocity(initialPos.z, displacement.z, travelTime);
        //Debug.Log("Z Velocity: " + zVelocity);

        return new Vector3(xVelocity, yVelocity, zVelocity);

    }

    private float CalculateInitialHorizontalVelocity(float initialXPos, float deltaXPos, float travelTime)
    {
        return deltaXPos / travelTime;// + initialXPos;
    }

    private float CalculateInitialVerticalVelocity(float initialYPos, float deltaYPos, float travelTime, float gravity)
    {
        return (deltaYPos) - (initialYPos) - (0.5f * gravity * Mathf.Pow(travelTime, 2)) / (travelTime);
    }
}
