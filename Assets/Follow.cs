using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    private GameObject camera;
	// Use this for initialization
	void Start () {
        camera = GameObject.Find("Camera");
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 direction = camera.transform.position-transform.position;
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),0.1f*Time.deltaTime);
        transform.LookAt(camera.transform);
	}
}
