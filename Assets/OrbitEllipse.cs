using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitEllipse : MonoBehaviour
{
    LineRenderer lineRenderer;
    [Range(3,36)] public int segments = 12;
    public float xAxis = 0;
    public float yAxis = 0;

    public void SetAxis(float x, float y){
        lineRenderer = GetComponent<LineRenderer>();
        this.xAxis = x;
        this.yAxis = y;
        CaclulateEllispe();
        lineRenderer.enabled = true;
    }
    public void HideOrbit(){
        lineRenderer.enabled = false;
    }

    void CaclulateEllispe(){
        Vector3[] points = new Vector3[segments+1];
        for(int i = 0; i < segments; i++){
            float angle = ((float)i/(float)segments) * 360 * Mathf.Deg2Rad;
            float x= Mathf.Sin(angle) * xAxis;
            float y = Mathf.Cos(angle) * yAxis;
            points[i] = new Vector3(x, y, 0f);
        }
        points[segments] = points[0];

        lineRenderer.positionCount = segments + 1;
        lineRenderer.SetPositions(points);
    }
}
