using System;
using UnityEngine;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	public class LoopEvent : ArgyleComponent
	{
		public float Interval = 1;
		public UnityEvent OnLoop = new UnityEvent();
		
		private float _lastLoopTime;
		private void Update()
		{
			if(Time.realtimeSinceStartup - _lastLoopTime > Interval)
			{
				OnLoop.Invoke();
				_lastLoopTime = Time.realtimeSinceStartup;
			}
		}
	}
}