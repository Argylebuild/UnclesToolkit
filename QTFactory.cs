using System;
using System.Collections.Generic;
using Argyle.glTF;
using Argyle.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// Pronounced Cutie Factory. EG "Argyle is a cutie factory! Only cuties work there!"
	/// NOTE: If you're reading this, THIS MEANS YOU!
	/// Create one or more channels to handle all members of a queue in non-blocking parallel. 
	/// </summary>
	public class QtFactory<T> where T : class
	{

		#region ==== Properties ====-----------------
		private Queue<T> _queue = new Queue<T>();

		
		public delegate void IterationMethod(T thing);
		public delegate UniTask IterationMethodAsync(T thing);

		private readonly IterationMethod _iterationMethod;
		private readonly IterationMethodAsync _iterationMethodAsync;


		private QtMachine[] _machines;
		private readonly int _machineQty;
		private int _maxIterations = 100;
		
		public bool IsRunning { get; private set; } = false;
		public bool IsAsync { get; }

		#endregion ------------------/Properties ====



		#region ==== CTOR ====-----------------

		/// <summary>
		/// Constructor for main thread operations (E.G. monobehavior operations)
		/// </summary>
		/// <param name="iterationMethod">The stuff you do to it. </param>
		/// <param name="machinesQty"></param>
		/// <param name="maxIterations"></param>
		public QtFactory(IterationMethod iterationMethod, int machinesQty, int maxIterations = Int32.MaxValue)
		{
			_iterationMethod = iterationMethod;
			IsAsync = false;
			_machineQty = machinesQty;
			_maxIterations = maxIterations;
			SetupMachines();
		}

		/// <summary>
		/// Constructor for multithreaded async operations (E.G. i/o, network, heavy calculations)
		/// </summary>
		/// <param name="iterationMethodAsync">The stuff you do to it. </param>
		/// <param name="machinesQty">How many parallel machines will run this async method?</param>
		/// <param name="maxIterations"></param>
		public QtFactory(IterationMethodAsync iterationMethodAsync, int machinesQty, int maxIterations = Int32.MaxValue)
		{
			_iterationMethodAsync = iterationMethodAsync;
			IsAsync = true;
			_machineQty = machinesQty;
			_maxIterations = maxIterations;
			SetupMachines();
		}

		
		private void SetupMachines()
		{
			_machines = new QtMachine[_machineQty];
			for (int i = 0; i < _machineQty; i++)
			{
				_machines[i] = new QtMachine(this, i);
			}

		}

		#endregion ------------------/CTOR ====


		#region ==== Management ====-----------------

		/// <summary>
		/// Adds to the queue only if the thing isn't already in it. 
		/// </summary>
		/// <param name="thing"></param>
		public void AddSafe(T thing)
		{
			if(!_queue.Contains(thing))
				_queue.Enqueue(thing);
		}

		/// <summary>
		/// Add an entire collection. Sorted by floats. 
		/// </summary>
		/// <param name="things"></param>
		public void AddSorted(ICollection<T> things)
		{
			_queue = new Queue<T>();
			foreach (var thing in things)
			{
				_queue.Enqueue(thing);
			}
		}

		/// <summary>
		/// If elements no longer belong in queue, shortcut to remove all.
		/// WARNING: O(nq * nb) at least so use sparingly.
		/// </summary>
		/// <param name="banned"></param>
		public void RemoveList(SafeHashset<T> banned)
		{
			if(_queue.Count > 0 && banned.Count > 0)
			{
				Queue<T> culled = new Queue<T>();
				var thing = _queue.Dequeue();
				if (!banned.Contains(thing))
					culled.Enqueue(thing);

				_queue = culled;
			}		
		}

		public void ResetFactory()
		{
			StopFactory();
			DestroyMachines();
			SetupMachines();
			_queue = new Queue<T>();
			StartFactory();
		}

		/// <summary>
		/// Stop and remove referene to factory machines.
		/// Releases for garbage collector. 
		/// </summary>
		private void DestroyMachines()
		{
			if(_machines.Length == _machineQty)
			{
				for (int i = 0; i < _machineQty; i++)
				{
					_machines[i]?.Kill();
					_machines[i] = null;
				}
			}		
		}

		
		#endregion ------------------/Management ====
		

		#region ==== Execution ====-----------------

		public void StartFactory()
		{
			IsRunning = true;
			foreach (var machine in _machines)
			{
				machine.MachineLoop();	
			}
		}

		public void StopFactory()
		{
			IsRunning = false;
			_queue = new Queue<T>();
		}

		#endregion ------------------/Execution ====





		#region ==== Support ====-----------------

		private class QtMachine
		{
			private QtFactory<T> _factory;

			public string Name { get; }
			private int _index;
			
			
			public QtMachine(QtFactory<T> factory, int index)
			{
				Name = factory.IsAsync ? 
					factory._iterationMethodAsync.Method.Name : factory._iterationMethod.Method.Name;
				Name += $" - {index}";

				this._index = index;
				
				_factory = factory;
			}

			public async void MachineLoop()
			{
				// if (_factory.IsAsync)
				// 	UniTask.SwitchToThreadPool();
				// else
				// 	UniTask.SwitchToMainThread();
				
				Stopwatch frameWatch = new Stopwatch();

				int iterationsPerFrame = 0;
				while (_factory.IsRunning && Application.isPlaying)
				{
					if(_toKill)
					{
						//Debug.Log("Killing the machine loop! ");
						_toKill = false;
						continue;
					}

					if(_factory._queue.Count > 0)
					{
						if (_factory.IsAsync)
							await _factory._iterationMethodAsync(_factory._queue.Dequeue());
						else
							_factory._iterationMethod(_factory._queue.Dequeue());
							
						iterationsPerFrame++;
						if (frameWatch.LapSoFar() > 1 / Timing.Instance.MinFramerate ||
						    iterationsPerFrame > _factory._maxIterations)
						{
							//Debug.Log($"Iterations per frame: {iterationsPerFrame}");
							iterationsPerFrame = 0;
							await UniTask.Yield();
							frameWatch.Lap();
						}
					}
					else
					{
						await UniTask.Yield();
					}
				}
			}
			
			
			private bool _toKill;

			public void Kill()
			{
				_toKill = true;
			}
		}

		#endregion ------------------/Support ====
	}
}