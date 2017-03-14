using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    private Mesh hexMesh;
    private MeshCollider meshCollider;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Color> colors = new List<Color>();

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Triangulate(HexCell[] cells)
    {
        
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for(int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    private void Triangulate(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    private void Triangulate(HexDirection direction, HexCell cell)
    {
        //triangles
        Vector3 center = cell.Position;
        EdgeVertices edge = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction));

        TriangulateEdgeFan(center, edge, cell.Color);

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, edge);
        }

    }

    private void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices edge)
    {
        HexCell neighbor = cell.GetNeighbor(direction) ;
        if(neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.Position.y - cell.Position.y;
        EdgeVertices edge2 = new EdgeVertices(
            edge.v1 + bridge,
            edge.v4 + bridge
            );
        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(edge, cell, edge2,  neighbor);
        }
        else
        {
            TriangulateEdgeStrip(edge, cell.Color, edge2, neighbor.Color);
        }
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if(direction <= HexDirection.E && nextNeighbor != null)
        {
            //triangles between three hexes
            Vector3 v5 = edge.v4 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Position.y;
            if(cell.Elevation <= neighbor.Elevation)
            {
                if(cell.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(edge.v4, cell, edge2.v4, neighbor, v5, nextNeighbor);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, edge.v4, cell, edge2.v4, neighbor);
                }
            }
            else if(neighbor.Elevation <= nextNeighbor.Elevation)
            {
                TriangulateCorner(edge2.v4, neighbor, v5, nextNeighbor, edge.v4, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, edge.v4, cell, edge2.v4, neighbor);
            }
        }
    }

    private void TriangulateEdgeTerraces(EdgeVertices begin, HexCell beginCell, EdgeVertices end, HexCell endCell)
    {
        EdgeVertices edge2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

        TriangulateEdgeStrip(begin, beginCell.Color, edge2, c2);

        for(int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices edge = edge2;
            Color c1 = c2;
            edge2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
            TriangulateEdgeStrip(edge, c1, edge2, c2);
        }


        TriangulateEdgeStrip(edge2, c2, end, endCell.Color);
    }

    private void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if(leftEdgeType == HexEdgeType.Slope)
        {
            if(rightEdgeType == HexEdgeType.Slope)
            {
                //slope slope flat
                TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if(rightEdgeType == HexEdgeType.Flat)
            {
                //slope flat slope
                TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
            }
            else
            {
                //slope cliff slope
                TriangulateCornerTerraceCliff(bottom, bottomCell, left, leftCell, right, rightCell);
            }

        } else if(rightEdgeType == HexEdgeType.Slope)
        {
            if(leftEdgeType == HexEdgeType.Flat)
            {
                //flat slope slope
                TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {
                TriangulateCornerCliffTerrace(bottom, bottomCell, left, leftCell, right, rightCell);
            }
        }
        else if(leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if(leftCell.Elevation < rightCell.Elevation)
            {
                TriangulateCornerCliffTerrace(right, rightCell, bottom, bottomCell, left, leftCell);
            }
            else
            {
                TriangulateCornerTerraceCliff(left, leftCell, right, rightCell, bottom, bottomCell);
            }
        }
        else
        {
            AddTriangle(bottom, left, right);
            AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
        }
    }

    private void TriangulateCornerTerraces (Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        Vector3 leftVector = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 rightVector = HexMetrics.TerraceLerp(begin, right, 1);
        Color leftColor = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
        Color rightColor = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

        AddTriangle(begin, leftVector, rightVector);
        AddTriangleColor(beginCell.Color, leftColor, rightColor);

        for(int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 tempLeft = leftVector;
            Vector3 tempRight = rightVector;
            Color tempLeftColor = leftColor;
            Color tempRightColor = rightColor;
            leftVector = HexMetrics.TerraceLerp(begin, left, i);
            rightVector = HexMetrics.TerraceLerp(begin, right, i);
            leftColor = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            rightColor = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);

            addQuad(tempLeft, tempRight, leftVector, rightVector);
            addQuadColor(tempLeftColor, tempRightColor, leftColor, rightColor);
        }

        addQuad(leftVector, rightVector, left, right);
        addQuadColor(leftColor, rightColor, leftCell.Color, rightCell.Color);

    }

    private void TriangulateCornerTerraceCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float boundryLerpStep = 1f / (rightCell.Elevation - beginCell.Elevation);
        if(boundryLerpStep < 0)
        {
            boundryLerpStep = -boundryLerpStep;
        }

        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(right), boundryLerpStep);
        Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, boundryLerpStep);

        TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

        if(leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
        }
        else
        {
            AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }

    private void TriangulateCornerCliffTerrace(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell)
    {
        float boundryLerpStep = 1f / (leftCell.Elevation- beginCell.Elevation);
        if (boundryLerpStep < 0)
        {
            boundryLerpStep = -boundryLerpStep;
        }
        Vector3 boundary = Vector3.Lerp(Perturb(begin), Perturb(left), boundryLerpStep);
        Color boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, boundryLerpStep);

        TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
        }
        else
        {
            AddTriangleUnperturbed(Perturb(left), Perturb(right), boundary);
            AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }

    private void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor)
    {
        Vector3 leftLerpVector = Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        Color leftLerpColor = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

        AddTriangleUnperturbed(Perturb(begin), leftLerpVector, boundary);
        AddTriangleColor(beginCell.Color, leftLerpColor, boundaryColor);

        for(int i = 2; i< HexMetrics.terraceSteps; i++)
        {
            Vector3 tempLeftLerpVector = leftLerpVector;
            Color tempLeftLerpColor = leftLerpColor;
            leftLerpVector = Perturb( HexMetrics.TerraceLerp(begin, left, i));
            leftLerpColor = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            AddTriangleUnperturbed(tempLeftLerpVector, leftLerpVector, boundary);
            AddTriangleColor(tempLeftLerpColor, leftLerpColor, boundaryColor);
        }
        AddTriangleUnperturbed(leftLerpVector, Perturb(left), boundary);
        AddTriangleColor(leftLerpColor, leftCell.Color, boundaryColor);
    }

    private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        AddTriangle(center, edge.v1, edge.v2);
        AddTriangleColor(color);
        AddTriangle(center, edge.v2, edge.v3);
        AddTriangleColor(color);
        AddTriangle(center, edge.v3, edge.v4);
        AddTriangleColor(color);
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
    {
        addQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        addQuadColor(c1, c2);
        addQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        addQuadColor(c1, c2);
        addQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        addQuadColor(c1, c2);
    }

    private void AddTriangleColor (Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    private void AddTriangleColor(Color color1, Color color2, Color color3)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void addQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        vertices.Add(Perturb(v4));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    private void addQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    private void addQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    private Vector3 Perturb (Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        Vector3 sampledPosition = position;
        sampledPosition.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
        sampledPosition.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
        return sampledPosition;
    }
}
