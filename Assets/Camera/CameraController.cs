using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject LookAtTarget;
    public Vector3 OffSet;

	// Use this for initialization
	void Start () {
        OffSet = transform.position - LookAtTarget.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
        transform.position = LookAtTarget.transform.position + OffSet; 
    }
}
