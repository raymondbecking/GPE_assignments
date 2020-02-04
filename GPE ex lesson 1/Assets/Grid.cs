using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [SerializeField]
    int xSize;
    [SerializeField]
    int ySize;

    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start () {
        Generate();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void Generate()
    {
        //Generate each gizmo with a delay (coroutined)
        WaitForSeconds wait = new WaitForSeconds(0.04f);

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        //Generate vector filled with grid dots for the 
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        //Create UV coordinates for the grid
        Vector2[] uv = new Vector2[vertices.Length];
        //For all dots on the origin move the x and y so they dont overlap
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for(int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;

        int[] tris = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                tris[ti + 0] = vi;
                tris[ti + 3] = tris[ti + 2] = vi + 1;
                tris[ti + 4] = tris[ti + 1] = vi + xSize + 1;
                tris[ti + 5] = vi + xSize + 2;
                mesh.triangles = tris;
            }
        }
        mesh.triangles = tris;
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos()
    {
        //Only draw when there are vertices (play mode)
        if(vertices == null)
        {
            return;
        }

        //Create gizmo for each vertice
        Gizmos.color = Color.black;
        for(int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
