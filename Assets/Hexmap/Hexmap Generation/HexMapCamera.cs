using UnityEngine;
using System.Collections;

public class HexMapCamera : MonoBehaviour {

    public float stickMinZoom;
    public float stickMaxZoom;
    public float swivelMinZoom;
    public float swivelMaxZoom;
    public float moveSpeedMinZoom;
    public float moveSpeedMaxZoom;
    public float rotationSpeed;
    public HexGrid map;


    private Transform swivel;
    private Transform stick;
    private float zoom = 1f;
    private float rotationAngle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        HandleZoom();
        HandleRotation();
        HandleMove();
    }

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    private void HandleZoom()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta !=0f)
        {
            AdjustZoom(zoomDelta);
        }
    }

    private void AdjustZoom(float zoomDelta)
    {
        zoom = Mathf.Clamp01(zoom + zoomDelta);
        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void HandleRotation()
    {
        float rotationDelta = Input.GetAxis("Rotation");
        if(rotationDelta != 0)
        {
            AdjustRotation(rotationDelta);
        }
    }

    private void AdjustRotation(float rotationDelta)
    {
        rotationAngle += rotationDelta * rotationSpeed * Time.deltaTime;
        if(rotationAngle < 0f)
        {
            rotationAngle += 360f;
        } else if(rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    private void HandleMove()
    {
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if(xDelta != 0f || zDelta != 0f){
            AdjustPosition(xDelta, zDelta);
        }
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 normalizedDelta = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += normalizedDelta * distance;
        transform.localPosition = position;

        transform.localPosition = ClampPosition(position);
    }

    //fix magic numbers here. 
    //They are to correct the camera to center on the cells properly. 
    private Vector3 ClampPosition(Vector3 position)
    {
        Vector3 clampedPosition = position;

        float xMax = (map.chunkCountX * HexMetrics.chunkSizeX -0.5f) * (2f * HexMetrics.innerRadius);
        clampedPosition.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (map.chunkCountZ * HexMetrics.chunkSizeZ - 1) * (1.5f * HexMetrics.innerRadius);
        clampedPosition.z = Mathf.Clamp(position.z, 0f, zMax);

        return clampedPosition;
    }

}