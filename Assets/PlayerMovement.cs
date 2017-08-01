using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private float _playerSpeed;
    public float PlayerSpeed { get { return _playerSpeed; } }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        MovePlayer();
    }

    private void MovePlayer()
    {
        float xMovement = Input.GetAxis("Horizontal") * Time.deltaTime * _playerSpeed;
        float zMovement = Input.GetAxis("Vertical") * Time.deltaTime * _playerSpeed;

        transform.Rotate(0, xMovement, 0);
        transform.Translate(0, 0, zMovement);
    }
}
