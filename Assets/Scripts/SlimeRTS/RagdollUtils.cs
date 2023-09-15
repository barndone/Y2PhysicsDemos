using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollUtils : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider rbCollider;
    [SerializeField] GameObject meshRoot;
    [SerializeField] List<Collider> meshColliders = new List<Collider>();
    [SerializeField] static List<Rigidbody> ragdollJoints = new List<Rigidbody>();


    private static bool ragdollEnabled = false;

    private float yOffset;
    private Vector3 offset;

    //  ignore collisions between ragdoll and player motor
    //  also ignore collisions between player and slimes
    private void Start()
    {
        //  hard coded for ignoring collisions between slimes and the player
        Physics.IgnoreLayerCollision(3, 6, true);

        //  ignore collisions between the character motor and the ragdoll colliders
        foreach (Collider col in meshColliders)
        {
            Physics.IgnoreCollision(rbCollider, col, true);

            var joint = col.gameObject.GetComponent<Rigidbody>();

            ragdollJoints.Add(joint);
        }

        yOffset = rb.transform.position.y - 0.3f;
        offset = new Vector3(0f, yOffset, 0f);
    }

    private void Update()
    {
        if (!ragdollEnabled)
        {
            meshRoot.transform.position = rb.transform.position - offset;
            meshRoot.transform.rotation = rb.transform.rotation;
        }
    }

    public static void EnableRagdoll()
    {
        ragdollEnabled = true;


        foreach (Rigidbody joint in ragdollJoints)
        {
            joint.isKinematic = !ragdollEnabled;
        }
    }
}