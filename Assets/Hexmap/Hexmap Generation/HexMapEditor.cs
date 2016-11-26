using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HexMapEditor : MonoBehaviour {
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activecolor;

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
            hexGrid.ColorCell(hit.point, activecolor);
        }
    }

    public void SelectColor(int index)
    {
        activecolor = colors[index];
    }
}
