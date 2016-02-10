using UnityEngine;
using System.Collections;
using Leap;

public class leapTest : MonoBehaviour {
    Controller controller;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Frame frame = controller.Frame();
	
	}
}

