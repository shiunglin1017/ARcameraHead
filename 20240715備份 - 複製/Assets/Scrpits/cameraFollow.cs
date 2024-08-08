using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject targetCamera; // Renamed from 'camera'
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        // canvas.transform.position = targetCamera.transform.position + Vector3.forward * 5;
    }

    // Update is called once per frame
    void Update()
    {
        canvas.transform.position = targetCamera.transform.position + Vector3.forward * 5;
    }
}
