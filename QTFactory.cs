using System;
using System.Collections.Generic;
using Argyle.glTF;
using Argyle.Utilities;
using Cysharp.Threading.Tasks;

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


		private QtMachine _machine;
		
		public bool IsRunning { get; private set; } = false;
		public bool IsAsync { get; }

		#endregion ------------------/Properties ====



		#region ==== CTOR ====-----------------

		/// <summary>
		/// Constructor for main thread operations (E.G. monobehavior operations)
		/// </summary>
		/// <param name="iterationMethod">The stuff you do to it. </param>
		public QTFactory(IterationMethod iterationMethod)
		{
			_iterationMethod = iterationMethod;
			IsAsync = false;
			_machine = new QtMachine(this);
		}

		/// <summary>
		/// Constructor for multithreaded async operations (E.G. i/o, network, heavy calculations)
		/// </summary>
		/// <param name="queue">QT queue of objects to iterate over according to...</param>
		/// <param name="iterationMethodAsync">The stuff you do to it. </param>

		public QTFactory(IterationMethodAsync iterationMethodAsync)
		{
			_iterationMethodAsync = iterationMethodAsync;
			IsAsync = true;
			_machine = new QtMachine(this);
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


		#endregion ------------------/IO ====
		

		#region ==== Execution ====-----------------

		public void StartFactory()
		{
			_machine.MachineLoop();
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

			public QtMachine(QTFactory<T> factory)
			{
				_factory = factory;
			}

			public async void MachineLoop()
			{
				if(_factory.IsRunning)
					return;
				_factory.IsRunning = true;

				while (_factory.IsRunning)
				{
					if(_factory.Queue.Count > 0)
					{
						if (_factory.IsAsync)
						{ await _factory._iterationMethodAsync(_factory.Queue.Dequeue()); }
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
		}
	}
}