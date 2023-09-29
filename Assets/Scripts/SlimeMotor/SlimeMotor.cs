using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeMotor : MonoBehaviour
{
    [SerializeField] float verticalStrength = 1.0f;
    [SerializeField] float horizontalStrength = 1.0f;
    [SerializeField] float jumpDelay = 1.5f;
    private float jumpTimer = 0.0f;

    [SerializeField] float minJumpDelay = 0.5f;
    [SerializeField] float maxJumpDelay = 1.5f;

    public Rigidbody rb;


    [SerializeField] bool UseTransformTargeting = true;
    [SerializeField] Transform destinationTarget;
    private Vector3 destinationPos;

    private GameObject target;
    public GameObject Target { get { return target; } private set { target = value; } }
    [SerializeField] Vector3 destinationDirection;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] bool grounded;

    bool jumpWish = false;

    [SerializeField] int happinessNeeded = 10;
    int happinessTracker = 0;
    [SerializeField] float scaleIncrease = 1.5f;

    private bool levelUpWish = false;

    public LineRenderer lineRend;

    [SerializeField] int trajectoryVisSteps = 15;

    public bool alive = true;

    public AudioSource slimeSource;


    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out rb))
        {
            //  do nothing because we have a rb :D
            grounded = true;
        }

        else
        {
            Debug.LogError("No Rigidbody attached to " + this.name, this);
        }

        if (TryGetComponent<LineRenderer>(out lineRend))
        {

        }
        else
        {
            Debug.LogError("No LineRenderer component attached to " + this.name, this);
        }

        if (TryGetComponent<AudioSource>(out slimeSource))
        {
            //  do nothing
        }

        else { Debug.LogError("No AudioSource component attached to " + this.name, this); }

        lineRend.transform.position = transform.position;
    }

    private void Update()
    {
        if (alive)
        {
            if (grounded)
            {
                if (jumpTimer >= jumpDelay && !jumpWish)
                {
                    jumpWish = true;
                }

                else
                {
                    jumpTimer += Time.deltaTime;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        destinationDirection = (GetDestinationPosition(UseTransformTargeting) - rb.position).normalized;

        if (grounded)
        {
            DrawPath();
        }

        if (jumpWish)
        {
            destinationDirection = (GetDestinationPosition(UseTransformTargeting) - rb.position).normalized;
            jumpTimer = 0.0f;

            jumpDelay = Random.Range(minJumpDelay, maxJumpDelay);

            var force = new Vector3(destinationDirection.x * horizontalStrength, verticalStrength, destinationDirection.z * horizontalStrength);

            rb.AddForce(force, ForceMode.Impulse);
            grounded = false;
            jumpWish = false;
            slimeSource.PlayOneShot(AudioManager.instance.GetSlimeImpactSound());
        }

        if (levelUpWish)
        {
            rb.AddForce(Vector3.up * verticalStrength, ForceMode.Impulse);
            rb.gameObject.transform.localScale = rb.gameObject.transform.localScale * scaleIncrease;
            happinessTracker = 0;
            levelUpWish = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var firstContact = collision.GetContact(0);
        var contactPoint = firstContact.point;

        var collisionAngle = Quaternion.Euler((contactPoint - rb.position).normalized);

        if (1 << firstContact.otherCollider.gameObject.layer == groundLayer)
        {

            Debug.DrawRay(firstContact.point, firstContact.normal * 5, Color.red, 5.0f);
            grounded = true;

            DrawPath();
        }

        slimeSource.PlayOneShot(AudioManager.instance.GetSlimeImpactSound());
    }

    public void ApplyHappiness(int value)
    {
        happinessTracker += value;

        if (happinessTracker >= happinessNeeded)
        {
            levelUpWish = true;
        }
    }

    public void DrawPath()
    {
        lineRend.positionCount = 1;
        lineRend.SetPosition(0, transform.position);

        float gravity = Physics.gravity.y;
        float airTime = CalculateTimeToLand(verticalStrength, gravity);
        float timeElapsed = 0.0f;

        for (int i = 1; i <= trajectoryVisSteps; i++)
        {
            timeElapsed += airTime / trajectoryVisSteps;
            float deltaXPos = CalculateXDisplacement(0, horizontalStrength, timeElapsed);
            float deltaYPos = CalculateYDisplacement(0, verticalStrength, timeElapsed, gravity);
            Vector2 xzPositions = Vector2.Scale(new Vector2(destinationDirection.x, destinationDirection.z), new Vector2(deltaXPos, deltaXPos));
            Vector3 point = new(xzPositions.x, deltaYPos, xzPositions.y);
            Vector3 worldSpacePoint = transform.position + point;

            lineRend.positionCount++;
            lineRend.SetPosition(lineRend.positionCount - 1, worldSpacePoint);
        }
    }

    public float CalculateXDisplacement(float initialXPosition, float initialXVelocity, float timeStep)
    {
        return initialXPosition + initialXVelocity * timeStep;
    }

    public float CalculateYDisplacement(float initialYPosition, float initialYVelocity, float timeStep, float gravity)
    {
        //  pro tip for future brandon, never do (1 / 2) unless you specify it is a float because it will truncate to 0 you idiot
        return initialYPosition + (initialYVelocity * timeStep) + (0.5f * (gravity * Mathf.Pow(timeStep, 2)));
    }

    public float CalculateTimeToLand(float initialYVelocity, float gravity)
    {
        return (-initialYVelocity - Mathf.Sqrt(Mathf.Pow(initialYVelocity, 2))) / (gravity);
    }

    public void SetDestination(Vector3 targetPosition)
    {
        UseTransformTargeting = false;
        destinationTarget = null;
        Target = null;
        destinationPos = targetPosition;
    }

    public void SetDestination(Transform targetTransform)
    {
        UseTransformTargeting = true;
        destinationTarget = targetTransform;
        Target = targetTransform.gameObject;
        destinationPos = Vector3.zero;
    }

    public Vector3 GetDestinationPosition(bool useTransform)
    {
        if (useTransform)
        {
            return destinationTarget.position;
        }
        else
        {
            return destinationPos;
        }
    }

    public Transform GetDestinationTransform()
    {
        return destinationTarget;
    }
}
