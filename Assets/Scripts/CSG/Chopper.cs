using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
//using Valve.VR.InteractionSystem;

public class Chopper : MonoBehaviour
{
    public GameObject tree;
    public GameObject wound1, wound2;

    static float treeDensity = 0.68f;

    class Wound
    {
        public GameObject wound;
        Mesh mesh;
        public Geometry.Quadrangle quad;

        public Wound(GameObject g)//wound должна быть представлена как GameObject с мэшом и такими ос€ми, чтобы вектор up перпендикул€рен плоскости, которую отражает мэш
        {
            g.transform.position = new Vector3((float)System.Math.Round(g.transform.position.x, 2), (float)System.Math.Round(g.transform.position.y, 2), (float)System.Math.Round(g.transform.position.z, 2));
            wound = g;
            mesh = g.GetComponent<MeshCollider>().sharedMesh;
            quad = new Geometry.Quadrangle(g.transform.TransformPoint(mesh.vertices[0]), g.transform.TransformPoint(mesh.vertices[1]), g.transform.TransformPoint(mesh.vertices[2]), g.transform.TransformPoint(mesh.vertices[3]));
            /*Debug.Log(g.transform.TransformPoint(mesh.vertices[0]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[1]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[2]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[3]));
            Debug.Log("......s");*/
        }
    }

    class Chopable
    {
        public GameObject tree;
        List<Wound> wounds = new List<Wound>();
        public Chopable(GameObject t)
        {
            tree = t;
        }
        public void GetNewWound(GameObject g)
        {
            Wound newWound = new Wound(g);



            foreach (Wound wound in wounds)
            {
                bool intsc = false;
                Vector3 intscPoint1 = wound.quad.Intersection(newWound.quad, true, ref intsc);
                if (intsc)
                {

                    Vector3 intscPoint2 = wound.quad.Intersection(newWound.quad, false, ref intsc);
                    if (intsc) Cut(newWound, wound, intscPoint1, intscPoint2);
                }
                else
                {
                    intscPoint1 = newWound.quad.Intersection(wound.quad, true, ref intsc);
                    if (intsc)
                    {

                        Vector3 intscPoint2 = newWound.quad.Intersection(wound.quad, false, ref intsc);
                        if (intsc) Cut(newWound, wound, intscPoint1, intscPoint2);
                    }
                }
            }
            wounds.Add(newWound);
        }

        void Cut(Wound wound1, Wound wound2, Vector3 intscPoint1, Vector3 intscPoint2)
        {

            if (Vector3.Distance(wound1.quad.points[0], intscPoint1) < Vector3.Distance(wound1.quad.points[0], intscPoint2))
            {
                Vector3 temp = intscPoint1;
                intscPoint1 = intscPoint2;
                intscPoint2 = temp;
            }

            Vector3 middle = (intscPoint1 + intscPoint2) / 2.0f;
            Vector3[] resVertices = new Vector3[6]
        {
            wound1.quad.points[0], wound1.quad.points[1], wound2.quad.points[0], wound2.quad.points[1], intscPoint1, intscPoint2
        };
            Mesh resMesh = new Mesh();
            resMesh.vertices = resVertices;
            int[] trianglesIndex = new int[6]
            {
                0,1,2,3,4,5
            };
            int[] resTriangles;
            if (wound1.quad.points[0].y < wound2.quad.points[0].y)
            {
                resTriangles = new int[24]
                {
                    trianglesIndex[5],trianglesIndex[2],trianglesIndex[0],
                    trianglesIndex[2],trianglesIndex[3],trianglesIndex[0],
                    trianglesIndex[3],trianglesIndex[1],trianglesIndex[0],
                    trianglesIndex[3],trianglesIndex[4],trianglesIndex[1],
                    trianglesIndex[2],trianglesIndex[5],trianglesIndex[3],
                    trianglesIndex[5],trianglesIndex[4],trianglesIndex[3],
                    trianglesIndex[0],trianglesIndex[1],trianglesIndex[4],
                    trianglesIndex[0],trianglesIndex[4],trianglesIndex[5]
                };
            }
            else resTriangles = new int[24]
                {
                    trianglesIndex[5],trianglesIndex[0],trianglesIndex[2],
                    trianglesIndex[0],trianglesIndex[1],trianglesIndex[2],
                    trianglesIndex[1],trianglesIndex[3],trianglesIndex[2],
                    trianglesIndex[1],trianglesIndex[4],trianglesIndex[3],
                    trianglesIndex[0],trianglesIndex[5],trianglesIndex[1],
                    trianglesIndex[5],trianglesIndex[4],trianglesIndex[1],
                    trianglesIndex[2],trianglesIndex[3],trianglesIndex[4],
                    trianglesIndex[2],trianglesIndex[4],trianglesIndex[5]
                };
            resMesh.triangles = resTriangles;

            var cutter = new GameObject("cutter");
            cutter.AddComponent<MeshFilter>().mesh = resMesh;
            cutter.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

            ///////////////////StartCutSuper
            GameObject TreeWithCut =
                Perform(
                    CSG.BooleanOp.Subtraction,
                    tree.gameObject,
                    cutter,
                    "TreeWithCut"
          );

            GameObject SubtractedPieceWood =
                Perform(
                    CSG.BooleanOp.Intersection,
                    tree.gameObject,
                    cutter,
                    "SubtractedPieceWood"
                );

            ObjectReposition(middle, SubtractedPieceWood);
            SubtractedPieceWood.GetComponent<Transform>().localScale = new Vector3(0.99f, 0.99f, 0.99f);
            SubtractedPieceWood.AddComponent<Rigidbody>();
            SubtractedPieceWood.AddComponent<MeshCollider>().convex = true;

            Vector3 rhsPos = wound1.wound.transform.up;
            Vector3 lhsPos = wound2.wound.transform.up;

            Vector3 V = (intscPoint1 + intscPoint2) / 2.0f;
            var kutter = new GameObject("kkk");
            kutter.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            kutter.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            kutter.transform.position = V;
            kutter.transform.localScale = new Vector3(4f, 4f, 1f);

            kutter.transform.right = Vector3.Cross(rhsPos, lhsPos);
            kutter.transform.forward = (intscPoint1 - V).normalized;
            kutter.transform.Rotate(Vector3.right, +90);

            GameObject UpperNotSliced =
                Perform(
                    CSG.BooleanOp.Subtraction,
                    tree,
                    kutter,
                    "UpperNotSliced"
                );

            kutter.transform.Rotate(Vector3.right, -180);

            GameObject LowerNotSliced =
                Perform(
                    CSG.BooleanOp.Subtraction,
                    tree,
                    kutter,
                    "LowerNotSliced"
                );

            GameObject UpperHalfTree =
                Perform(
                    CSG.BooleanOp.Subtraction,
                    UpperNotSliced.gameObject,
                    cutter.gameObject,
                    "UpperHalfTree"
                );

            ObjectReposition(kutter.transform.position, UpperHalfTree);

            GameObject LowerHalfTree =
                Perform(
                    CSG.BooleanOp.Subtraction,
                    LowerNotSliced.gameObject,
                    cutter.gameObject,
                    "LowerHalfTree"
                );

            ObjectReposition(kutter.transform.position, LowerHalfTree);

            var cube = new GameObject("smolCube");
            cube.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            cube.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            cube.transform.position = V;
            cube.transform.localScale = new Vector3(5f, 0.00001f, 5f);

            cube.transform.right = Vector3.Cross(rhsPos, lhsPos);
            cube.transform.forward = (intscPoint1 - V).normalized;

            Model result2 = CSG.Intersect(tree, cube);

            var sqrtThing = new GameObject("PeaceOfTree");
            sqrtThing.AddComponent<MeshFilter>().mesh = result2.mesh;
            sqrtThing.AddComponent<MeshRenderer>().materials = result2.materials.ToArray();

            ObjectReposition(cube.transform.position, sqrtThing);

            LowerHalfTree.AddComponent<Rigidbody>();
            LowerHalfTree.AddComponent<MeshCollider>().convex = true;
            UpperHalfTree.AddComponent<Rigidbody>();
            UpperHalfTree.AddComponent<MeshCollider>().convex = true;

            float volumeUpper = BreakingPhysics.VolumeOfMesh(UpperHalfTree);
            float volumeLower = BreakingPhysics.VolumeOfMesh(LowerHalfTree);
            UpperHalfTree.GetComponent<Rigidbody>().mass = volumeUpper * treeDensity;
            LowerHalfTree.GetComponent<Rigidbody>().mass = volumeLower * treeDensity;

            float sqr = BreakingPhysics.Area(sqrtThing);
            BreakingPhysics.setComponentFixedJoint(UpperHalfTree, LowerHalfTree, sqr * 300);

            Destroy(TreeWithCut.gameObject);
            Destroy(UpperNotSliced.gameObject);
            Destroy(LowerNotSliced.gameObject);
            Destroy(cutter);
            Destroy(kutter);
            Destroy(cube);
            Destroy(sqrtThing);
            Destroy(tree);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Chopable choppable = new Chopable(tree);
        choppable.GetNewWound(wound1);
        choppable.GetNewWound(wound2);
        Debug.Log("nice");
        Debug.Log("nice");
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var verticle in tree.GetComponent<MeshFilter>().mesh.vertices)
        {
            Gizmos.DrawCube(tree.transform.TransformPoint(verticle), new Vector3(0.05f, 0.05f, 0.05f));
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    static Vector3 FindCenter(Vector3[] poly)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v3 in poly)
        {
            center += v3;
        }
        return center / poly.Length;
    }
    static GameObject Perform(CSG.BooleanOp booleanOp, GameObject lhs, GameObject rhs, string name)
    {

        Model result = CSG.Perform(booleanOp, lhs, rhs);
        var materials = result.materials.ToArray();
        GameObject pb = new GameObject(name);
        pb.AddComponent<MeshFilter>().sharedMesh = (Mesh)result;
        pb.AddComponent<MeshRenderer>().sharedMaterials = materials;
        pb.AddComponent<Chopper>().tree = pb;
        return pb;
    }

    static void ObjectReposition(Vector3 fix, GameObject broken)
    {
        Vector3[] v = broken.GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < broken.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
            v[i] -= fix;


        broken.GetComponent<MeshFilter>().mesh.vertices = v;
        broken.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        broken.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        broken.transform.position += fix;
    }
}