using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wireframe : MonoBehaviour
{

    [SerializeField]
    private Material lineMaterial;
    private Vector3[] lines;
    private List<Vector3> linesList;
    private Color lineColor;
    private bool showWireframe = false;
    private Material[] originalMaterials;
    private Material[] transparentMaterials;

    // Use this for initialization
    void Start()
    {

        linesList = new List<Vector3>();
        lineColor = new Color(1.0f, 0.0f, 0.0f);
        originalMaterials = gameObject.GetComponent<MeshRenderer>().materials;
        List<Material> tMat = new List<Material>();

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            Material mat = originalMaterials[i];
            Material newMat = new Material(mat);
            newMat.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.01f));
            tMat.Add(newMat);
        }

        transparentMaterials = tMat.ToArray();

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i + 2 < triangles.Length; i += 3)
        {
            linesList.Add(vertices[triangles[i]]);
            linesList.Add(vertices[triangles[i + 1]]);
            linesList.Add(vertices[triangles[i + 2]]);
        }

        //optmization
        lines = linesList.ToArray();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            showWireframe = !showWireframe;
            if (showWireframe)
            {
                gameObject.GetComponent<MeshRenderer>().materials = transparentMaterials;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().materials = originalMaterials;
            }

        }
    }

    void OnRenderObject()
    {
        if (showWireframe)
        {
            //poner control de errores

            GL.PushMatrix();
            lineMaterial.SetPass(0);
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.LINES);
            GL.Color(lineColor);

            for (int i = 0; i + 2 < lines.Length; i += 3)
            {
                GL.Vertex(lines[i]);
                GL.Vertex(lines[i + 1]);
                GL.Vertex(lines[i + 2]);
            }

            GL.End();
            GL.PopMatrix();
        }

    }
}
