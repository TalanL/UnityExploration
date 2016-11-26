using UnityEngine;
using System.Collections;

[System.Serializable]
public struct HexCoordinates {
    [SerializeField]
    private int _x, _z;

    public int x { get { return _x; } private set { } }
    public int z { get { return _z; } private set { } }
    public int y { get { return -x - z; } }


    public HexCoordinates(int x, int z)
    {
        this._x = x;
        this._z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int X  = Mathf.RoundToInt(x);
        int Y = Mathf.RoundToInt(y);
        int Z = Mathf.RoundToInt(-x - y);

        if( X + Y + Z != 0)
        {
            float deltaX = Mathf.Abs(x - X);
            float deltaY = Mathf.Abs(y - Y);
            float deltaZ = Mathf.Abs(-x - y - Z);

            if(deltaX > deltaY && deltaX > deltaZ)
            {
                X = -Y - Z;
            }
            else if (deltaZ > deltaY)
            {
                Z = -X - Y;
            }
        }
        return new HexCoordinates(X, Z);
    }

    public override string ToString()
    {
        return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
    }

    public string ToSTringOnSeparateLines()
    {
        return x.ToString() + "\n" + y.ToString() + "\n" +  z.ToString();
    }


}
