using UnityEngine;
using System.Collections;
using System;

//Easy Access for our hexes outer and inner radiuses;
public static class HexMetrics {
    
    public const float outerRadius = 10f;

    //Magic number here is 5 * route of three;
    public const float innerRadius = outerRadius * 0.866025404f;

    public const float solidFactor = 0.75f;

    public const float blendfactor = 1f - solidFactor;

    public const float elevationStep = 5f;

    //Not a fan of this static loop in corners. 
    private static Vector3[] corners =
    {
        new Vector3(0f,0f,outerRadius),
        new Vector3(innerRadius, 0f,0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3( 0f,0f,-outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f,0f,outerRadius)
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction  + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendfactor;
    }
}
