using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public static class DrawGizmos
{
    public static void Circle(Vector3 center, float radius, Color color)
    {
        Gizmos.color = color;
        int segments = 64;
        float angle = 0f;
        float step = 360f / segments;

        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            angle += step;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
