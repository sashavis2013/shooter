using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary >
/// Draws gizmos spheres
/// </summary>
public class DebugDrawSphere : MonoBehaviour
{
    public Color color = Color.red;
    public float radius = 0.05f;
    
    private void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
