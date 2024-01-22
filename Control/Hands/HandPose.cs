using System;
using Argyle.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit.Control
{
	/// <summary>
	/// An abstraction to handle hand pose that can adapt to different platforms.
	/// It uses references to positions of various hand parts to trigger various events and statuses.
	/// </summary>
	public class HandPose : ArgyleComponent
	{
		#region ==== Options ====------------------

		[Header("Options")]
		public Side _side = Side.Right;
		[Tooltip("The angle between the palm and the camera to be considered palm up.")]
		public float _palmUpAngleThreshold = 45f;
		[Tooltip("The distance between the thumb and index finger to be considered a pinch.")]
		public float _pinchDistanceThreshold = .01f;

		#endregion -----------------/Options ====
		
		#region ==== Position References ====------------------

		
		#region == Reference Properties ==----
		
		public Transform PalmCenter;
		public Transform Wrist;
		public Transform Shoulder;

		#endregion ----/Reference Properties ==


		#region == Fingers ==----

		public Finger Thumb;
		public Finger Index;
		public Finger Middle;
		public Finger Ring;
		public Finger Pinky;

		#endregion ----/Fingers ==
		
		
		#endregion -----------------/Position References ====

		
		
		#region ==== Gestures ====------------------

		public Gesture PalmUpGesture = new Gesture();
		public Gesture PinchGesture = new Gesture();
		public Gesture PointGesture = new Gesture();
		public Gesture BirdGesture = new Gesture();
		public Gesture ThumbsUpGesture = new Gesture();
		public Gesture ThumbsDownGesture = new Gesture();
		

		#endregion -----------------/Gestures ====


		#region ==== Monobehavior ====------------------

		private void Start()
		{
			Thumb.Initialize(Pinky.Knuckle, Index.Knuckle, PalmCenter);
			Index.Initialize(Wrist, Middle.Knuckle, PalmCenter);
			Middle.Initialize(Wrist, Middle.Knuckle, PalmCenter);
			Ring.Initialize(Wrist, Middle.Knuckle, PalmCenter);
			Pinky.Initialize(Wrist, Middle.Knuckle, PalmCenter);
		}

		#endregion -----------------/Monobehavior ====
		

		#region ==== Tracking Loop ====------------------

		private void Update()
		{
			EvaluatePose();
		}

		private void EvaluatePose()
		{
			//check bool states
			PalmUpGesture.CheckGesture(() => 
				Vector3.Angle(PalmDirection, directionToCamera) < _palmUpAngleThreshold);
			PinchGesture.CheckGesture(() => 
				Vector3.Distance(Index.Tip.position, Thumb.Tip.position) < _pinchDistanceThreshold);
			PointGesture.CheckGesture(() => 
				Pinky.IsIn && Middle.IsIn && Index.IsOut);
			BirdGesture.CheckGesture(() => 
				Index.IsIn && Middle.IsOut && Pinky.IsIn);
			ThumbsUpGesture.CheckGesture(() => 
				Thumb.IsOut && Index.IsIn && Middle.IsIn && Pinky.IsIn && Vector3.Angle(Thumb.Direction, Vector3.up) < _palmUpAngleThreshold);
			ThumbsDownGesture.CheckGesture(() => 
				Thumb.IsOut && Index.IsIn && Middle.IsIn && Pinky.IsIn && Vector3.Angle(Thumb.Direction, Vector3.down) < _palmUpAngleThreshold);
		}
		
		
		[Header("Temporary Debugging")]
		public bool isPalmUp;
		public bool isPinching;
		public bool isPointing;
		public bool isBird;
		public bool isThumbsUp;
		public bool isThumbsDown;
		

		#endregion -----------------/Tracking Loop ====



		#region ==== Support ====------------------

		public enum Side
		{
			Left,
			Right
		}
		
		Vector3 directionToCamera => Vector3.Normalize(Reference.MainCameraTransform.position - PalmCenter.position);
		public Vector3 HandDirection => Vector3.Normalize(Middle.Knuckle.position - Wrist.position);
		public Vector3 ArmDirection => Vector3.Normalize(Wrist.position - Shoulder.position);
		/// <summary>
		/// Using cross product to get the ironman blast direction of the palm.
		/// Switch the cross order based on the side of the hand.
		/// </summary>
		Vector3 PalmDirection => _side == Side.Right ? 
			Vector3.Normalize(Vector3.Cross(Middle.Knuckle.position - PalmCenter.position, Thumb.Knuckle.position - PalmCenter.position)) :
			Vector3.Normalize(Vector3.Cross(Thumb.Knuckle.position - PalmCenter.position, Middle.Knuckle.position - PalmCenter.position));
		
		#endregion -----------------/Support ====
		
	}


	[Serializable]
	public class Gesture
	{
		public bool IsCurrent;
		public UnityEvent OnGestureStarted = new UnityEvent();
		public UnityEvent OnGestureStopped = new UnityEvent();

		public delegate bool GestureTestDelegate();

		public void CheckGesture(GestureTestDelegate test)
		{
			bool wasCurrent = IsCurrent;

			IsCurrent = test();
			
			//maybe trigger events
			if(IsCurrent && !wasCurrent)
				OnGestureStarted.Invoke();
			else if(!IsCurrent && wasCurrent)
				OnGestureStopped.Invoke();
		}
		
	}
}