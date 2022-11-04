using System;
using Argyle.UnclesToolkit.Geometry;
using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class DriftSimulator : ArgyleComponent
	{
		[Range(0, 1f)]
		public float driftiness = .05f;

		public bool _useInBuild = false;


		private Vector3 lastCameraPosition;

		private void Start()
		{
			lastCameraPosition = Reference.MainCameraTransform.position;
		}


		private void Update()
		{
			if(!Application.isEditor && !_useInBuild)
				return;
			
			float distance = Vector3.Distance(Reference.MainCameraTransform.position, lastCameraPosition) * driftiness * driftiness;
			lastCameraPosition = Reference.MainCameraTransform.position;
			
			Vector3 direction = Vector3Utility.Random();
			TForm.Translate(direction * distance);
		}
	}
}