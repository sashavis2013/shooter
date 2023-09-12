using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Draws gizmos lines
/// </summary>
public class DebugDrawLine : MonoBehaviour
{
    public Color color = Color.red;
    public float length = 100.0f;


    private void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * length);
    }
}
