using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startTest()
    {
        transform.GetChild(0).GetChild(0).GetComponent<Chopper>(); 
    }

    void resetOobjects()
    {

    }
}
