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
		public Side _side;
		[Tooltip("The angle between the palm and the camera to be considered palm up.")]
		public float _palmUpAngleThreshold = 45f;
		[Tooltip("The distance between the thumb and index finger to be considered a pinch.")]
		public float _pinchDistanceThreshold = .01f;

		#endregion -----------------/Options ====
		
		#region ==== Position References ====------------------

		#region == Markers ==----

		//palm
		[Header("Hand Part References")]
		[SerializeField] private ObjectReferenceMarker _palmCenterMarker;
		[SerializeField] private ObjectReferenceMarker _wristMarker;
		[SerializeField] private ObjectReferenceMarker _shoulderMarker;

		//Fingers

		//thumb 
		[SerializeField] private ObjectReferenceMarker _thumbKnuckleMarker;
		[SerializeField] private ObjectReferenceMarker _thumbTipMarker;

		//index 
		[SerializeField] private ObjectReferenceMarker _indexKnuckleMarker;
		[SerializeField] private ObjectReferenceMarker _indexTipMarker;

		//middle
		[SerializeField] private ObjectReferenceMarker _middleKnuckleMarker;
		[SerializeField] private ObjectReferenceMarker _middleTipMarker;

		//pinky
		[SerializeField] private ObjectReferenceMarker _pinkyKnuckleMarker;
		[SerializeField] private ObjectReferenceMarker _pinkyTipMarker;

		#endregion ----/Markers ==

		#region == Reference Properties ==----

		public Transform PalmCenter => ObjectReference.GetReference(_palmCenterMarker).TForm;
		public Transform Wrist => ObjectReference.GetReference(_wristMarker).TForm;
		public Transform Shoulder => ObjectReference.GetReference(_shoulderMarker).TForm;
		
		public Transform ThumbKnuckle => ObjectReference.GetReference(_thumbKnuckleMarker).TForm;
		public Transform ThumbTip => ObjectReference.GetReference(_thumbTipMarker).TForm;
		
		public Transform IndexKnuckle => ObjectReference.GetReference(_indexKnuckleMarker).TForm;
		public Transform IndexTip => ObjectReference.GetReference(_indexTipMarker).TForm;
		
		public Transform MiddleKnuckle => ObjectReference.GetReference(_middleKnuckleMarker).TForm;
		public Transform MiddleTip => ObjectReference.GetReference(_middleTipMarker).TForm;
		
		public Transform PinkyKnuckle => ObjectReference.GetReference(_pinkyKnuckleMarker).TForm;
		public Transform PinkyTip => ObjectReference.GetReference(_pinkyTipMarker).TForm;
		
		
		#endregion ----/Reference Properties ==
		
		#endregion -----------------/Position References ====

		#region ==== Statuses ====------------------
		//TODO: After testing, make all these properties with private setters
		
		public bool IsPalmUp;
		public bool IsPinchOpen;
		public bool IsPinch;
		public bool IsPointing;
		public bool IsBird;
		public bool IsThumbsUp;
		public bool IsThumbsDown;
		
		//directions
		public Vector3 PalmDirection;
		public Vector3 ThumbDirection;
		public Vector3 IndexDirection;
		public Vector3 MiddleDirection;
		public Vector3 PinkyDirection;
		public Vector3 HandDirection;
		public Vector3 ArmDirection;

		#endregion -----------------/Statuses ====


		#region ==== Events Out ====------------------

		public UnityEvent OnPalmUpStart = new UnityEvent();
		public UnityEvent OnPalmUpStop = new UnityEvent();
		public UnityEvent OnPinchStart = new UnityEvent();
		public UnityEvent OnPinchStop = new UnityEvent();
		public UnityEvent OnPointStart = new UnityEvent();
		public UnityEvent OnPointStop = new UnityEvent();
		public UnityEvent OnBirdStart = new UnityEvent();
		public UnityEvent OnBirdStop = new UnityEvent();
		public UnityEvent OnThumbsUpStart = new UnityEvent();
		public UnityEvent OnThumbsUpStop = new UnityEvent();
		public UnityEvent OnThumbsDownStart = new UnityEvent();
		public UnityEvent OnThumbsDownStop = new UnityEvent();

		#endregion -----------------/Events Out ====

		#region ==== Gesture Architecture ====------------------

		public Gesture ThumbsDownGesture = new Gesture();

		#endregion -----------------/Gesture Architecture ====

		#region ==== Tracking Loop ====------------------

		private void Update()
		{
			EvaluatePose();
		}

		private void EvaluatePose()
		{
			//Set Directions
			PalmDirection = Vector3.Normalize(Vector3.Cross(ThumbKnuckle.position - PalmCenter.position, MiddleKnuckle.position - PalmCenter.position));
			ThumbDirection = Vector3.Normalize(ThumbTip.position - ThumbKnuckle.position);
			IndexDirection = Vector3.Normalize(IndexTip.position - IndexKnuckle.position);
			MiddleDirection = Vector3.Normalize(MiddleTip.position - MiddleKnuckle.position);
			PinkyDirection = Vector3.Normalize(PinkyTip.position - PinkyKnuckle.position);
			HandDirection = Vector3.Normalize(MiddleKnuckle.position - Wrist.position);
			ArmDirection = Vector3.Normalize(Wrist.position - Shoulder.position);
			
			//check bool states
			CheckPalmUp(IsPalmUp);
			
		}
		
		

		private void CheckPalmUp(bool wasPalmUp)
		{
			Vector3 directionToCamera = Vector3.Normalize(Reference.MainCameraTransform.position - PalmCenter.position);
			
			//use cross product to find palm direction
			if(Vector3.Angle(PalmDirection, directionToCamera) < _palmUpAngleThreshold)
			{
				IsPalmUp = true;
				if (!wasPalmUp)
					OnPalmUpStart.Invoke();
			}
			else
			{
				IsPalmUp = false;
				if (wasPalmUp)
					OnPalmUpStop.Invoke();
			}
		}

		private void CheckPinch(bool wasPinch)
		{
			if(Vector3.Distance(IndexTip.position, ThumbTip.position) < _pinchDistanceThreshold)
			{
				IsPinch = true;
				if (!wasPinch)
					OnPinchStart.Invoke();
			}
			else
			{
				IsPinch = false;
				if (wasPinch)
					OnPinchStop.Invoke();
			}
		}

		private void CheckPoint(bool wasPoint)
		{
			if(IsMiddleIn && IsPinkyIn && IsIndexOut)
			{
				IsPointing = true;
				if (!wasPoint)
					OnPointStart.Invoke();
			}
			else
			{
				IsPointing = false;
				if (wasPoint)
					OnPointStop.Invoke();
			}
		}
		
		private void CheckBird(bool wasBird)
		{
			if(IsMiddleOut && IsPinkyIn && IsPinkyIn)
			{
				IsBird = true;
				if (!wasBird)
					OnBirdStart.Invoke();
			}
			else
			{
				IsBird = false;
				if (wasBird)
					OnBirdStop.Invoke();
			}
		}
		
		private void CheckThumbsUp(bool wasThumbsUp)
		{
			bool isThumbs = IsThumbOut && IsIndexIn && IsMiddleIn && IsPinkyIn;
			
			if(isThumbs && Vector3.Angle(ThumbDirection, Vector3.up) < _palmUpAngleThreshold)
			{
				IsThumbsUp = true;
				if (!wasThumbsUp)
					OnBirdStart.Invoke();
			}
			else
			{
				IsThumbsUp = false;
				if (wasThumbsUp)
					OnBirdStop.Invoke();
			}
		}

		private void CheckThumbsDown(bool wasThumbsDown)
		{
			bool isThumbs = IsThumbOut && IsIndexIn && IsMiddleIn && IsPinkyIn;
			
			if(isThumbs && Vector3.Angle(ThumbDirection, Vector3.down) < _palmUpAngleThreshold)
			{
				IsThumbsDown = true;
				if (!wasThumbsDown)
					OnThumbsDownStart.Invoke();
			}
			else
			{
				IsThumbsDown = false;
				if (wasThumbsDown)
					OnThumbsDownStop.Invoke();
			}
		}
		

		#endregion -----------------/Tracking Loop ====



		#region ==== Support ====------------------

		public enum Side
		{
			Left,
			Right
		}
		
		bool IsMiddleOut => Vector3.Angle(MiddleDirection, HandDirection) < _palmUpAngleThreshold;
		bool IsMiddleIn => Vector3.Distance(MiddleTip.position, PalmCenter.position) < _pinchDistanceThreshold * 3;
		bool IsPinkyIn => Vector3.Distance(PinkyTip.position, PalmCenter.position) < _pinchDistanceThreshold * 3;
		bool IsIndexOut => Vector3.Angle(IndexDirection, HandDirection) < _palmUpAngleThreshold;
		bool IsIndexIn => Vector3.Distance(IndexTip.position, PalmCenter.position) < _pinchDistanceThreshold * 3;
		bool IsThumbOut => Vector3.Angle(ThumbDirection, HandDirection) > 80;

		#endregion -----------------/Support ====
		
	}


	[Serializable]
	public class Gesture
	{
		public bool IsCurrent;
		public UnityEvent OnGestureStarted = new UnityEvent();
		public UnityEvent OnGestureStopped = new UnityEvent();
	}
	
	
}