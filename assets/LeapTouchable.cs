using UnityEngine;
using System.Collections;
using Leap;

public class LeapTouchable : MonoBehaviour {
    Controller controller;

	Vector3 startPoint;
	Quaternion startRotation;
	RigidbodyConstraints startConstraints;
	bool preventOverlappingCollision = false;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
		startRotation = transform.rotation;
		startConstraints = rigidbody.constraints;
	}
	
	// Update is called once per frame
	void Update () {
        //Frame frame = controller.Frame();
	
	}

	public RigidbodyConstraints OrgRigidBodyConstraints
	{
		get {return startConstraints;}
	}

	public bool PreventOverlappingCollision
	{
		get {return preventOverlappingCollision;}
		set {preventOverlappingCollision = value;}
	}

	public void reset(){
		transform.position = startPoint;
		transform.rotation = startRotation;
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		//Debug.Log ("Touchable OnCollisionEnter");
		//Debug.Log (gameObject);
	}

	void OnCollisionStay(Collision collisionInfo) 
	{
		// Do something here (to prevent abnormal movement) if have time
		// TODO: if overlapping, freeze the position & rotation of collisionInfo.gameObject
		/*if (collisionInfo.gameObject.tag == "Touchable" && LeapInput.DraggingMode) {
			LeapTouchable tcb = (LeapTouchable)collisionInfo.gameObject.GetComponent(typeof(LeapTouchable));
			if (tcb != null){
				if (gameObject.renderer.bounds.Intersects(collisionInfo.gameObject.renderer.bounds)) {
					if (gameObject.renderer.bounds.center.y > collisionInfo.gameObject.renderer.bounds.center.y){
						collisionInfo.gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
					}
				}
				else{
					collisionInfo.gameObject.rigidbody.constraints = tcb.OrgRigidBodyConstraints;
				}
			}
		}*/
	}

	void OnCollisionExit(Collision collisionInfo) 
	{
		// Do something here
		//Debug.Log ("Touchable OnCollisionExit " + gameObject + " " + collisionInfo.gameObject);

		LeapTouchable tcb = (LeapTouchable)collisionInfo.gameObject.GetComponent(typeof(LeapTouchable));
		if (tcb != null) {
			collisionInfo.gameObject.rigidbody.constraints = tcb.OrgRigidBodyConstraints;
		}

		if (collisionInfo.gameObject.tag == "Touchable") {
			if (preventOverlappingCollision && !LeapInput.DraggingMode){
				/*Debug.Log(gameObject + " need to prevent overlapping collision of itself and object it is colliding with.");
				Debug.Log(gameObject.renderer.bounds.size.y);
				Debug.Log(gameObject.renderer.bounds.center.y);
				Debug.Log(collisionInfo.gameObject.renderer.bounds.size.y/2);
				Debug.Log(collisionInfo.gameObject.renderer.bounds.center.y);*/
				gameObject.rigidbody.isKinematic = true;
				collisionInfo.gameObject.rigidbody.isKinematic = true;
				Vector3 newPos = gameObject.transform.position;
				newPos.y = collisionInfo.gameObject.renderer.bounds.center.y + collisionInfo.gameObject.renderer.bounds.size.y/2 + 0.2f;
				gameObject.transform.position = newPos;
				collisionInfo.gameObject.rigidbody.isKinematic = false;
				collisionInfo.gameObject.rigidbody.velocity = new Vector3(0, 0, 0);
				gameObject.rigidbody.isKinematic = false;
				gameObject.rigidbody.velocity = new Vector3(0, 0, 0);
				preventOverlappingCollision = false;
			}
		}


	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Touchable OnTriggerEnter");
		Debug.Log (gameObject);
	}
}
