using System;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

namespace Argyle.Utilities
{
	public class Timing : Manager<Timing>
	{
		public float MinFramerate = 20;
		public float MaxFramerate = 90;

		float frameStartTime;

		// Use this for initialization
		void Start()
		{
			 frameStartTime = Time.realtimeSinceStartup;
			// CalculateAverageFrameTimeAsync();
			 CalculateFps();
			 Application.targetFrameRate = 60;
		}

		// Update is called once per frame
		void Update()
		{
			frameStartTime = Time.realtimeSinceStartup;
		}

		/// <summary>
		/// Frames per second. Recalculated once per second. Usable in dynamic performance adjustment.
		/// </summary>
		public float Fps => fps;
		private float fps = 20;

		private async UniTaskVoid CalculateFps()
		{
			float calcLoopStart = Time.realtimeSinceStartup;
			float timeSinceCalc = Time.realtimeSinceStartup - calcLoopStart;
			int framesSinceCalc = 0;
			
			while (true)
			{
				while (timeSinceCalc < 1)
				{
					timeSinceCalc = Time.realtimeSinceStartup - calcLoopStart;
					framesSinceCalc++;
					await UniTask.NextFrame();
				}
		
				fps = framesSinceCalc;
				timeSinceCalc = 0;
				framesSinceCalc = 0;
				calcLoopStart = Time.realtimeSinceStartup;
			}
		}
		
		
	
		// public float GetTimeSinceFrameStart()
		// {
		// 	return Time.realtimeSinceStartup - frameStartTime;
		// }

		// public async Task NextFrame()
		// {
		// 	var lastFrameStartTime = frameStartTime;
		// 	while (frameStartTime == lastFrameStartTime)
		// 	{
		// 		await Task.Delay(10);
		// 	}
		// }

		[SerializeField] private int framerateThrottleCount;
		[SerializeField] private int frameratePassCount;
		public async UniTask MaintainFramerate()
		{
			//yield control to maintain framerate
			if (Time.deltaTime > 1/MinFramerate)
			{
				framerateThrottleCount++;
				Debug.Log("MAINTAIN FRAMERATE (see stack trace");
				await UniTask.NextFrame();
			}
			else
			{
				frameratePassCount++;
			}
		}

		
		
	}
}