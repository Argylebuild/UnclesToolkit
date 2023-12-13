using System;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// Runs on the update, watching the movement of the transform from frame to frame.
	/// Measures velocity and acceleration in both translation and rotation.
	/// </summary>
	public class MovementStatWatcher : ArgyleComponent
	{
		#region ==== Public stat properties ====------------------

		/// <summary>
		/// Change in position (meters) per second. Including directional vector.
		/// For magnitude, use Velocity.magnitude.
		/// </summary>
		public Vector3 Velocity { get; private set; }
		/// <summary>
		/// Change in velocity (meters) per second. Including directional vector.
		/// For magnitude, use Acceleration.magnitude.
		/// </summary>
		public Vector3 Acceleration { get; private set; }
		/// <summary>
		/// Change in rotation (degrees) per second. Including directional vector.
		/// For magnitude, use AngularVelocity.magnitude.
		/// </summary>
		public Vector3 AngularVelocity { get; private set; }
		/// <summary>
		/// Change in angular velocity (degrees) per second. Including directional vector.
		/// For magnitude, use AngularAcceleration.magnitude.
		/// </summary>
		public Vector3 AngularAcceleration { get; private set; }

		#endregion -----------------/Public stat properties ====



		#region ==== Supporting private fields ====------------------

		private float _lastMeasure;
		private Vector3 _lastPosition;
		private Vector3 _lastEuler;
		private Vector3 _lastVelocity;
		private Vector3 _lastAngularVelocity;
		
		#endregion -----------------/Supporting private fields ====
		
		
		private void Update()
		{
			var time = Time.time;
			var deltaTime = time - _lastMeasure;
			_lastMeasure = time;
			
			//measure translation
			Velocity = (TForm.position - _lastPosition) / deltaTime;
			Acceleration = (Velocity - _lastVelocity) / deltaTime;
			
			//measure rotation
			AngularVelocity = (TForm.eulerAngles - _lastEuler) / deltaTime;
			AngularAcceleration = (AngularVelocity - _lastAngularVelocity) / deltaTime;
			
			//store last values
			_lastPosition = TForm.position;
			_lastVelocity = Velocity;
			_lastEuler = TForm.eulerAngles;
			_lastAngularVelocity = AngularVelocity;
		}
	}
}