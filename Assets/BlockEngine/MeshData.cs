using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData
{
    public Vector3[] colliderArray;
    public int[] colliderTriangles;
    public Vector3[] verticesArray;
    public int[] trianglesArray;
    public Vector2[] uvArray;

    public List<Vector3> collider = new List<Vector3>();
    public List<int> collTri = new List<int>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public MeshData() { }

    public void AddQuadTriangles()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }

    public void AddQuadColliderTriangles()
    {
        collTri.Add(collider.Count - 4);
        collTri.Add(collider.Count - 3);
        collTri.Add(collider.Count - 2);

        collTri.Add(collider.Count - 4);
        collTri.Add(collider.Count - 2);
        collTri.Add(collider.Count - 1);
    }
}