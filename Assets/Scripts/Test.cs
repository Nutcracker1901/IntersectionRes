using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject tPrefab;
    Transform Wound1, Wound2;
    bool stopFlag = true;
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
        GetComponentInChildren<Chopper>().startTest();
        Wound1.transform.position += Vector3.up * 0.1f;
        Wound2.transform.position += Vector3.up * 0.1f;
    }

    public void startTest()
    {
        stopFlag = !stopFlag;
    }
    public void resetObjects(Transform transform1, Transform transform2)
    {
        GameObject buff;
        if (transform.childCount < 1)
        {
            buff = GameObject.Instantiate(tPrefab, transform);
        }
        else
        {
            Destroy(transform.GetChild(0).gameObject);
            buff = GameObject.Instantiate(tPrefab, transform);
        }
        buff.transform.GetChild(1).position = transform1.position;
        buff.transform.GetChild(1).rotation = transform1.rotation;
        buff.transform.GetChild(2).position = transform2.position;
        buff.transform.GetChild(2).rotation = transform2.rotation;
    }
}
