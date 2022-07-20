using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.Utilities
{
	public class PointAtCamera : ArgyleComponent
	{

		[Header("Behavior")] 
		public bool continuous;

		[Tooltip("If continuous, time in seconds between loop iterations.")]
		[Range(0, 10)]
		public float loopTime = .1f;
	
		[Header("Location")]
		public bool yAxisOnly = true;
		public bool reverse = false;

		[Header("Target")]
		private Transform target;

		public Transform Target
		{
			get
			{
				if (target == null)
				{
					if (targetOverride != null)
						target = ObjectReference.GetReference(targetOverride).TForm;
					else
						target = Reference.MainCameraTransform;
				}

				return target;
			}
		}
    
		[Tooltip("If empty, will point at main camera")]
		public ObjectReferenceMarker targetOverride;

		#region  == Monobehavior ==


		private void Awake()
		{
			PointLoop = new Looper("PointAtCamera: PointLoop",loopTime).Add(Point);
		}

		// Use this for initialization
		void Start () {
			Point();
		}
	
		private void OnEnable()
		{
			Point();

			if (continuous)
				PointLoop.StartLoop();
		}

		private void OnDisable()
		{
			PointLoop.StopLoop();
		}


		#endregion /monobehavior


		private Looper PointLoop;
	
		public void Point()
		{
			if(this == null)
				return;

			if (yAxisOnly)
			{
				TForm.LookAt(new Vector3(
					Target.position.x,
					TForm.position.y,
					Target.position.z));

				if (reverse)
				{
					TForm.Rotate(0f, 180f, 0f);
				}
			}
			else
			{
				Vector3 targetPosition = Target.position;
				if (reverse)
					targetPosition = TForm.position - (Target.position - TForm.position);
		    
				TForm.LookAt(targetPosition);
			}
		}

	}
}
