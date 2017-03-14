using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour {

    private HexCell[] cells;
    private HexMesh hexMesh;
    private Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }
    
    public void Refresh()
    {
        enabled = true;
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
        cell.chunk = this;
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }
}
