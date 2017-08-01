using UnityEngine;
using System.Collections;

public class HexCell : MonoBehaviour {

    public HexGridChunk chunk;
    public HexCoordinates coordinates;
    public RectTransform uiRect;

    [SerializeField]
    private int elevation = int.MinValue;

    [SerializeField]
    private HexCell[] neighbors;


    //All of this is set up for one incoming river and one outgoing river
    //If I want to expand this I will need to change it to an array.
    [SerializeField]
    private bool _hasIncomingRiver;
    public bool HasIncomingRiver
    {
        get
        {
            return _hasIncomingRiver;
        }
        set
        {
            _hasIncomingRiver = false;
        }
    }

    [SerializeField]
    private bool _hasOutgoingRiver;
    public bool HasOutgoingRiver
    {
        get
        {
            return _hasOutgoingRiver;
        }
        set
        {
            _hasOutgoingRiver = value;
        }
    }

    [SerializeField]
    private HexDirection _incomingRiver;
    public HexDirection IncomingRiver {
        get {
            return _incomingRiver;
        }
        set
        {
            _incomingRiver = value;
        }
    }


    [SerializeField]
    private HexDirection _outgoingRiver;
    public HexDirection OutgoingRiver
    {
        get
        {
            return _outgoingRiver;
        }
        set
        {
            _outgoingRiver = value;
        }
    }

    public bool HasRiver
    {
        get
        {
            return HasIncomingRiver || HasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return HasIncomingRiver != HasOutgoingRiver;
        }
    }

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

                if (HasOutgoingRiver && elevation < GetNeighbor(OutgoingRiver).elevation)
                {
                    RemoveOutgoingRiver();
                }
                if (HasIncomingRiver && elevation > GetNeighbor(IncomingRiver).elevation)
                {
                    RemoveIncomingRiver();
                }

                Refresh();
            }

            
        }
    }

    public Vector3 Position
    {
        get { return transform.localPosition; }
    }

    public float StreamBedY
    {
        get
        {
            return (elevation + HexMetrics.streamBedElevationOffset) * HexMetrics.elevationStep;
        }
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

    public bool HasRiverThroughEdge ( HexDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction || HasOutgoingRiver && OutgoingRiver == direction;
    }

    public void RemoveOutgoingRiver()
    {
        if(HasOutgoingRiver)
        {
            HasOutgoingRiver = false;
            Refresh();

            HexCell neighbor = GetNeighbor(OutgoingRiver);
            neighbor.HasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        } 
    }

    public void RemoveIncomingRiver()
    {
        if (HasIncomingRiver)
        {
            HasIncomingRiver = false;
            Refresh();

            HexCell neighbor = GetNeighbor(IncomingRiver);
            neighbor.HasIncomingRiver = false;
            neighbor.RefreshSelfOnly();
        }
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (HasOutgoingRiver && OutgoingRiver == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
        {
            return;
        }

        RemoveOutgoingRiver();
        if(HasIncomingRiver && IncomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        HasOutgoingRiver = true;
        OutgoingRiver = direction;
        RefreshSelfOnly();

        neighbor.RemoveIncomingRiver();
        neighbor.HasIncomingRiver = true;
        neighbor.IncomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
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

    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

}
