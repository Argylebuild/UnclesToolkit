using System;
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

		protected void Start()
		{
			Application.lowMemory += UnityLowMemoryHandler;
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
			//.Log($"MEMORY MANAGER: Checking memory usage at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
			MemoryUsed = GC.GetTotalMemory(false);
			MemoryLimitUsed = MemoryUsed / (float)MemoryLimit;

			_lastCheckTime = Time.realtimeSinceStartup;
			if (MemoryUsed > MemoryLimit)
			{
				//Debug.Log($"MEMORY MANAGER: Memory limit reached at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
				OnMemoryLimitReached.Invoke();
			}
			//Debug.Log($"MEMORY MANAGER: Finished checking memory usage at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
		}

		public void UnityLowMemoryHandler()
		{
			//Debug.Log($"Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
			Resources.UnloadUnusedAssets();
			//Debug.Log($"Finished Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
			OnUnityLowMemory.Invoke();
			
			ReportOnMemoryCleanup();
		}

		private async UniTaskVoid ReportOnMemoryCleanup()
		{
			await UniTask.NextFrame();
			Debug.Log($"Frame after Unloading unused assets at {Time.realtimeSinceStartup} seconds. Memory used: {MemoryUsed / GB} GB");
		}
		
		
	}
}