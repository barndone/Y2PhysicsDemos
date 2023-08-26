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
    int lastProjectedCollisionCount = -1;
    Collider[] lastProjectedCollisions = new Collider[100];

    private void Awake()
    {
        
        if (TryGetComponent<PlayerInput>(out input))
        {
            moveAction = input.currentActionMap.FindAction("Move");
        }
        else
        {
            Debug.LogError("ERROR: Missing PlayerInput component on: " + this.gameObject.name, this);
        }

        if (TryGetComponent<Rigidbody>(out rb))
        {
            //  do nothing if succesful
        }
        else
        {
            Debug.LogError("ERROR: Missing Rigidbody component on: " + this.gameObject.name, this);
        }

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
        Vector3 projectedPos = rb.position + moveWish * (speed * Time.deltaTime);

        lastProjectedCollisionCount = Physics.OverlapBoxNonAlloc(projectedPos, Vector3.one / 2.0f, lastProjectedCollisions);

        for (int i = 0; i < lastProjectedCollisionCount; i++)
        {
            Collider other = lastProjectedCollisions[i];

            //  ignore our own collider
            if (col == other) { continue; }

            Debug.Log(other.name, other);

            bool isPenetrating = Physics.ComputePenetration(col, projectedPos, Quaternion.identity, other, other.transform.position, other.transform.rotation, out Vector3 penDir, out float penDepth);

            if (isPenetrating)
            {
                projectedPos += penDir * penDepth;
            }
        }

        rb.MovePosition(projectedPos);
    }
}
