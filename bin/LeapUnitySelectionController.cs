/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Leap;

//Contains all the logic for the focusing, selecting, highlighting, and moving of objects
//based on leap input.  Depends on The LeapUnityHandController & the LeapFingerCollisionDispatcher
//to move the hand representations around and detect collisions.  Highlighting is achieved by adding
//an highlight material to the object, then manipulating it's color based on how close it is to being
//selected.  Also depends on LeapInput & LeapInputUnityBridge. Currently just a prototype, it is 
//disabled in the scene by default.

public class LeapUnitySelectionController : MonoBehaviour {

	public static LeapUnitySelectionController Get()
	{
		return (LeapUnitySelectionController)GameObject.FindObjectOfType(typeof(LeapUnitySelectionController));		
	}
	
	public virtual bool CheckEndSelection(Frame thisFrame)
	{
		if( m_Touching.Count == 0 )
			return true;

		// TODO: if one finger is lost, then it is forever lost. Anyway to get it back?
		if( m_Touching.Count == 1 ){
			if (!oneFingerLost){
				m_FingerLostTime = Time.time;
			}
			oneFingerLost = true;
		}
		else {
			oneFingerLost = false;
			m_FingerLostTime = 0.0f;
		}
		/*if (LeapInput.NumberOfFingers [0] != 2 && LeapInput.NumberOfFingers [1] != 2) {
			checkTime = true;
		} 
		else if (m_ObjectTouched[0].finger.transform.parent.gameObject.transform.parent.childCount > 3){
			//Debug.Log(m_ObjectTouched[0].finger.transform.parent.gameObject.transform.parent.childCount);
			checkTime = true;
		}*/

		if (oneFingerLost && Time.time - m_FirstTouchedTime > kMinSelectionTime + kSelectionTime && Time.time - m_FingerLostTime > kIdleStartDeselectTime + kSelectionTime) {
			return true;
		}
		
		return false;
	}
	public virtual bool CheckShouldMove(Frame thisFrame)
	{
        //if (LeapInput.EnableTranslation && m_Touching.Count == 2 && m_ObjectTouched.Count == 2)
		if (LeapInput.EnableTranslation && m_ObjectTouched.Count == 2)
		{
			//Debug.Log("m_Touching[0].transform.parent.gameObject == m_Touching[1].transform.parent.gameObject:" + (m_Touching[0].transform.parent.gameObject == m_Touching[1].transform.parent.gameObject));
			//Debug.Log("m_Touching[0].transform.parent.gameObject.transform.parent.gameObject:" + m_Touching[0].transform.parent.gameObject.transform.parent.gameObject);
			//Debug.Log("m_Touching[1].transform.parent.gameObject.transform.parent.gameObject:" + m_Touching[1].transform.parent.gameObject.transform.parent.gameObject);
			if ((m_ObjectTouched[0].gameObject == m_ObjectTouched[1].gameObject) &&
			    (m_ObjectTouched[0].finger.transform.parent.gameObject.transform.parent.gameObject == m_ObjectTouched[1].finger.transform.parent.gameObject.transform.parent.gameObject))
			{
				return true;
			}
			return false;
		}
		return false;
	}
	public virtual bool CheckShouldRotate(Frame thisFrame)
	{
		return LeapInput.EnableRotation && m_Touching.Count >= 2;
	}
	public virtual bool CheckShouldScale(Frame thisFrame)
	{
		return LeapInput.EnableScaling && m_Touching.Count >= 2;
	}
	
	// public virtual bool CheckIfTouched(Frame thisFrame)
	// {
		// return LeapInput.EnableTranslation && m_Touching.Count == 2;
	// }
	
	public virtual void DoMovement(Frame thisFrame)
	{
        foreach (GameObject finger in m_Touching)
        {
            finger.collider.isTrigger = true;
        }
		Vector3 currPositionSum = new Vector3(0,0,0);
		Vector3 lastPositionSum = new Vector3(0,0,0);
		foreach( GameObject obj in m_Touching )
		{
			currPositionSum += obj.transform.position;
		}
		foreach( Vector3 vec in m_LastPos )
		{
			lastPositionSum += vec;	
		}
        m_FocusedObject.rigidbody.isKinematic = true;
		m_FocusedObject.transform.position += (currPositionSum - lastPositionSum) / m_Touching.Count;
	}
	public virtual void DoRotation(Frame thisFrame)
	{
		Vector3 lastVec = m_LastPos[1] - m_LastPos[0];
		Vector3 currVec = m_Touching[1].transform.position - m_Touching[0].transform.position;
		if( lastVec != currVec )
		{
			// TODO: is this exactly how this should be done?? NOPE => This leads to rotation to be limited to 1 axis
			if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationX) == RigidbodyConstraints.FreezeRotationX && 
			    (m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) == RigidbodyConstraints.FreezeRotationY){
				lastVec.z = 0.0f;
				currVec.z = 0.0f;
			}
			if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) == RigidbodyConstraints.FreezeRotationY && 
			    (m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ) == RigidbodyConstraints.FreezeRotationZ){
				lastVec.x = 0.0f;
				currVec.x = 0.0f;
			}
			if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationX) == RigidbodyConstraints.FreezeRotationX && 
			    (m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ) == RigidbodyConstraints.FreezeRotationZ){
				lastVec.y = 0.0f;
				currVec.y = 0.0f;
			}

			Vector3 axis = Vector3.Cross(currVec, lastVec);
			/*if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationX) == RigidbodyConstraints.FreezeRotationX){
				axis.x = 0.0f;
			}
			if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationY) == RigidbodyConstraints.FreezeRotationY){
				axis.y = 0.0f;
			}
			if ((m_FocusedObject.rigidbody.constraints & RigidbodyConstraints.FreezeRotationZ) == RigidbodyConstraints.FreezeRotationZ){
				axis.z = 0.0f;
			}*/

			float lastDist = lastVec.magnitude;
			float currDist = currVec.magnitude;
			float axisDist = axis.magnitude;
			float angle = -Mathf.Asin(axisDist / (lastDist*currDist));
			m_FocusedObject.transform.RotateAround(axis/axisDist, angle);
		}	
	}
	public virtual void DoScaling(Frame thisFrame)
	{
		Vector3 lastVec = m_LastPos[1] - m_LastPos[0];
		Vector3 currVec = m_Touching[1].transform.position - m_Touching[0].transform.position;
		if( lastVec != currVec )
		{
			float lastDist = lastVec.magnitude;
			float currDist = currVec.magnitude;
			//clamp the scale of the object so we don't shrink/grow too much
			Vector3 scaleClamped = m_FocusedObject.transform.localScale * Mathf.Clamp((currDist/lastDist), .8f, 1.2f);
			scaleClamped.x = Mathf.Clamp(scaleClamped.x, .3f, 5.0f);
			scaleClamped.y = Mathf.Clamp(scaleClamped.y, .3f, 5.0f);
			scaleClamped.z = Mathf.Clamp(scaleClamped.z, .3f, 5.0f);
			m_FocusedObject.transform.localScale = scaleClamped;
		}
	}

	void Update()
	{
		Leap.Frame thisFrame = LeapInput.Frame;
		if( thisFrame == null ) 
			return;

		if (LeapInput.NumberOfFingers[0] + LeapInput.NumberOfFingers[1] == 10) 
		{
			//Application.LoadLevel(Application.loadedLevel);

			foreach (objectTouched touchedObject in m_ObjectTouched)
			{
				touchedObject.finger.collider.isTrigger = false;
			}
			
			ClearFocus();
			
			GameObject[] allTouchable = GameObject.FindGameObjectsWithTag("Touchable");

			foreach (GameObject obj in allTouchable){
				LeapTouchable tcb = (LeapTouchable)obj.GetComponent(typeof(LeapTouchable));
				if (tcb != null)
					tcb.reset();
			}
			return;
		}
  
		//Remove fingers which have been disabled
		int index;
		while( (index = m_Touching.FindIndex(i => i.collider && i.collider.enabled == false)) != -1 ) 
		{
            m_Touching[index].collider.isTrigger = false;
			m_Touching.RemoveAt(index);
			m_LastPos.RemoveAt(index);
		}

		while( (index = m_ObjectTouched.FindIndex(i => i.finger.collider && i.finger.collider.enabled == false)) != -1 ) 
		{
			m_ObjectTouched[index].finger.collider.isTrigger = false;
			m_ObjectTouched.RemoveAt(index);
		}

		while( (index = m_ObjectTouched.FindIndex(i => i.finger.collider && i.finger.collider.enabled == false)) != -1 ) 
		{
			m_ObjectTouched.RemoveAt(index);
		}
		
		if( m_LastFrame != null && thisFrame != null && m_Selected)
		{
			float transMagnitude = thisFrame.Translation(m_LastFrame).MagnitudeSquared;
			if( transMagnitude > kMovementThreshold )
				m_LastMovedTime = Time.time;
		}
		
		//Set selection after the time has elapsed
		if( !m_Selected && m_FocusedObject && (Time.fixedTime - m_FirstTouchedTime) >= kSelectionTime )
			m_Selected = true;
		
		//Update the focused object's color
		float selectedT = m_FocusedObject != null ? (Time.time - m_FirstTouchedTime) / kSelectionTime : 0.0f;


		//If we have passed the minimum deselection threshold and are past the minimum time to start deselecting...
		if( m_Selected && Time.time - m_FirstTouchedTime > kIdleStartDeselectTime + kSelectionTime )
		{
			selectedT = 1.3f - (((Time.time - m_LastMovedTime) - kIdleStartDeselectTime) / kSelectionTime);
		}
		SetHighlightColor( Color.Lerp(kBlankColor, m_HighlightMaterial.color, selectedT) );

		//Process the movement of the selected object.
		if( m_Selected && thisFrame != m_LastFrame )
		{
			//End selection if we don't see any fingers or the scaling factor is going down quickly ( indicating we are making a fist )
			if( CheckEndSelection(thisFrame) )
			{
				foreach (objectTouched touchedObject in m_ObjectTouched)
				{
					touchedObject.finger.collider.isTrigger = false;
				}

				ClearFocus();
			}
			else
			{
				if( CheckShouldMove(thisFrame) )
				{
					DoMovement(thisFrame);
				}
				if( CheckShouldRotate(thisFrame) )
				{
					DoRotation(thisFrame);
				}
				if( CheckShouldScale(thisFrame) )
				{
					DoScaling(thisFrame);
				}
			}
		}

		
		m_LastFrame = thisFrame;
		for( int i = 0; i < m_Touching.Count; ++i )
		{
			m_LastPos[i] = m_Touching[i].transform.position;	
		}
	}

	// TODO: the current logic is very incorrect. Change the logic for m_Touching and m_ObjectTouched
	public void OnTouched(GameObject finger, Collider other)
	{		
		if (!m_Touching.Contains(finger))
		{
			m_Touching.Add(finger);
			m_LastPos.Add(finger.transform.position);
		}

		if (!m_ObjectTouched.Exists (obj => obj.finger == finger)) 
		{
			objectTouched obj = new objectTouched();
			obj.finger = finger;
			obj.gameObject = other.gameObject;
			m_ObjectTouched.Add(obj);
		}
		else if (!m_FocusedObject)
		{
			//objectTouched touched;
			//touched.finger = null;
			//touched.gameObject = null;
			int index = m_ObjectTouched.FindIndex(obj => obj.finger == finger);
			if (index != -1){
				objectTouched touched = m_ObjectTouched.Find(obj => obj.finger == finger);
				touched.gameObject = other.gameObject;
				m_ObjectTouched.RemoveAt(index);
				m_ObjectTouched.Add(touched);
			}
		}

        //if we're still just focused (not selected yet), change our focus
        //if (m_Touching.Count == 2 && (LeapInput.NumberOfFingers[0] == 2 || LeapInput.NumberOfFingers[1] == 2))
		if (LeapInput.NumberOfFingers[0] == 2 || LeapInput.NumberOfFingers[1] == 2)
		//if (m_Touching.Count == 2)
        {
			// only do this if it IS touched by 2 fingers, NOT if 1 finger touched the object, and the other
			// finger touched another
			List<objectTouched> l = m_ObjectTouched.FindAll(obj => obj.gameObject == other.gameObject);
            //if (!m_Selected && other.gameObject != m_FocusedObject && 
			//    m_ObjectTouched.FindAll(obj => obj.gameObject == other.gameObject).Count == 2)

			// TODO: is this a good method ????
			if (l.Count == 2 && 
			    (l[0].finger.transform.parent.gameObject.transform.parent.gameObject == l[1].finger.transform.parent.gameObject.transform.parent.gameObject))
			{
				if (!m_Selected && other.gameObject != m_FocusedObject)
	            {
	                //ClearFocus();
	                SetFocus(other.gameObject);
	            }
			}
        }
	}

	public void OnStoppedTouching(GameObject finger, Collider other)
	{
		Debug.Log ("OnStoppedTouching");
		/*int index = m_Touching.FindIndex(o => o == finger);
		if( index != -1 )
		{
			m_Touching.RemoveAt(index);
			m_LastPos.RemoveAt(index);
		}*/
		//we deal with changing focus in the update loop.
	}
	public void OnRayHit(RaycastHit info)
	{
			
	}
	
	public void ClearFocus()
	{
		if( m_FocusedObject != null )
		{
			List<Material> materials = new List<Material>( m_FocusedObject.renderer.materials );
			Material removeMaterial = materials.Find( m => m.name == m_HighlightMaterial.name + " (Instance)" );
			materials.Remove(removeMaterial);
			m_FocusedObject.renderer.materials = materials.ToArray();
			Destroy(removeMaterial); //cleanup instanced material;

			m_FocusedObject.rigidbody.isKinematic = false;
			m_FocusedObject.rigidbody.useGravity = true;

			LeapInput.DraggingMode = false;
		}


		m_FocusedObject = null;
		m_FirstTouchedTime = 0.0f;
		m_LastMovedTime = 0.0f;
		m_Selected = false;
		m_Touching.Clear();
		m_LastPos.Clear();
		m_ObjectTouched.Clear();

		LeapInput.DraggingFingers[0] = null;
		LeapInput.DraggingFingers[1] = null;
	}
	
	public void SetFocus(GameObject focus)
	{
		m_FocusedObject = focus;
		m_FirstTouchedTime = Time.time;
		m_LastMovedTime = Time.time + kMinSelectionTime;
		//Add the new material, but set it as blank so it doesn't really show up.
		List<Material> materials = new List<Material>( focus.renderer.materials );
		Material newMaterial = new Material(m_HighlightMaterial);
		newMaterial.color = new Color(0,0,0,0);
		materials.Add(newMaterial);
		focus.renderer.materials = materials.ToArray();

		LeapTouchable tcb = (LeapTouchable)focus.GetComponent(typeof(LeapTouchable));
		if (tcb != null)
			tcb.PreventOverlappingCollision = true;

		LeapInput.DraggingMode = true;

		LeapInput.DraggingFingers[0] = m_ObjectTouched[0].finger.transform.parent.gameObject;
		LeapInput.DraggingFingers[1] = m_ObjectTouched[1].finger.transform.parent.gameObject;
	}
	
	public void SetHighlightColor(Color c)
	{
		if( m_FocusedObject == true )
		{
			Material[] materials = m_FocusedObject.renderer.materials;
			Material changeMat = Array.Find(materials, m => m.name == m_HighlightMaterial.name + " (Instance)" );
			changeMat.color = c;
			m_FocusedObject.renderer.materials = materials;
		}
	}
	
	void Start()
	{
		m_HighlightMaterial = Resources.Load("Materials/Highlight") as Material;
	}
	
	protected GameObject m_FocusedObject = null;
	protected Leap.Frame m_LastFrame = null;

	//m_Touching maintains a list of fingers currently touching the focused object.
	//m_LastPos is the list of their last positions, used durring the update loop.
	protected List<GameObject> 	m_Touching = new List<GameObject>();
    protected GameObject m_Palm = null;
	protected List<Vector3>		m_LastPos = new List<Vector3>();
    protected Vector3 m_PalmLastPos = new Vector3();
	
	protected bool m_Selected = false;
	protected float m_FirstTouchedTime = 0.0f;
	protected float m_LastMovedTime = 0.0f;

	protected float m_FingerLostTime = 0.0f;
	protected bool oneFingerLost = false;
	
	protected const float kSelectionTime = .25f;
	protected const float kIdleStartDeselectTime = .5f;
	protected const float kMinSelectionTime = 2.0f;
	protected const float kMovementThreshold = 2.0f;
	protected Color kBlankColor = new Color(0,0,0,0);
	
	protected struct objectTouched
	{
		public GameObject gameObject;
		public GameObject finger;
	}

	// TODO: do not use m_Touching in the future ???
	protected List<objectTouched> m_ObjectTouched = new List<objectTouched>();
	private Material m_HighlightMaterial = null;
}
