using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HexMapEditor : MonoBehaviour {
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activecolor;
    public int activeElevation;
    private bool applyColor;
    private bool applyElevation = true;
    private int brushSize;

    void Awake()
    {
        SelectColor(0);
    }

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.x;
        int centerZ = center.coordinates.z;

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for(int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }


    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activecolor;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
        }
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activecolor = colors[index];
        }
    }

    public void SelectElevation (float elevation)
    {
            activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visable)
    {
        hexGrid.ShowUI(visable);
    }
}
