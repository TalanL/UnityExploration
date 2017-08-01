using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

enum OptionalToggle
{
    Ignore, Yes, No
}

public class HexMapEditor : MonoBehaviour {
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activecolor;
    public int activeElevation;
    private bool applyColor;
    private bool applyElevation = true;
    private int brushSize;
    private OptionalToggle riverMode;
    private bool isDrag;
    private HexDirection dragDirection;
    private HexCell previousCell;

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
        else
        {
            previousCell = null;
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if(previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            } else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        } else
        {
            previousCell = null;
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
            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            } else if(isDrag && riverMode == OptionalToggle.Yes)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    otherCell.SetOutgoingRiver(dragDirection);
                }
            }
        }
    }

    private void ValidateDrag (HexCell currentCell)
    {
        for(dragDirection = HexDirection.NE; dragDirection<= HexDirection.NW; dragDirection++)
        {
            if(previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
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

    public void SetRiverMove (int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
}
