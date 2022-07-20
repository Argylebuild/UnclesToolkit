using System;
using Argyle.Utilities.Geometry;
using EasyButtons;
using UnityEngine;

namespace Argyle.Utilities
{
	
	/// <summary>
	/// A component access to the work being done 
	/// </summary>
	public class AnimateTransform : ArgyleComponent
	{
		public float time = .5f;
		public bool useLocal = true;

		[Tooltip("A transform to use as a guide for transformations. If null, fallback vectors are used.")]
		public Transform leadTransform;
		[Tooltip("The transform being transformed. If null, this transform is used.")]
		public Transform targetTransform;

		public Transform TargetTransform
		{
			get
			{
				if (targetTransform == null)
					targetTransform = TForm;

				return targetTransform;
			}
		}

		private void Start()
		{
			_originalTranslation = useLocal ? TargetTransform.localPosition : TargetTransform.position;
			_originalScale = TargetTransform.localScale;
		}

		#region ==== Translation ====------------

		private Vector3 _originalTranslation;
		
		[Header("Translation")]
		public Vector3 _translationFallback;
		public Vector3 TargetTranslation
		{
			get
			{
				Vector3 targetTranslation;
				if (leadTransform == null)
					targetTranslation = _translationFallback;
				else
					targetTranslation = useLocal ? leadTransform.localPosition : leadTransform.position;

				return targetTranslation;
			}
		}
		
		
		[Button]
		public void TranslateBy() => TargetTransform.AnimateTranslate(TargetTranslation, time, useLocal);

		[Button]
		public void TranslateTo() => TargetTransform.AnimateTranslateTo(TargetTranslation, time, useLocal);
		
		[Button]
		public void TranslateBack() => TargetTransform.AnimateTranslateTo(_originalTranslation, time, useLocal);

		#endregion /Translation ====------------

		
		
		
		#region ==== Scale ====------------

		private Vector3 _originalScale;
		
		[Header("Scale")] 
		public Vector3 _scaleFactorFallback = Vector3.one;
		
		public Vector3 TargetScale
		{
			get
			{
				Vector3 targetScale;
				if (leadTransform == null)
					targetScale = _scaleFactorFallback;
				else
					targetScale = leadTransform.localScale;

				return targetScale;
			}
		}

		
		
		[Button]
		public void ScaleBy() => TargetTransform.AnimateScale(TargetScale, time);

		[Button]
		public void ScaleTo() => TargetTransform.AnimateScaleTo(TargetScale, time);

		[Button]
		public void ScaleBack() => TargetTransform.AnimateScaleTo(_originalScale, time);

		#endregion /Scale ====------------
	}
}