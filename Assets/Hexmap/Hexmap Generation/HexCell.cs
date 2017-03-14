using UnityEngine;
using System.Collections;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;

    
    public RectTransform uiRect;

    [SerializeField]
    private int elevation = int.MinValue;

    [SerializeField]
    private HexCell[] neighbors;

    public HexGridChunk chunk;

    private Color _color;
    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            if(_color != value)
            {
                _color = value;
                Refresh();
            }
        }
    }

    public int Elevation {
        get {
            return elevation;
        }
        set {
            if(elevation != value)
            {
                elevation = value;
                Vector3 position = transform.localPosition;
                position.y = value * HexMetrics.elevationStep;
                position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
                transform.localPosition = position;

                Vector3 uiPosition = uiRect.localPosition;
                uiPosition.z = -position.y;
                uiRect.localPosition = uiPosition;
                if (elevation == value)
                {

                }
                Refresh();
            }
        }
    }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType (HexDirection direction)
    {
        return HexMetrics.getEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.getEdgeType(elevation, otherCell.elevation);
    }

    private void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for(int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}
