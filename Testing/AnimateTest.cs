using Argyle.Utilities.Geometry;
using EasyButtons;
using UnityEngine;

namespace Argyle.Utilities.Testing
{
	public class AnimateTest : ArgyleComponent
	{
		public float time;
		public bool useLocal = true;

		#region Translation
		
		
		[Header("Translation")]
		public Vector3 translation;

		[Button]
		public void TestTranslate()
		{
			TForm.AnimateTranslate(translation, time, useLocal);
		}

		public Transform target;
		private Vector3 targetPosition;
		
		[Button]
		public void TestTranslateToTarget()
		{
			if (target != null)
				targetPosition = target.position;
			
			TForm.AnimateTranslateTo(targetPosition, time, useLocal);
		}
		
		

		#endregion

		#region Scale

		[Header("Scale")]
		public Vector3 _scaleFactor = Vector3.one;
		
		[Button]
		public void TestScale()
		{
			TForm.AnimateScale(_scaleFactor, time);
		}

		[Button]
		public void TestScaleTo()
		{
			TForm.AnimateScaleTo(target.localScale, time);
		}

		#endregion
	}
}