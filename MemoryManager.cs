﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	public class MemoryManager : Manager<MemoryManager>
	{
		public float MemoryLimitGB = 4;
		const int GB = 1024 * 1024 * 1024;
		public static long MemoryLimit => (int) (Instance.MemoryLimitGB * GB);

		public static long MemoryUsed { get; private set; }

		public static float MemoryLimitUsed { get; private set; }
		
		
		
		public UnityEvent OnMemoryLimitReached = new UnityEvent();
		
		public UnityEvent OnUnityLowMemory = new UnityEvent();


		#region ==== Monobehavior ====------------------

		protected override void PostStart()
		{
			Application.lowMemory += UnityLowMemoryHandler;
			base.PostStart();
		}

		public float MemoryCheckFrequency = 5f;
		private float _lastCheckTime;
		private void Update()
		{
			if(Time.realtimeSinceStartup - _lastCheckTime > MemoryCheckFrequency)
			{
				CheckMemoryUsage();
			}
			
		}

		#endregion -----------------/Monobehavior ====

		private void CheckMemoryUsage()
		{
			MemoryUsed = GC.GetTotalMemory(false);
			MemoryLimitUsed = MemoryUsed / (float)MemoryLimit;

			_lastCheckTime = Time.realtimeSinceStartup;
			if (MemoryUsed > MemoryLimit)
			{
				OnMemoryLimitReached.Invoke();
			}
		}

		public void UnityLowMemoryHandler()
		{
			Debug.Log($"Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
			Resources.UnloadUnusedAssets();
			Debug.Log($"Finished Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
			OnUnityLowMemory.Invoke();
		}

		private async void ReportOnMemoryCleanup()
		{
			await UniTask.NextFrame();
			Debug.Log($"Frame after Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
		}
		
		
	}
}