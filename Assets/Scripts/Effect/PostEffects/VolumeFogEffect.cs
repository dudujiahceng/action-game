using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeFogEffect : MonoBehaviour {
    public GameObject cameraMain;
    float startAngle = 0;
    void Update()
    {
        startAngle += 0.05f % 360f;
        GetComponent<Renderer>().material.SetVector("_CameraPosition", cameraMain.transform.position);
        GetComponent<Renderer>().material.SetFloat("_StartAngle", startAngle);
    }
}
