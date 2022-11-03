using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	
	/// <summary>
	/// Reusable loop logic to run a collection of methods periodically. Able to be turned off and on.
	/// Directions for common use:
	/// 1. Instatiate object.
	/// 2. add callback methods in the Awake function using .Add
	/// 3. Start and stop the loop in the OnEnable and OneDisable or base on other triggers (like press/release)
	/// </summary>
	public class Looper
	{

		#region === Properties ===----------

		/// <summary>
		/// A label for the loop. Helps with debug, primarily.
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Time in seconds between loops. If zero, frames are used. 
		/// </summary>
		public float LoopDuration;

		/// <summary>
		/// Duration of each loop converted to int milliseconds.
		/// </summary>
		private int DurationMilliseconds => (int) (LoopDuration * 1000);

		/// <summary>
		/// Seconds to wait for the loop to stop before timing out. 
		/// </summary>
		public float TimeOut = 10;
		
		/// <summary>
		/// Delegate with no arguments that can be used to pass a method to the loop. 
		/// </summary>
		public delegate void IterationMethod();

		public delegate UniTask IterationMethodAsync();

		/// <summary>
		/// Methods passed to the looper to run every loop. 
		/// </summary>
		private HashSet<IterationMethod> _iterations = new HashSet<IterationMethod>();

		/// <summary>
		/// Async unitask methods passed to the looper to run every loop. 
		/// </summary>
		private HashSet<IterationMethodAsync> _iterationsAsync = new HashSet<IterationMethodAsync>();

		/// <summary>
		/// Controls whether to continue looping.
		/// </summary>
		public bool IsLooping {private set; get; }

		/// <summary>
		/// Reports when loop is successfully stopped. 
		/// </summary>
		private bool _isStopped = true;

		#region = Tracking =--------

		/// <summary>
		/// For referencing which method was last started. In case it fails or stalls in that loop. 
		/// </summary>
		string currentMethod;
		
		/// <summary>
		/// For referencing how many cycles have gone in present loop. 
		/// </summary>
		private int cycleNumber;

		#endregion /Tracking =--------

		#endregion /Properties ===----------




		#region === Constructors ===---------

		/// <summary>
		/// Constructor to build a basic looper. Must supply duration.
		/// </summary>
		/// <param name="duration">Time in seconds between loops.
		/// If 0, runs every frame.</param>
		public Looper(string name, float duration)
		{
			Name = name;
			LoopDuration = duration;
		}
		
		
		public Looper Add(IterationMethod method)
		{
			if(!_iterations.Contains(method))
				_iterations.Add(method);
			return this;
		}
		public Looper Add(IterationMethodAsync method) 
		{
			if(!_iterationsAsync.Contains(method))
				_iterationsAsync.Add(method);
			return this;
		} 
		public Looper Remove(IterationMethod method)
		{
			_iterations.Remove(method);
			return this;
		}
		public Looper Remove(IterationMethodAsync method)
		{
			_iterationsAsync.Remove(method);
			return this;
		}

		#endregion /Constructors ===---------


		#region === Controls ===------------

		public Looper StartLoop()
		{
			RunLoop();
			return this;
		}
		
		/// <summary>
		/// Start the loop immediately. Run asyncronously until stopped. 
		/// </summary>
		private async void RunLoop()
		{
			if(IsLooping)
				return;
			await WaitForStopped(); //prevents restarting too before finished stopping and ruining a wait. 

			cycleNumber++;

			IsLooping = true;
			_isStopped = false;
			while (IsLooping)
			{
				try //so if there is an error in one part of the loop, it waits then tries again.
				{
					foreach (var iteration in _iterations)
					{
						if(!Application.isPlaying || !IsLooping)
						{
							_isStopped = true;
							currentMethod = $"None. Last method: {currentMethod}";
							return;
						}			
						currentMethod = iteration.Method.Name;
						iteration();
					}				
					foreach (var iteration in _iterationsAsync)
					{
						if(!Application.isPlaying || !IsLooping)
						{
							_isStopped = true;
							currentMethod = $"None. Last method: {currentMethod}";
							return;
						}					
						currentMethod = iteration.Method.Name;
						await iteration();
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					_isStopped = true;
				}

				currentMethod = "None. Finished.";
				
				
				await DelayByDuration();
			}

			_isStopped = true;
		}

		private async UniTask DelayByDuration()
		{
			if (LoopDuration == 0)
				await UniTask.NextFrame();
			else
				await UniTask.Delay(DurationMilliseconds);
		}


		/// <summary>
		/// Finish current loop and don't start another. 
		/// </summary>
		public void StopLoop()
		{
			IsLooping = false;
		}
		

		#endregion /Controls ===------------

		
		

		#region Reporting

		public async UniTask WaitForStopped()
		{
			float startTime = Time.realtimeSinceStartup;
			while (!_isStopped)
			{
				if(Time.realtimeSinceStartup > startTime + TimeOut)
				{
					Debug.LogWarning($"Failed to properly stop {Name} loop. Quit after {TimeOut} seconds. " +
					                 $"May cause duplicate looping. \n " +
					                 $"Loop stopped at loop {cycleNumber} in method {currentMethod}.");
					break;
				}				
				
				await DelayByDuration();
			}		
		}

		#endregion
	}
}