using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HexMapEditor : MonoBehaviour {
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activecolor;
    public int activeElevation;

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
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    private void EditCell(HexCell cell)
    {
        cell.color = activecolor;
        cell.Elevation = activeElevation;
        hexGrid.Refresh();
    }

    public void SelectColor(int index)
    {
        activecolor = colors[index];
    }

    public void SelectElevation (float elevation)
    {
        activeElevation = (int)elevation;
    }
}
