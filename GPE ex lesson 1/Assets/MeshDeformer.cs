using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{

    Mesh deformingMesh;
    //Keep track of the original vertice positions
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    //Spring deform
    public float springForce = 10f;
    public float damping = 5f;
    float uniformScale = 1f;

    //Ripple deform
    public float amplitude = 0.125f;
    public float frequency = 4f;
    const float PI = 3.14159f;

    // Use this for initialization
    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        //Save original vertices
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        //Vector to save the velocity of each vertex
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    // Update is called once per frame
    void Update()
    {
        uniformScale = transform.localScale.x;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateVertex(int i)
    {
        //Save current velocity
        Vector3 velocity = vertexVelocities[i];
        //Calculate displacement based on vertice origin
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        //Ensure displacement is scaled based on locals
        displacement *= uniformScale;


        //velocity -= displacement * springForce * Time.deltaTime;
        //velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }



    //Add force on raycasted point
    public void AddDeformingRipple(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        Debug.DrawLine(Camera.main.transform.position, point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            //Add force for each vertex (spring deformation)
            AddRippleToVertex(i, point);
        }
    }

    void AddRippleToVertex(int i, Vector3 point)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float velocity = amplitude * Mathf.Sin(-PI * frequency + Time.deltaTime);
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    //Add force on raycasted point
    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        Debug.DrawLine(Camera.main.transform.position, point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {            
            //Add force for each vertex (spring deformation)
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
}
