using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class KinematicMotor : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    private InputAction moveAction;

    private Rigidbody rb;

    private Collider col;

    [SerializeField] float speed = 5.0f;

    private Vector3 moveWish;

    [SerializeField] private float maxSpeed = 8.0f;
    [SerializeField] private float acceleration = 12.0f;
    [SerializeField] private float maxGroundAngle = 60.0f;

    public bool useGravity;
    [SerializeField] private bool preventClimbing;

    int lastProjectedCollisionCount = -1;
    Collider[] lastProjectedCollisions = new Collider[100];

    public Vector3 Velcotiy { get; private set; }

    private void Awake()
    {
        //  Check that the object this script is attached to has a PlayerInput component
        if (TryGetComponent<PlayerInput>(out input))
        {
            moveAction = input.currentActionMap.FindAction("Move");
        }
        else
        {
            Debug.LogError("ERROR: Missing PlayerInput component on: " + this.gameObject.name, this);
        }

        //  check that the object this script is attached to has a Rigidbody component
        if (TryGetComponent<Rigidbody>(out rb))
        {
            //  do nothing if succesful
        }
        else
        {
            Debug.LogError("ERROR: Missing Rigidbody component on: " + this.gameObject.name, this);
        }

        //  check that the object this script is attached to has a collider component
        if (TryGetComponent<Collider>(out col))
        {
            //  do nothing if succesful
        }
        else
        {
            Debug.LogError("ERROR: Missing collider component on: " + this.gameObject.name, this);
        }
    }

    private void Update()
    {
        var moveVec = moveAction.ReadValue<Vector2>();

        moveWish.x = moveVec.x;
        moveWish.z = moveVec.y;
    }

    private void FixedUpdate()
    {
        //  process imput
        float projectedVelocity = Vector3.Dot(Velcotiy, moveWish);
        float accelerationMagnitude = acceleration * Time.deltaTime;

        if (projectedVelocity + accelerationMagnitude > maxSpeed)
        {
            accelerationMagnitude = maxSpeed - projectedVelocity;
        }

        Velcotiy += moveWish * accelerationMagnitude;

        //  process forces
        if (useGravity)
        {
            Velcotiy += Physics.gravity * Time.deltaTime;
        }

        Vector3 projectedPos = rb.position + Velcotiy * Time.deltaTime;

        lastProjectedCollisionCount = Physics.OverlapBoxNonAlloc(projectedPos, Vector3.one / 2.0f, lastProjectedCollisions);

        for (int i = 0; i < lastProjectedCollisionCount; i++)
        {
            Collider other = lastProjectedCollisions[i];

            //  ignore our own collider
            if (col == other) { continue; }

            bool isPenetrating = Physics.ComputePenetration(
                col, projectedPos, Quaternion.identity, 
                other, other.transform.position, other.transform.rotation, 
                out Vector3 penDir, out float penDepth);

            //  depenetrate if actually colliding
            if (isPenetrating)
            {
                //  projectedPos += penDir * penDepth;

                //  floor?
                if (penDir.y > 0 && Vector3.Angle(penDir, Vector3.up) < maxGroundAngle)
                {
                    projectedPos.y += penDir.y * penDepth;
                    Vector3 clippedVelocity = Vector3.ProjectOnPlane(Velcotiy, penDir);
                    clippedVelocity.x = Velcotiy.x;
                    clippedVelocity.z = Velcotiy.z;

                    //  only changes y, the x and z values were coppied over
                    Velcotiy = clippedVelocity;
                }

                //  walls and other objects
                else
                {
                    float oldPosY = projectedPos.y;

                    projectedPos += penDir * penDepth;

                    if (preventClimbing)
                    {
                        projectedPos.y = Mathf.Min(projectedPos.y, oldPosY);
                    }

                    float oldVelY = Velcotiy.y;

                    Vector3 clippedVelocity = Vector3.ProjectOnPlane(Velcotiy, penDir);

                    if (preventClimbing)
                    {
                        clippedVelocity.y = Mathf.Min(clippedVelocity.y, oldVelY);
                    }

                    Velcotiy = clippedVelocity;
                }
            }
        }

        rb.MovePosition(projectedPos);
    }
}
