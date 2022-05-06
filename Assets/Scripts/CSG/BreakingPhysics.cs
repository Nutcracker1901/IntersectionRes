using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPhysics : MonoBehaviour
{


    /// <summary>
    /// Функция подсчета площади поверхности у GameObject'а. Используется для нахождения площади у вырезаной части дерева.
    /// </summary>
    /// <param name="gameObject">GameObject, у которого нужно найти площадь.</param>
    /// <returns></returns>
    public static float Area(GameObject gameObject)
    {

        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        var triangles = mesh.triangles;
        var vertices = mesh.vertices;

        double sum = 0.0;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }

        Debug.Log((float)(sum / 2.0));
        return (float)(sum / 2.0);
    }

    /*void DelayedCall(GameObject lhs, GameObject rhs, GameObject Output)
    {
        Model modelR = CSG.Intersect(lhs, rhs);
        Output.GetComponent<MeshFilter>().mesh = modelR.mesh;
        Output.GetComponent<MeshRenderer>().materials = modelR.materials.ToArray();
    }*/

    public static void setComponentFixedJoint(GameObject first, GameObject second, float breakForce)
    {
        if (!first.GetComponent<FixedJoint>())
        {
            FixedJoint joint = first.AddComponent<FixedJoint>();
            joint.connectedBody = second.GetComponent<Rigidbody>();
            joint.breakForce = breakForce;
        }
    }

    /// <summary>
    /// Функция подсчета объема Triangl'а, используется в качестве обслуживающей. 
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    /// <summary>
    /// Функция подсчета объекта GameObject'а.
    /// </summary>
    /// <param name="gameObject">GameObject, у которого нужно найти объем.</param>
    /// <returns></returns>
    public static float VolumeOfMesh(GameObject gameObject)
    {
        float volume = 0;

        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);

        }
        return Mathf.Abs(volume) * gameObject.transform.localScale.x * gameObject.transform.localScale.y * gameObject.transform.localScale.z;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
