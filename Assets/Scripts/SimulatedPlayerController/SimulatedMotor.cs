using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimulatedMotor : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector3 moveWish;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;


    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float groundAcceleration = 5.0f;
    [SerializeField] float groundFriction;


    private bool sprintWish = false;

    [SerializeField] float maxWalkSpeed = 20f;
    [SerializeField] float maxSprintSpeed = 35f;
    [SerializeField] float sprintMultiplier = 1.5f;

    [SerializeField] LayerMask groundLayers;
    [SerializeField] float maxGroundAngle = 60.0f;

    [SerializeField] float desiredHoverDistance = 0.1f;
    [SerializeField] float hoverStrength = 50.0f;
    [SerializeField] float hoverDamper = 10.0f;

    const int MaxGroundHitCapacity = 32;
    private RaycastHit[] lastGroundHits = new RaycastHit[MaxGroundHitCapacity];
    private int lastGroundHitCount = 0;

    private bool jumpWish;
    [SerializeField] private float jumpForce = 50.0f;
    private bool isGrounded;

    [SerializeField] private bool useCustomGravity = false;
    [SerializeField] private Vector3 gravity;

    // Start is called before the first frame update
    void Awake()
    {
        //  Check that the object this script is attached to has a PlayerInput component
        if (TryGetComponent<PlayerInput>(out input))
        {
            moveAction = input.currentActionMap.FindAction("Move");
            jumpAction = input.currentActionMap.FindAction("Jump");
            sprintAction = input.currentActionMap.FindAction("Sprint");
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
        if (TryGetComponent<CapsuleCollider>(out col))
        {
            //  do nothing if succesful
        }
        else
        {
            Debug.LogError("ERROR: Missing collider component on: " + this.gameObject.name, this);
        }

        if (useCustomGravity)
        {
            //  otherwise do nothing?
        }
        else
        {
            gravity = Physics.gravity;
        }
    }

    private void HandleJump(InputAction.CallbackContext obj)
    {
        jumpWish = true;
    }

    private void HandleMovement(InputAction.CallbackContext obj)
    {

        if (obj.performed)
        {
            Vector3 moveInput = obj.ReadValue<Vector2>();
            moveWish.x = moveInput.x;
            moveWish.z = moveInput.y;
        }

        else if (obj.canceled)
        {
            moveWish.x = moveWish.z = 0.0f;
        }
    }

    private void HandleSprint(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            sprintWish = true;
            maxSpeed = maxSprintSpeed;
        }

        else if (obj.canceled)
        {
            sprintWish = false;
            maxSpeed = maxWalkSpeed;
        }
    }

    private void OnEnable()
    {
        var boundActionMap = input.currentActionMap;
        if (boundActionMap != null)
        {
            jumpAction.performed += HandleJump;
            moveAction.performed += HandleMovement;
            moveAction.canceled  += HandleMovement;

            sprintAction.performed += HandleSprint;
            sprintAction.canceled += HandleSprint;
        }
    }

    private void OnDisable()
    {
        var boundActionMap = input.currentActionMap;
        if (boundActionMap != null)
        {
            jumpAction.performed -= HandleJump;
            moveAction.performed -= HandleMovement;
            moveAction.canceled  -= HandleMovement;

            sprintAction.performed -= HandleSprint;
            sprintAction.canceled  -= HandleSprint;
        }
    }

    //  apply changes to physics object
    private void FixedUpdate()
    {
        Vector3 projectedVelocity = rb.velocity;

        //  apply friction
        float keepY = projectedVelocity.y;
        projectedVelocity.y = 0.0f;

        float groundSpeed = projectedVelocity.magnitude;
        if (groundSpeed != 0)
        {
            float frictionAccel = groundSpeed * groundFriction * Time.deltaTime;
            projectedVelocity *= Mathf.Max(groundSpeed - frictionAccel, 0) / groundSpeed;
        }

        projectedVelocity.y = keepY;

        float projectedMagnitude = Vector3.Dot(projectedVelocity, moveWish);
        float accelerationMagnitude = groundAcceleration * Time.deltaTime;

        if (sprintWish)
        {
            accelerationMagnitude *= sprintMultiplier;
        }

        if (projectedMagnitude + accelerationMagnitude > maxSpeed)
        {
            accelerationMagnitude = maxSpeed - projectedMagnitude;
        }

        projectedVelocity += moveWish * accelerationMagnitude;


        //  handle gravity
        projectedVelocity += gravity * Time.deltaTime;

        // handle ground
        Vector3 castOrigin = transform.TransformPoint(col.center);
        float castLength = (col.height / 2.0f) - col.radius + desiredHoverDistance;

        Debug.Assert(desiredHoverDistance >= 0.0f, "Hover distance is too short! must be non-negative", this);

        lastGroundHitCount = Physics.SphereCastNonAlloc(
            castOrigin, col.radius, Vector3.down, lastGroundHits, castLength, groundLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < lastGroundHitCount; i++)
        {
            RaycastHit hit = lastGroundHits[i];

            // our own body? skip!
            if (hit.collider == col) { continue; }

            // already inside? skip!
            if (hit.point == Vector3.zero &&
                hit.distance == 0.0f) { continue; }

            // too steep? skip!
            float groundAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (groundAngle > maxGroundAngle) { continue; }

            isGrounded = true;

            float distanceFromGround = hit.distance - col.height / 2.0f + col.radius;
            float displacement = desiredHoverDistance - distanceFromGround;

            float hoverForce = hoverStrength * displacement - hoverDamper * projectedVelocity.y;
            rb.AddForce(Vector3.up * hoverForce);
        }
        //  handle jumping
        if (jumpWish)
        {
            //  if we are grounded
            if (isGrounded)
            {
                projectedVelocity.y += jumpForce;
                isGrounded = false;
            }

            jumpWish = false;
        }

        // update player velocity
        rb.velocity = projectedVelocity;
    }
}
