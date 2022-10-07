using System;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;
using UnityEngine.Events;

namespace Argyle.Utilities.UI
{

	/// <summary>
	/// Independent activator component to allow toggling of an arbitrary gameobject
	/// By placing on object above or separate in the heirarchy, we avoid the problem of
	/// ...disabled objects failing to reactivate.
	/// </summary>
	public class Activator : ArgyleComponent
	{
		public GameObject _target;
		[SerializeField] private bool _activeOnStart;
	
		[Space(5f)]
		[Header("Auto Closing")]
	
		public bool _autoCloseByDistance;

		public float _autoCloseDistance;
		
		private Transform _closeByDistanceObject;
		public Transform CloseByDistanceTform
		{
			get
			{
				if (_closeByDistanceObject == null)
					_closeByDistanceObject = Reference.MainCameraTransform;

				return _closeByDistanceObject;
			}
		}
	
		public bool _autoCloseByTime;
	
		[Tooltip("How long in seconds to wait before deactivating the target. ")]
		public float _autoCloseTime;

		protected bool _isActive;


		#region Events

		
		public Events events = new Events();

		[Serializable]
		public class Events
		{
			public UnityEvent ActivatedEvent = new UnityEvent();
			public UnityEvent DeactivatedEvent = new UnityEvent();
		}

		#endregion


		#region === Monobehavior ===--------

		private void Start()
		{
			if(_activeOnStart)
				Activate();
			else
				Deactivate();
		}

		#endregion /Monobehavior ===--------
		
		[Button]
		public void Activate()
		{
			if(_target == null || !Application.isPlaying)
				return;

			if (_target.IsPrefab())
				_target = Instantiate(_target, TForm.position, TForm.rotation, TForm);
			_target.gameObject.name = "ControlPanelP";

			//Debug.Log($"Activating {_target.name}");
			_isActive = true;
			_target.SetActive(true);

			if (_autoCloseByTime)
				StartTimer();
			if (_autoCloseByDistance)
				StartDistanceCheck();

			events.ActivatedEvent.Invoke();
		}

		[Button]
		public void Deactivate()
		{
			if(_target == null || !Application.isPlaying)
				return;

			//Debug.Log($"Deactivating {_target?.name}");
			_target.SetActive(false);
			_isActive = false;

			events.DeactivatedEvent.Invoke();
		}

		public void Toggle()
		{
			if (_isActive)
				Deactivate();
			else
				Activate();
		}

		/// <summary>
		/// Starts testing the distance between the target and the main camera.
		/// When the time passed is longer than the autoCloseDistance, Deactivate() is called.
		/// </summary>
		/// <returns></returns>
		public async UniTask StartTimer() => await StartTimerAsync(_autoCloseTime);

		/// <summary>
		/// Waits the number of seconds specified then deactivates.
		/// </summary>
		/// <returns></returns>
		public async UniTask StartTimerAsync(float seconds)
		{
			int milliseconds = (int) (seconds * 1000);
			await UniTask.Delay(milliseconds);
			
			//Debug.Log($"{_target} time deactivation hit.");
			Deactivate();
		}

		/// <summary>
		/// Waits the number of seconds specified then deactivates.
		/// </summary>
		/// <param name="seconds"></param>
		public void StartTimer(float seconds) => StartTimerAsync(seconds);

		

		/// <summary>
		/// Starts testing the distance between the target and the main camera.
		/// When that distance is longer than the autoCloseDistance, Deactivate() is called.
		/// </summary>
		/// <returns></returns>
		public async UniTask StartDistanceCheck()
		{
			float distance = 0;
			while (distance < _autoCloseDistance)
			{
				await UniTask.Delay(1000);
				distance = Vector3.Distance(_target.transform.position, CloseByDistanceTform.position);
			}
			Deactivate();
		}

	}
}
