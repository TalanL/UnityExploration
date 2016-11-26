using UnityEngine;
using System.Collections;

//Easy Access for our hexes outer and inner radiuses;
public static class HexMetrics {
    
    public const float outerRadius = 10f;

    //Magic number here is 5 * route of three;
    public const float innerRadius = outerRadius * 0.866025404f;

    //Not a fan of this static loop in corners. 
    public static Vector3[] corners =
    {
        new Vector3(0f,0f,outerRadius),
        new Vector3(innerRadius, 0f,0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3( 0f,0f,-outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f,0f,outerRadius)
    };
	
}
