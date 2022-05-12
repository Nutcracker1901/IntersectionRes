using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{

    public class Quadrangle
    {
        public Vector3[] points;
        public Vector3[] plane;
        Vector3 minmaxX, minmaxY, minmaxZ;

        int i, j, k;

        public Vector3 calc = new Vector3();
        Vector3[] calcVariable = new Vector3[4];

        Vector3 res = new Vector3(0.0001f, 0, 0);

        
        public Quadrangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            points = new Vector3[4]; points[0] = p1; points[1] = p2; points[2] = p3; points[3] = p4;

            minmaxX.x = p1.x; minmaxX.y = p1.x;
            minmaxY.x = p1.y; minmaxY.y = p1.y;
            minmaxZ.x = p1.z; minmaxZ.y = p1.z;
            //////minmaxX.x = min x,    minmaxX.y = max x, minmaxX.z = расстояние 
            foreach (Vector3 p in points)
            {
                minmaxX.x = minmaxX.x > p.x ? p.x : minmaxX.x;
                minmaxX.y = minmaxX.y < p.x ? p.x : minmaxX.y;

                minmaxX.z = Mathf.Abs(minmaxX.y - minmaxX.x);

                minmaxY.x = minmaxY.x > p.y ? p.y : minmaxY.x;
                minmaxY.y = minmaxY.y < p.y ? p.y : minmaxY.y;

                minmaxY.z = Mathf.Abs(minmaxY.y - minmaxY.x);

                minmaxZ.x = minmaxZ.x > p.z ? p.z : minmaxZ.x;
                minmaxZ.y = minmaxZ.y < p.z ? p.z : minmaxZ.y;

                minmaxZ.z = Mathf.Abs(minmaxZ.y - minmaxZ.x);
            }

            if ((minmaxX.z>=minmaxY.z)&&(minmaxX.z>=minmaxZ.z))
            {
                i = 0; j = 1; k = 2; calcVariable[i].y = minmaxX.y; calcVariable[i].z = minmaxX.x;
            }
            else if ((minmaxY.z >= minmaxX.z) && (minmaxY.z >= minmaxZ.z))
            {
                i = 1; j = 2; k = 0; calcVariable[i].y = minmaxY.y; calcVariable[i].z = minmaxY.x;
            }
            else
            {
                i = 2; j = 0; k = 1; calcVariable[i].y = minmaxZ.y; calcVariable[i].z = minmaxZ.x;
            }

            plane = new Vector3[4];
            plane[0].x = (p2.y * p3.z - p2.y * p1.z - p1.y * p3.z - p3.y * p2.z + p3.y * p1.z + p1.y * p2.z);

            plane[1].x = p2.z * p3.x - p2.z * p1.x - p1.z * p3.x - p3.z * p2.x + p3.z * p1.x + p1.z * p2.x;

            plane[2].x = p2.x * p3.y - p2.x * p1.y - p1.x * p3.y - p3.x * p2.y + p3.x * p1.y + p1.x * p2.y;

            plane[3].x = -p1.x * plane[0].x - p1.y * plane[1].x - p1.z * plane[2].x;
        }



        bool InBetween(Vector3 p)
        {
            if ((p.x - minmaxX.x > res.x) && (minmaxX.y - p.x > res.x) && (p.y - minmaxY.x > res.x) && (minmaxY.y - p.y > res.x) && (p.z - minmaxZ.x > res.x) && (minmaxZ.y - p.z > res.x))
            {
                return true;
            }
            else return false; 
        }

        public Vector3 Intersection(Quadrangle q2, bool direct, ref bool intsc)
        {
            bool flag = true;
            calcVariable[3].x = Mathf.Abs(calcVariable[i].y - calcVariable[i].z) / 10000;

            if (direct)
            {
                calcVariable[i].x = calcVariable[i].y;
                calcVariable[3].x = -1.0f * calcVariable[3].x;
            }
            else calcVariable[i].x = calcVariable[i].z;

            calcVariable[k].x = ((q2.plane[j].x / plane[j].x) * (plane[i].x * calcVariable[i].x + plane[3].x) - q2.plane[i].x * calcVariable[i].x - q2.plane[3].x) / (q2.plane[k].x - plane[k].x * q2.plane[j].x / plane[j].x);
            calcVariable[j].x = (-plane[k].x * calcVariable[k].x - plane[i].x * calcVariable[i].x - plane[3].x) / plane[j].x;

            do
            {                   
                calcVariable[i].x += calcVariable[3].x;               
                calcVariable[k].x = ((q2.plane[j].x / plane[j].x) * (plane[i].x * calcVariable[i].x + plane[3].x) - q2.plane[i].x * calcVariable[i].x - q2.plane[3].x) / (q2.plane[k].x - plane[k].x * q2.plane[j].x / plane[j].x);
                calcVariable[j].x = (-plane[k].x * calcVariable[k].x - plane[i].x * calcVariable[i].x - plane[3].x) / plane[j].x;

                res.y = plane[i].x * calcVariable[i].x + plane[j].x * calcVariable[j].x + plane[k].x * calcVariable[k].x + plane[3].x - (q2.plane[i].x * calcVariable[i].x + q2.plane[j].x * calcVariable[j].x + q2.plane[k].x * calcVariable[k].x + q2.plane[3].x);
                if ((res.y < res.x) && (q2.InBetween(calc))) flag = false;
            } while (InBetween(calc) && flag);
            
            calc.x = calcVariable[0].x;
            calc.y = calcVariable[1].x;
            calc.z = calcVariable[2].x;

            Debug.Log("Results are");
            if (InBetween(calc) && (q2.InBetween(calc)))
            {
                Debug.Log(calc); Debug.Log("They do");
                intsc = true;
                calc.x = (float)System.Math.Round(calc.x, 2);
                calc.y = (float)System.Math.Round(calc.y, 2);
                calc.z = (float)System.Math.Round(calc.z, 2);
                return calc;
            }
            else
            {
                Debug.Log("No Intersection");
                intsc = false;
                return new Vector3();
            }
        }

        public bool CylinderCheck(GameObject Cylind, Vector3 point)
        {
            if (Cylind.GetComponent<CapsuleCollider>().bounds.Contains(point))
            {
                Debug.Log("Yes"); Debug.Log(point);
                return true;
            }
            return false;
        }
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
