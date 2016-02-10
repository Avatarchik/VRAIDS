using UnityEngine;
using System.Collections;
using Camerat;

public class head_tracking : MonoBehaviour {
    Programhead test = new Programhead();
    Coordinate d;
	// Use this for initialization
	void Start () {
        test.CameraCapture();
	
	}
	
	// Update is called once per frame
	void Update () {
        d = test.getcoor();

        Debug.Log("x = " + d.x);
        //Debug.Log("test");
	}
}
