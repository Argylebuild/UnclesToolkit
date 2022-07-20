using System;
using UnityEngine.Events;

namespace Argyle.Utilities
{
	public class MonoEvents : ArgyleComponent
	{
		public UnityEvent AwakeEvent = new UnityEvent();
		public UnityEvent StartEvent = new UnityEvent();
		public UnityEvent EnableEvent = new UnityEvent();
		public UnityEvent DisableEvent = new UnityEvent();
		public UnityEvent DestroyEvent = new UnityEvent();

		private void Awake()
		{
			AwakeEvent.Invoke();
		}

		private void Start()
		{
			StartEvent.Invoke();
		}

		private void OnEnable()
		{
			EnableEvent.Invoke();
		}

		private void OnDisable()
		{
			DisableEvent.Invoke();
		}

		private void OnDestroy()
		{
			DestroyEvent.Invoke();
		}
	}
}