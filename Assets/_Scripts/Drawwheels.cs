using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Drawwheels : MonoBehaviour
{
    public GameObject obj;
    GameObject drawing;
    public MeshCollider drawArea;
    Camera cam;
    public Material mat;
    bool hasDrawingStarted;

    private bool IsCursorInDrawArea
    {
        get
        {
            return drawArea.bounds.Contains(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 11)));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerInput>().camera;
    }

    public void StartDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (!IsCursorInDrawArea)
            return;
        StartCoroutine(Draw());
    }

    private IEnumerator Draw()
    {
        hasDrawingStarted = true;
        drawing = new GameObject("Drawing");

        drawing.transform.localScale = new Vector3(1, 1, 0);

        drawing.AddComponent<MeshFilter>();
        drawing.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>(new Vector3[8]);

        Vector3 startPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        Vector3 temp = new Vector3(startPosition.x, startPosition.y, 0.5f);

        for(int i=0; i<vertices.Count; i++)
        {
            vertices[i] = temp;
        }

        List<int> triangles = new List<int>(new int[36]);

        //FrontFace
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        //TopFace
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 2;
        triangles[10] = 4;
        triangles[11] = 5;

        //RightFace
        triangles[12] = 1;
        triangles[13] = 2;
        triangles[14] = 5;
        triangles[15] = 1;
        triangles[16] = 5;
        triangles[17] = 6;

        //LeftFace
        triangles[18] = 0;
        triangles[19] = 7;
        triangles[20] = 4;
        triangles[21] = 0;
        triangles[22] = 4;
        triangles[23] = 3;

        //BackFace
        triangles[24] = 5;
        triangles[25] = 4;
        triangles[26] = 7;
        triangles[27] = 5;
        triangles[28] = 4;
        triangles[29] = 6;

        //BottomFace
        triangles[30] = 0;
        triangles[31] = 6;
        triangles[32] = 7;
        triangles[33] = 0;
        triangles[34] = 1;
        triangles[35] = 6;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        drawing.GetComponent<MeshFilter>().mesh = mesh;
        drawing.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        drawing.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);

        Vector3 lastMousePosition = startPosition;

        while (IsCursorInDrawArea)
        {
            float minDistance = 0.1f;

            float distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), lastMousePosition);

            while(distance < minDistance)
            {
                distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), lastMousePosition);
                yield return null;
            }
            vertices.AddRange(new Vector3[4]);
            triangles.AddRange(new int[30]);

            int vindex = vertices.Count - 8;
            int vindex0 = vindex + 3;
            int vindex1 = vindex + 2;
            int vindex2 = vindex + 1;
            int vindex3 = vindex + 0;

            int vindex4 = vindex + 4;
            int vindex5 = vindex + 5;
            int vindex6 = vindex + 6;
            int vindex7 = vindex + 7;

            Vector3 currentMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Vector3 mouseForwardvector = (currentMousePosition - lastMousePosition).normalized;

            float linethickness = 0.25f;

            Vector3 topRightVertex = currentMousePosition + Vector3.Cross(mouseForwardvector, Vector3.back) * 0.25f;
            Vector3 bottomRightVertex = currentMousePosition + Vector3.Cross(mouseForwardvector, Vector3.forward) * 0.25f;
            Vector3 topLeftVertex = new Vector3(topRightVertex.x, topRightVertex.y, 1);
            Vector3 bottomLeftVertex = new Vector3(bottomRightVertex.x, bottomRightVertex.y, 1);

            vertices[vindex4] = topLeftVertex;
            vertices[vindex5] = topRightVertex;
            vertices[vindex6] = bottomRightVertex;
            vertices[vindex7] = bottomLeftVertex;

            int tIndex = triangles.Count - 30;

            //topface;
            triangles[tIndex + 0] = vindex2;
            triangles[tIndex + 1] = vindex3;
            triangles[tIndex + 2] = vindex4;
            triangles[tIndex + 3] = vindex2;
            triangles[tIndex + 4] = vindex4;
            triangles[tIndex + 5] = vindex5;

            //rightface;
            triangles[tIndex + 6] = vindex1;
            triangles[tIndex + 7] = vindex2;
            triangles[tIndex + 8] = vindex5;
            triangles[tIndex + 9] = vindex1;
            triangles[tIndex + 10] = vindex5;
            triangles[tIndex + 11] = vindex6;

            //leftface;
            triangles[tIndex + 12] = vindex0;
            triangles[tIndex + 13] = vindex7;
            triangles[tIndex + 14] = vindex4;
            triangles[tIndex + 15] = vindex0;
            triangles[tIndex + 16] = vindex4;
            triangles[tIndex + 17] = vindex3;

            //backface;
            triangles[tIndex + 18] = vindex5;
            triangles[tIndex + 19] = vindex4;
            triangles[tIndex + 20] = vindex7;
            triangles[tIndex + 21] = vindex0;
            triangles[tIndex + 22] = vindex4;
            triangles[tIndex + 23] = vindex3;

            //bottomface;
            triangles[tIndex + 24] = vindex0;
            triangles[tIndex + 25] = vindex6;
            triangles[tIndex + 26] = vindex7;
            triangles[tIndex + 27] = vindex0;
            triangles[tIndex + 28] = vindex1;
            triangles[tIndex + 29] = vindex6;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            lastMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

            yield return null;
        }
    }

    public void EndDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (!hasDrawingStarted)
            return;

        hasDrawingStarted = false;
        StopAllCoroutines();
        Redraw();
        CalculateNormals();

        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;

        Mesh objMesh = new Mesh();
        objMesh.vertices = mesh.vertices;
        objMesh.triangles = mesh.triangles;
        objMesh.normals = mesh.normals;

        obj.GetComponent<MeshFilter>().mesh = objMesh;

        obj.GetComponent<MeshFilter>().sharedMesh = objMesh;

        /*obj.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        obj.GetComponent<Renderer>().material.color = new Color(0.1f, 0.2f, 0.3f);*/

        obj.GetComponent<Renderer>().material = mat;
        Destroy(drawing);
    }

    private void Redraw()
    {
        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for(int i=1; i<vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices[i].x + (vertices[0].x * -1),
                vertices[i].y + (vertices[0].y * -1),
                vertices[i].z + (vertices[0].z * -1));
        }
        vertices[0] = Vector3.zero;
        mesh.vertices = vertices;

    }
    private void CalculateNormals()
    {
        new MeshImporter(drawing).Import();
        ProBuilderMesh proMesh = drawing.GetComponent<ProBuilderMesh>();

        Normals.CalculateNormals(proMesh);
        proMesh.ToMesh();
        proMesh.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
