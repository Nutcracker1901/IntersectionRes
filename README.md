# IntersectionRes
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
                        if(intsc) Cut(newWound, wound, intscPoint1, intscPoint2);
                    }
                }
            }
            wounds.Add(newWound);
        }



void Cut(Wound wound1, Wound wound2, Vector3 intscPoint1, Vector3 intscPoint2)
        {
            float t = Vector3.Distance(wound1.quad.points[0], intscPoint1);
            float w =  Vector3.Distance(wound1.quad.points[0], intscPoint2);
            if (Vector3.Distance(wound1.quad.points[0], intscPoint1) < Vector3.Distance(wound1.quad.points[0], intscPoint2))
			{
               Vector3 temp = intscPoint1;
               intscPoint1 = intscPoint2;
               intscPoint2 = temp;
			}

            Vector3[] resVertices = new Vector3[6]
        {
            //wound1.quad.points[0], wound1.quad.points[1], wound2.quad.points[0], wound2.quad.points[2], intscPoint1, intscPoint2
            wound1.quad.points[0], wound1.quad.points[1], wound2.quad.points[0], wound2.quad.points[1], intscPoint1, intscPoint2
        };
            Mesh resMesh = new Mesh();
            resMesh.vertices = resVertices;
            int[] trianglesIndex = new int[6]
