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
	public class QTFactory<T> where T : class
	{

		#region ==== Properties ====-----------------
		private Queue<T> Queue = new Queue<T>();

		
		public delegate void IterationMethod(T thing);
		public delegate UniTask IterationMethodAsync(T thing);

		private IterationMethod _iterationMethod;
		private IterationMethodAsync _iterationMethodAsync;


		private QtMachine[] _machines;
		private int _machineQty;
		
		public bool IsRunning { get; private set; } = false;
		public bool IsAsync { get; }

		#endregion ------------------/Properties ====



		#region ==== CTOR ====-----------------

		/// <summary>
		/// Constructor for main thread operations (E.G. monobehavior operations)
		/// </summary>
		/// <param name="iterationMethod">The stuff you do to it. </param>
		public QTFactory(IterationMethod iterationMethod, int machinesQty)
		{
			_iterationMethod = iterationMethod;
			IsAsync = false;
			_machineQty = machinesQty;
			SetupMachines();
		}

		/// <summary>
		/// Constructor for multithreaded async operations (E.G. i/o, network, heavy calculations)
		/// </summary>
		/// <param name="iterationMethodAsync">The stuff you do to it. </param>
		/// <param name="machinesQty">How many parallel machines will run this async method?</param>
		public QTFactory(IterationMethodAsync iterationMethodAsync, int machinesQty)
		{
			_iterationMethodAsync = iterationMethodAsync;
			IsAsync = true;
			_machineQty = machinesQty;
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


		#region ==== IO ====-----------------

		/// <summary>
		/// Adds to the queue only if the thing isn't already in it. 
		/// </summary>
		/// <param name="thing"></param>
		public void AddSafe(T thing)
		{
			if(!Queue.Contains(thing))
				Queue.Enqueue(thing);
		}

		/// <summary>
		/// Add an entire collection. Sorted by floats. 
		/// </summary>
		/// <param name="things"></param>
		public void AddSorted(ICollection<T> things)
		{
			Queue = new Queue<T>();
			foreach (var thing in things)
			{
				Queue.Enqueue(thing);
			}
		}

		/// <summary>
		/// If elements no longer belong in queue, shortcut to remove all.
		/// O(nq * nb) at least so use sparingly.
		/// </summary>
		/// <param name="banned"></param>
		public void RemoveList(SafeHashset<T> banned)
		{
			if(Queue.Count > 0 && banned.Count > 0)
			{
				Queue<T> culled = new Queue<T>();
				var thing = Queue.Dequeue();
				if (!banned.Contains(thing))
					culled.Enqueue(thing);

				Queue = culled;
			}		
		}

		public void ResetFactory()
		{
			StopFactory();
			DestroyMachines();
			SetupMachines();
			Queue = new Queue<T>();
			StartFactory();
		}

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

		public void Report(QtMachine machine, T thing)
		{
			string methodName = IsAsync ? 
				_iterationMethodAsync.Method.Name : _iterationMethod.Method.Name;
			
			Debug.Log($"QtFactory machine {machine.Name}: acting on {thing} \n" +
			          $"out of {_machines.Length} machines.");
		}

		public int Count => Queue.Count;
		
		#endregion ------------------/IO ====
		

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
			Queue = new Queue<T>();
		}

		#endregion ------------------/Execution ====

		
		
		
		
		
		public class QtMachine
		{
			private QTFactory<T> _factory;

			public string Name { get; }
			private int index;
			
			
			public QtMachine(QTFactory<T> factory, int index)
			{
				Name = factory.IsAsync ? 
					factory._iterationMethodAsync.Method.Name : factory._iterationMethod.Method.Name;
				Name += $" - {index}";

				this.index = index;
				
				_factory = factory;
			}

			public async void MachineLoop()
			{
				while (_factory.IsRunning && Application.isPlaying)
				{
					//_factory.Report(this, null);
					if(_toKill)
					{
						Debug.Log("Killing the machine loop! ");
						_toKill = false;
						continue;
					}
					if(_factory.Queue.Count > 0)
					{
						if (_factory.IsAsync)
						{
							var thing = _factory.Queue.Dequeue();
							await _factory._iterationMethodAsync(thing);
							_factory.Report(this, thing);
						}
						else
						{
							Stopwatch frameWatch = new Stopwatch();

							_factory._iterationMethod(_factory.Queue.Dequeue());

							if (frameWatch.LapSoFar() > 1 / Timing.Instance.MinFramerate)
							{
								frameWatch.Lap();
								await UniTask.NextFrame();
							}
						}
					}

					await UniTask.NextFrame();
				}
			}
			
			
			private bool _toKill;

			public void Kill()
			{
				_toKill = true;
			}
		}
	}
}