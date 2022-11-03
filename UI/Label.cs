using System;
using Argyle.UnclesToolkit;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Argyle.Utilities.UI
{
	/// <summary>
	/// Holds text and optionally moves it to follow a user (or other object) locally within boundaries.
	/// </summary>
	public class Label : ArgyleComponent
	{
		//inspector properties
		public ObjectReferenceMarker followReference;
		public TextMeshPro Text;
		public bool _beginFollowLoopOnStart = false;
		[Tooltip("If continuous, time in miliseconds between loop iterations.")]
		[Range(10, 10000)]
		public int loopTime = 100;

		
		//private fields
		private Vector3 _constraint;
		private Transform _parent;
		private bool _isFollowing = true;
		private bool _zeroHeight;
		private bool _isSetup;

		//Properties
		public Transform FollowTarget {
			get
			{
				if (_followTarget == null && followReference != null)
					_followTarget = followReference.Reference.TForm;

				if (_followTarget == null)
					Debug.LogError($"No object reference found for label on {name}");
				
				return _followTarget;
			}
		}

		private Transform _followTarget;


		#region === Monobehavior ===

		private void Start()
		{
			if (_beginFollowLoopOnStart)
				StartFollowInBoundsLoop();
		}

		private void OnEnable()
		{
			FollowWhenReadyAsync();
		}

		#endregion /Monobehavior ===
		
		

		/// <summary>
		/// Starts a loop that runs follow every frame. Requires parameters tobe already set. 
		/// </summary>
		public async UniTaskVoid StartFollowInBoundsLoop()
		{
			_isFollowing = true;
			while (_isFollowing)
			{
				await FollowWhenReadyAsync();
				await UniTask.Delay(loopTime);
				await UniTask.NextFrame();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="localExtents"></param>
		/// <param name="parent"></param>
		/// <param name="zeroHeight"></param>
		public void StartFollowInBoundsLoop(Vector3 localExtents, Transform parent, bool zeroHeight = true )
		{
			//setConditions (move to seperate)
			SetParameters(localExtents, parent, zeroHeight);

			StartFollowInBoundsLoop();
		}

		async UniTask FollowWhenReadyAsync()
		{
			while (_parent == null)
			{
				await UniTask.Delay(100);
			}
			
			FollowInBounds();
		}
		
		/// <summary>
		/// Single follow event. Expects parameters already set.
		/// </summary>
		public void FollowInBounds()
		{
			if(!_isSetup)
				return;
			
			Vector3 translatedTarget = _parent.InverseTransformPoint(FollowTarget.position);

			TForm.localPosition = new Vector3(
				Mathf.Clamp(translatedTarget.x, -_constraint.x, _constraint.x),
				_zeroHeight ? 0 : Mathf.Clamp(translatedTarget.y, -_constraint.y, _constraint.y),
				Mathf.Clamp(translatedTarget.z, -_constraint.z, _constraint.z)
			);
		}

		public void SetParameters(Vector3 localExtents, Transform parent, bool zeroHeight)
		{
			_constraint = localExtents;
			_parent = parent;
			_zeroHeight = zeroHeight;

			_isSetup = true;
		}

		public void StopFollowInBoundsLoop()
		{
			_isFollowing = false;
		}
	}
}