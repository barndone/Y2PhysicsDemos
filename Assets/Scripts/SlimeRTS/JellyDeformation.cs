using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDeformation : MonoBehaviour
{
    //  jiggle speed
    public float bounceSpeed;
    //  emulates mass
    public float fallForce;
    //  strength of the surface
    public float stiffness;

    private MeshFilter meshFilter;
    private Mesh mesh;

    JellyVertex[] jellyVertices;
    Vector3[] currentMeshVertices;

    [SerializeField] float maxDisplacement = 1.0f;


    private void Start()
    {
        if (TryGetComponent<MeshFilter>(out meshFilter))
        {
            //  we must clean this up on destroy
            mesh = meshFilter.mesh;
            //  get the mesh for this slime
            GetVertices();
        }
        else
        {
            Debug.LogWarning("No MeshFilter attached to " + gameObject.name+ ". Removing JellyDeformation component.", this);
            Destroy(gameObject.GetComponent<JellyDeformation>());
        }
    }

    private void OnDestroy()
    {
        //  cleanup mesh to avoid memory leak
        Destroy(mesh);
    }

    private void GetVertices()
    {
        jellyVertices = new JellyVertex[mesh.vertices.Length];
        currentMeshVertices = new Vector3[mesh.vertices.Length];

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            jellyVertices[i] = new JellyVertex(i, mesh.vertices[i], mesh.vertices[i], Vector3.zero);
            currentMeshVertices[i] = mesh.vertices[i];
        }
    }

    private void Update()
    {
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        for (int i = 0; i < jellyVertices.Length; i++)
        {
            jellyVertices[i].UpdateVelocity(bounceSpeed);
            jellyVertices[i].Settle(stiffness);

            

            if (jellyVertices[i].currentVelocity.magnitude <= maxDisplacement)
            {
                jellyVertices[i].currentVertPos += jellyVertices[i].currentVelocity * Time.deltaTime;
            }
            else
            {
                var curDisplacement = jellyVertices[i].currentVelocity.magnitude;
                var factor = maxDisplacement / curDisplacement;

                jellyVertices[i].currentVelocity *= factor;
            }

            currentMeshVertices[i] = jellyVertices[i].currentVertPos;
        }

        mesh.vertices = currentMeshVertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void OnCollisionEnter(Collision collision)
    {
        var contactCount = collision.contactCount;
        ContactPoint[] collisionPoints = new ContactPoint[contactCount];
        collision.GetContacts(collisionPoints);

        for (int i = 0; i < collisionPoints.Length; i++)
        {
            Vector3 inputPoint = collisionPoints[i].point + (collisionPoints[i].point * .1f);
            ApplyPressureToPoint(inputPoint, fallForce);
        }
    }

    public void ApplyPressureToPoint(Vector3 _point, float _pressure)
    {
        for (int i = 0; i < jellyVertices.Length; i++)
        {
            jellyVertices[i].ApplyPressureToVertex(transform, _point, _pressure);
        }
    }
}

public class JellyVertex
{
    public int vertIndex;
    public Vector3 initialVertPos;
    public Vector3 currentVertPos;

    public Vector3 currentVelocity;

    public JellyVertex(int _vertIndex, Vector3 _initialVertPos, Vector3 _currentVertPos, Vector3 _currentVelocity)
    {
        vertIndex = _vertIndex;
        initialVertPos = _initialVertPos;
        currentVertPos = _currentVertPos;
        currentVelocity = _currentVelocity;
    }

    //  return the displacement of a given vert
    public Vector3 GetCurrentDisplacement()
    {
        return currentVertPos - initialVertPos;
    }

    public void UpdateVelocity(float _bounceSpeed)
    {
        currentVelocity = currentVelocity - GetCurrentDisplacement() * _bounceSpeed * Time.deltaTime;
    }


    public void Settle(float _stiffness)
    {
        currentVelocity *= 1f - _stiffness * Time.deltaTime;
    }

    public void ApplyPressureToVertex(Transform _transform, Vector3 _position, float _pressure)
    {
        Vector3 distanceVertPoint = currentVertPos - _transform.InverseTransformPoint(_position);
        float adaptedPressure = _pressure / (1f + distanceVertPoint.sqrMagnitude);
        float velocity = adaptedPressure * Time.deltaTime;
        currentVelocity += distanceVertPoint.normalized * velocity;
    }
}

