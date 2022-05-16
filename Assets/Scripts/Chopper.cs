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
            Debug.Log(g.transform.TransformPoint(mesh.vertices[0]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[1]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[2]));
            Debug.Log(g.transform.TransformPoint(mesh.vertices[3]));
            Debug.Log("......s");
        }
    }

    class Chopable
    {
        public GameObject tree;
        List<Wound> wounds = new List<Wound>();
        public Chopable(GameObject t)
        {
            tree = t;
            if (tree.GetComponent<MeshFilter>().sharedMesh == null) throw new System.Exception("gburh no shared mesh");
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

            Vector3 middle = (intscPoint1 + intscPoint2) / 2;
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

            Model result1 = CSG.Subtract(tree, cutter);

            Model result2 = CSG.Intersect(cutter, tree);

            tree.GetComponent<MeshFilter>().mesh = result1.mesh;
            tree.GetComponent<MeshRenderer>().materials = result1.materials.ToArray();
            tree.GetComponent<Transform>().localScale = Vector3.one;

            Vector3[] v = tree.GetComponent<MeshFilter>().mesh.vertices;
            for (int i = 0; i < tree.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
                v[i] -= tree.transform.position;


            tree.GetComponent<MeshFilter>().mesh.vertices = v;
            tree.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            tree.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            var subtracted = new GameObject("PeaceOfWood");
            subtracted.AddComponent<MeshFilter>().mesh = result2.mesh;
            subtracted.AddComponent<MeshRenderer>().materials = result2.materials.ToArray();

            v = subtracted.GetComponent<MeshFilter>().mesh.vertices;
            for (int i = 0; i < subtracted.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
                v[i] -= middle;//tree.transform.position;

            subtracted.GetComponent<MeshFilter>().mesh.vertices = v;
            subtracted.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            subtracted.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            subtracted.transform.position += middle;//tree.transform.position;

            subtracted.GetComponent<Transform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
            //subtracted.AddComponent<Rigidbody>();
            subtracted.AddComponent<MeshCollider>().convex = true;
            subtracted.AddComponent<Chopper>().tree = subtracted;

            //subtracted.AddComponent<Throwable>();

            //Destroy(cutter);
            
            PostCut(wound1, wound2, intscPoint1, intscPoint2);
        }

        void PostCut(Wound wound1, Wound wound2, Vector3 intscPoint1, Vector3 intscPoint2)
        {
            Vector3 rhsPos = wound1.wound.transform.up;
            Vector3 lhsPos = wound2.wound.transform.up;

            Vector3 V = (intscPoint1 + intscPoint2)/2.0f;
            var kutter = new GameObject("kkk");
            kutter.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            kutter.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            kutter.transform.position = V;
            kutter.transform.localScale = new Vector3(4f, 4f, 1f);
            
            kutter.transform.right = Vector3.Cross(rhsPos, lhsPos);
            kutter.transform.forward = (intscPoint1 - V).normalized;
            kutter.transform.Rotate(Vector3.right, -90);

            var cube = new GameObject("smolCube");
            cube.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            cube.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
            cube.transform.position = V;
            cube.transform.localScale = new Vector3(5f, 0.00001f, 5f);

            cube.transform.right = Vector3.Cross(rhsPos, lhsPos);
            cube.transform.forward = (intscPoint1 - V).normalized;

            Model result1 = CSG.Subtract(tree, kutter);
            Model result2 = CSG.Intersect(tree, cube);

            var sqrtThing = new GameObject("PeaceOfTree");
            sqrtThing.AddComponent<MeshFilter>().mesh = result2.mesh;
            sqrtThing.AddComponent<MeshRenderer>().materials = result2.materials.ToArray();
            
            Vector3[] v = sqrtThing.GetComponent<MeshFilter>().mesh.vertices;
            for (int i = 0; i < sqrtThing.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
                v[i] -= cube.transform.position;

            sqrtThing.GetComponent<MeshFilter>().mesh.vertices = v;
            sqrtThing.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            sqrtThing.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            sqrtThing.transform.position = cube.transform.position;
            Destroy(cube);

            if (sqrtThing.GetComponent<MeshFilter>().mesh.subMeshCount == 2)
            {
                Destroy(kutter);
                Destroy(sqrtThing);
                return;
            }

            var lowerhalfTree = new GameObject("lowerhalfOfTree");
            lowerhalfTree.AddComponent<MeshFilter>().mesh = result1.mesh;
            lowerhalfTree.AddComponent<MeshRenderer>().materials = result1.materials.ToArray();

            v = lowerhalfTree.GetComponent<MeshFilter>().mesh.vertices;
            for (int i = 0; i < lowerhalfTree.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
                v[i] -= kutter.transform.position;

            lowerhalfTree.GetComponent<MeshFilter>().mesh.vertices = v;
            lowerhalfTree.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            lowerhalfTree.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            lowerhalfTree.transform.position = kutter.transform.position;

            result1 = CSG.Subtract(tree, lowerhalfTree);


            Destroy(tree);
            Destroy(kutter);

            var upperhalfTree = new GameObject("upperhalfOfTree");
            upperhalfTree.AddComponent<MeshFilter>().mesh = result1.mesh;
            upperhalfTree.AddComponent<MeshRenderer>().materials = result1.materials.ToArray();

            v = upperhalfTree.GetComponent<MeshFilter>().mesh.vertices;
            for (int i = 0; i < upperhalfTree.GetComponent<MeshFilter>().mesh.vertices.Length; i++)
                v[i] -= lowerhalfTree.transform.position;

            upperhalfTree.GetComponent<MeshFilter>().mesh.vertices = v;
            upperhalfTree.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            upperhalfTree.GetComponent<MeshFilter>().mesh.RecalculateNormals();

            upperhalfTree.transform.position = lowerhalfTree.transform.position;
            
            tree.transform.position = Vector3.zero;

            //upperhalfTree.AddComponent<Rigidbody>();
            upperhalfTree.AddComponent<MeshCollider>().convex = true;
            //lowerhalfTree.AddComponent<Rigidbody>();
            lowerhalfTree.AddComponent<MeshCollider>().convex = true;

            //float volumeUpper = BreakingPhysics.VolumeOfMesh(upperhalfTree);
            //float volumeLower = BreakingPhysics.VolumeOfMesh(lowerhalfTree);
            //upperhalfTree.GetComponent<Rigidbody>().mass = volumeUpper * treeDensity;
            //lowerhalfTree.GetComponent<Rigidbody>().mass = volumeLower * treeDensity;

            //float sqr = BreakingPhysics.Area(sqrtThing);
            //Destroy(sqrtThing);
            //BreakingPhysics.setComponentFixedJoint(upperhalfTree, lowerhalfTree, sqr*300);

            lowerhalfTree.AddComponent<Chopper>().tree = lowerhalfTree;
            upperhalfTree.AddComponent<Chopper>().tree = upperhalfTree;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //startTest();
    }

    public void startTest()
    {
        Chopable choppable = new Chopable(tree);
        choppable.GetNewWound(wound1);
        choppable.GetNewWound(wound2);
        //Debug.Log("nice");
        //Debug.Log("nice");
    }

    private void OnDrawGizmosSelected()
    {
        foreach(var verticle in tree.GetComponent<MeshFilter>().sharedMesh.vertices)
        {
            Gizmos.DrawCube(tree.transform.TransformPoint(verticle), new Vector3(0.05f, 0.05f, 0.05f));
        }
    }
    // Update is called once per frame
    void Update()
    {
 
    }

}
