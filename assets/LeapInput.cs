/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2013.                                   *
* Leap Motion proprietary and  confidential.  Not for distribution.            *
* Use subject to the terms of the Leap Motion SDK Agreement available at       *
* https://developer.leapmotion.com/sdk_agreement, or another agreement between *
* Leap Motion and you, your company or other organization.                     *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

/// <summary>
/// This static class serves as a static wrapper to provide some helpful C# functionality.
/// The main use is simply to provide the most recently grabbed frame as a singleton.
/// Events on aquiring, moving or loosing hands are also provided.  If you want to do any
/// global processing of data or input event dispatching, add the functionality here.
/// It also stores leap input settings such as how you want to interpret data.
/// To use it, you must call Update from your game's main loop.  It is not fully thread safe
/// so take care when using it in a multithreaded environment.
/// </summary>
public static class LeapInput 
{	
	public static bool EnableTranslation = true;
	public static bool EnableRotation = true;
	public static bool EnableScaling = false;
	
	//public static int NumberOfFingers = 0;
	
	// number of fingers of each hand
	public static int[] NumberOfFingers = new int[2];

	public static bool DraggingMode = false;
	public static GameObject[] DraggingFingers = new GameObject[2];
	
	/// <summary>
	/// Delegates for the events to be dispatched.  
	/// </summary>
	public delegate void PointableFoundHandler( Pointable p, bool visible );
	public delegate void PointableUpdatedHandler( Pointable p, bool visible );
	public delegate void HandFoundHandler( Hand h, bool visible );
	public delegate void HandUpdatedHandler( Hand h, bool visible );
	public delegate void ObjectLostHandler( int id );
	
	/// <summary>
	/// Event delegates are trigged every frame in the following order:
	/// Hand Found, Pointable Found, Hand Updated, Pointable Updated,
	/// Hand Lost, Hand Found.
	/// </summary>
	public static event PointableFoundHandler PointableFound;
	public static event PointableUpdatedHandler PointableUpdated;
	public static event ObjectLostHandler PointableLost;
	
	public static event HandFoundHandler HandFound;
	public static event HandUpdatedHandler HandUpdated;
	public static event ObjectLostHandler HandLost;
	
	public static Leap.Frame Frame
	{
		get { return m_Frame; }
	}
	
	public static void Update() 
	{	
		if( m_controller != null )
		{
			
			Frame lastFrame = m_Frame == null ? Frame.Invalid : m_Frame;
			m_Frame	= m_controller.Frame();
			m_device = m_controller.Devices[0];
			
			DispatchLostEvents(Frame, lastFrame);
			DispatchFoundEvents(Frame, lastFrame);
			DispatchUpdatedEvents(Frame, lastFrame);
		}
	}
	
	//*********************************************************************
	// Private data & functions
	//*********************************************************************
	private enum HandID : int
	{
		Primary		= 0,
		Secondary	= 1
	};
	
	//Private variables
	static Leap.Controller 		m_controller	= new Leap.Controller();
	static Leap.Device 			m_device		= null;
	static Leap.Frame			m_Frame			= null;
	
	private static void DispatchLostEvents(Frame newFrame, Frame oldFrame)
	{
		foreach( Hand h in oldFrame.Hands )
		{
			if( !h.IsValid )
				continue;
			if( !newFrame.Hand(h.Id).IsValid && HandLost != null )
				HandLost(h.Id);
		}
		foreach( Pointable p in oldFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( !newFrame.Pointable(p.Id).IsValid && PointableLost != null )
				PointableLost(p.Id);
		}
	}

	private static void DispatchFoundEvents(Frame newFrame, Frame oldFrame)
	{
		foreach (Hand h in newFrame.Hands) 
		{
			if (!h.IsValid)
				continue;
			if (!oldFrame.Hand (h.Id).IsValid && HandFound != null)
				HandFound (h, !HandCloseToBoundary(h));
		}
		foreach (Pointable p in newFrame.Pointables) 
		{
			if (!p.IsValid)
				continue;
			if (!oldFrame.Pointable (p.Id).IsValid && PointableFound != null)
				PointableFound (p, !FingerCloseToBoundary(p) && !DraggingMode);
		}
	}

	private static void DispatchUpdatedEvents(Frame newFrame, Frame oldFrame)
	{
		foreach( Hand h in newFrame.Hands )
		{
			if( !h.IsValid )
				continue;
			if( oldFrame.Hand(h.Id).IsValid && HandUpdated != null)
				HandUpdated(h, !HandCloseToBoundary(h));
		}
		foreach( Pointable p in newFrame.Pointables )
		{
			if( !p.IsValid )
				continue;
			if( oldFrame.Pointable(p.Id).IsValid && PointableUpdated != null)
				PointableUpdated(p, !FingerCloseToBoundary(p) && !DraggingMode);
		}
	}

	private static bool FingerCloseToBoundary(Pointable p)
	{
		if (m_device != null){
			float distanceToBoxWall = m_device.DistanceToBoundary(p.TipPosition);
			// TODO: make this changable through Unity?
			if (distanceToBoxWall < 5.0){
				return true;
			}
		}
		return false;
	}

	private static bool HandCloseToBoundary(Hand h)
	{
		if (m_device != null){
			float distanceToBoxWall = m_device.DistanceToBoundary(h.PalmPosition);
			// TODO: make this changable through Unity?
			if (distanceToBoxWall < 5.0){
				return true;
			}
		}
		return false;
	}
}
