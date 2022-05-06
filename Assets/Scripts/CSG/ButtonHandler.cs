using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Parabox.CSG;



public class ButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// œÂ‰ÒÚ‡‚ÎÂÌËÂ ÔÎÓÒÍÓÒÚË(¬–≈Ã≈ÕÕŒ≈)
    /// Õ¿◊¿ÀŒ
    /// </summary>
    /// 

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                    / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    //Find the line of intersection between two planes.
    //The inputs are two game objects which represent the planes.
    //The outputs are a point on the line and a vector which indicates it's direction.
    void planePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, GameObject plane1, GameObject plane2)
    {

        linePoint = Vector3.zero;
        lineVec = Vector3.zero;

        //Get the normals of the planes.
        Vector3 plane1Normal = plane1.transform.up;
        Vector3 plane2Normal = plane2.transform.up;

        //We can get the direction of the line of intersection of the two planes by calculating the
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.      
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float numerator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero.
        if (Mathf.Abs(numerator) > 0.000001f)
        {

            Vector3 plane1ToPlane2 = plane1.transform.position - plane2.transform.position;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / numerator;
            linePoint = plane2.transform.position + t * ldir;
        }
    }

    Vector3 P1, P2;
    List<Vector3> pRes;
    ///////////////////////////////////////////////


    /// <summary>
    /// ÷≈ÕŒ 
    /// </summary>
    public GameObject Quadre1;
    public GameObject Quadre2;
    public GameObject Cylind;
    public GameObject Copystuff;
    public GameObject CuttingObj = new GameObject();


    public Vector3[] Vertices1, Vertices2, resVertices;
    public Vector2[] UV;
    public int[] Triangles1, Triangles2, resTriangles;

    public Mesh mesh1, mesh2, resMesh;

    private void Start()
    {

    }

    public void OnButtonPress()
    {
        mesh1 = Quadre1.GetComponent<MeshFilter>().mesh;
        mesh2 = Quadre2.GetComponent<MeshFilter>().mesh;

        Vertices1 = mesh1.vertices;
        Vertices2 = mesh2.vertices;

        Geometry.Quadrangle quadrangle1 = new Geometry.Quadrangle(Quadre1.transform.TransformPoint(Vertices1[0]), Quadre1.transform.TransformPoint(Vertices1[1]), Quadre1.transform.TransformPoint(Vertices1[2]), Quadre1.transform.TransformPoint(Vertices1[3]));
        Geometry.Quadrangle quadrangle2 = new Geometry.Quadrangle(Quadre2.transform.TransformPoint(Vertices2[0]), Quadre2.transform.TransformPoint(Vertices2[1]), Quadre2.transform.TransformPoint(Vertices2[3]), Quadre2.transform.TransformPoint(Vertices2[3]));

        Debug.Log(Quadre1.transform.TransformPoint(Vertices1[0]));
        Debug.Log(Quadre1.transform.TransformPoint(Vertices1[1]));
        Debug.Log(Quadre1.transform.TransformPoint(Vertices1[2]));
        Debug.Log(Quadre1.transform.TransformPoint(Vertices1[3]));

        Debug.Log("messssss");

        Debug.Log(Quadre2.transform.TransformPoint(Vertices2[0]));
        Debug.Log(Quadre2.transform.TransformPoint(Vertices2[1]));
        Debug.Log(Quadre2.transform.TransformPoint(Vertices2[2]));
        Debug.Log(Quadre2.transform.TransformPoint(Vertices2[3]));
        //Debug.Log(Vertices1[0]);

        P1 = new Vector3(); P2 = new Vector3();
        bool res1 = true, res2 = true;
        P1 = quadrangle1.Intersection(quadrangle2, true, ref res1);
        P2 = quadrangle1.Intersection(quadrangle2, false, ref res2);
        Debug.Log("PP"); Debug.Log(P1); Debug.Log(P2);

        //pRes = new Vector3[6];

        resVertices = new Vector3[6]
        {
            Quadre1.transform.TransformPoint(Vertices1[0]), Quadre1.transform.TransformPoint(Vertices1[1]), Quadre2.transform.TransformPoint(Vertices2[0]), Quadre2.transform.TransformPoint(Vertices2[2]), P1, P2
        };
        Debug.Log("PP AFTER");
        resMesh = new Mesh();
        resMesh.vertices = resVertices;
        Debug.Log("PP double after");
        resTriangles = new int[24]
        {
         /*   0, 1, 4,
            1, 4, 5,
            0, 4, 3,
            1, 5, 2,
            3, 4, 5,
            2, 3, 5,
            0, 1, 3,
            1, 3, 2*/
            0, 5, 3,
            3, 5, 4,
            4, 2, 3,
            3, 2, 1,
            1, 2, 4,
            4, 5, 0,
            0, 1, 4,
            1, 0, 3
        };
        Debug.Log("WHY");
        resMesh.triangles = resTriangles;

        var cutter = new GameObject();
        cutter.AddComponent<MeshFilter>().sharedMesh = resMesh;
        cutter.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        Debug.Log("not");
        CuttingObj = cutter;

        //Model result1 = CSG.Intersect(Quadre1, Quadre2);
        //var subtract = new GameObject();
        //subtract.AddComponent<MeshFilter>().sharedMesh = result1.mesh;
        //subtract.AddComponent<MeshRenderer>().sharedMaterials = result1.materials.ToArray();
        //subtract.AddComponent<Rigidbody>();
        //subtract.AddComponent<MeshCollider>().convex = true;

        //Debug.Log("fgshdjkfdhg");
        //Debug.Log(result1.mesh.vertices[0]);
        //Debug.Log(result1.mesh.vertices[result1.mesh.vertices[result1.mesh.vertices.]]);

        Vector3 position = Cylind.transform.position;//Cylind.transform.TransformPoint();


        //Model result1 = CSG.Intersect(cutter, Cylind);
        Model result1 = CSG.Subtract(Cylind, cutter);
        var subtract = new GameObject();
        subtract.AddComponent<MeshFilter>().sharedMesh = result1.mesh;
        subtract.AddComponent<MeshRenderer>().sharedMaterials = result1.materials.ToArray();
        //subtract.AddComponent<Rigidbody>();
        //subtract.AddComponent<MeshCollider>().convex = true;

        //Model result2 = CSG.Subtract(Cylind, subtract);
        Model result2 = CSG.Intersect(cutter, Cylind);
        var subtracted = new GameObject();
        subtracted.AddComponent<MeshFilter>().sharedMesh = result2.mesh;
        subtracted.AddComponent<MeshRenderer>().sharedMaterials = result2.materials.ToArray();
        subtracted.GetComponent<Transform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
        //subtracted.AddComponent<Rigidbody>();
        //subtracted.AddComponent<MeshCollider>().convex = true;

        Destroy(Cylind);
        //Destroy(subtract);
        //Cylind.GetComponent<MeshFilter>().sharedMesh = result1.mesh;
        //Cylind.GetComponent<MeshRenderer>().sharedMaterials = result1.materials.ToArray();

        /*Model result2 = CSG.Subtract(Cylind, subtract);
        var subtracted = new GameObject();
        subtracted.AddComponent<MeshFilter>().sharedMesh = result2.mesh;
        subtracted.AddComponent<MeshRenderer>().sharedMaterials = result2.materials.ToArray();
        subtracted.GetComponent<Transform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
        subtracted.AddComponent<Rigidbody>();
        subtracted.AddComponent<MeshCollider>().convex = true;
        */
        /*Model result3 = CSG.Subtract(Cylind, subtracted);
        var subtractedd = new GameObject();
        subtractedd.AddComponent<MeshFilter>().sharedMesh = result3.mesh;
        subtractedd.AddComponent<MeshRenderer>().sharedMaterials = result3.materials.ToArray();
        subtractedd.GetComponent<Transform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
        subtractedd.AddComponent<Rigidbody>();
        subtractedd.AddComponent<MeshCollider>().convex = true;*/



        Destroy(cutter);
        //Destroy(Cylind);

        Vector3 vec1 = new Vector3(-2.12384f, 4.27509f, -6.59436f);
        Vector3 vec2 = new Vector3(-3.08392f, 4.10423f, -6.39592f);

        planePlaneIntersection(out vec1, out vec2, Quadre1, Quadre2);

        Vector3 direct1 = vec2 - vec1;
        Vector3 direct2 = Quadre1.transform.TransformPoint(Vertices1[0]) - Quadre1.transform.TransformPoint(Vertices1[1]);
        //Debug.Log(direct.normalized);
        //Quadre1.GetComponent<Collider>().Raycast(new Ray(vec1, direct.normalized), out RaycastHit hit1, 100.0f);
        Debug.Log("agfdsdsgsgfdsgsdgsdgs");

        Debug.Log(position);
        subtracted.transform.position = position;

        //Debug.Log(hit1.point);
        Vector3 intersection;
        //bool a=LineLineIntersection(out intersection, Quadre1.transform.TransformPoint(Vertices1[1]), direct2, vec1, vec2);
        //Debug.Log(intersection);
        Debug.Log(vec1);
        Debug.Log(vec2);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(P1, P2);
        //Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(10, 5, 23));
        //Gizmos.DrawCube(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
    }
}

