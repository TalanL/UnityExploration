using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HexGrid : MonoBehaviour {

    public int chunkCountX = 4;
    public int chunkCountZ = 3;

    private int cellCountX;
    private int cellCountZ;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public Color defaultColor = Color.white;

    private HexCell[] cells;

    public Texture2D noiseSource;

    public HexGridChunk chunkPrefab;
    private HexGridChunk[] chunks;

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for(int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];
        int cellIndex = 0;
        for (int z = 0; z < cellCountZ; z++)
        {
            for (int y = 0; y < cellCountX; y++)
            {
                CreateCell(y, z, cellIndex++);
            }
        }
    }

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if( z > 0)
        {
           if((z&1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if(x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToSTringOnSeparateLines();
        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.x + coordinates.z * cellCountX + coordinates.z / 2;
        
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        bool inBounds = true;
        int z = coordinates.z;
        if (z < 0 || z >= cellCountZ)
        {
            inBounds = false;
        }
        int x = coordinates.x + z/2;
        if (x < 0 || x >= cellCountX)
        {
            inBounds = false;
        }
        return (inBounds) ? cells[x + z * cellCountX] : null;
    }

    public void ShowUI(bool visable)
    {
        for(int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visable);
        }
    }
}
