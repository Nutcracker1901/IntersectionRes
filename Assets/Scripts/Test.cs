using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject tPrefab;
    Transform Wound1, Wound2;
    bool stopFlag = false;
    float timer = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Wound1 = tPrefab.transform.GetChild(1);
        Wound2 = tPrefab.transform.GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopFlag)
        {
            if (timer < 4.9f)
            {
                timer += Time.deltaTime;
            } else
            {
                UpdateWrapper();
                timer = 0f;
            }
            Debug.Log("StopFlag is false!");
        }
        Debug.Log(timer);
    }

    void UpdateWrapper()
    {
        resetObjects(Wound1, Wound2);
        tPrefab.GetComponentInChildren<Chopper>().startTest();
        Wound1.up += new Vector3(0, 0.01f);
        Wound2.up += new Vector3(0, 0.01f);
    }

    public void startTest()
    {
        stopFlag = !stopFlag;
    }

    public void resetObjects()
    {
        if (transform.childCount < 1)
        {
            GameObject.Instantiate(tPrefab, transform);
        } else
        {
            Destroy(transform.GetChild(0).gameObject);
            GameObject.Instantiate(tPrefab, transform);
        }
    }

    public void resetObjects(Transform transform1, Transform transform2)
    {
        tPrefab.transform.GetChild(1).position = transform1.position;
        tPrefab.transform.GetChild(1).rotation = transform1.rotation;
        tPrefab.transform.GetChild(2).position = transform2.position;
        tPrefab.transform.GetChild(2).rotation = transform2.rotation;

        if (transform.childCount < 1)
        {
            GameObject.Instantiate(tPrefab, transform);
        }
        else
        {
            Destroy(transform.GetChild(0).gameObject);
            GameObject.Instantiate(tPrefab, transform);
        }
    }
}
