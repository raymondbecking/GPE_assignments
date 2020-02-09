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

    public float springForce = 10f;
    public float damping = 5f;
    float uniformScale = 1f;

    float amplitude = 0.125f;
    float freq = 8;
    const float PI = 3.14159f;

    private Vector3 hitLocation;

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

        //Update displaced vertices
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement *= uniformScale;
        //Velocity change to cause sinus waves
        velocity -= displacement * (amplitude * Mathf.Sin(PI * freq * displacement.magnitude * Time.deltaTime));
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        //if (this.transform.Find("GameObject") != null && this.name == "Cube")
        //{
        //    Debug.DrawLine(this.transform.Find("GameObject").transform.position, point, Color.green);
        //}
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    //This function is used to give the vertices a velocity based on the amount of force applied
    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        vertexVelocities[i] -= pointToVertex.normalized * velocity;

    }


}