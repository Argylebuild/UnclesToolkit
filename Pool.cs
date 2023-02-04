using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// Object pooling manager. Handles instantiation, collection, clearing.
	/// I almost called the class LifeGuard! Aren't you glad I'm so restrained??
	/// </summary>
	public class Pool
	{
		#region ==== Configuration ====------------------

		public GameObject Prefab { get; private set; }
		public int MinSize { get; private set; }
		private Transform _parent;

		#endregion -----------------/Configuration ====

		
		public Queue<GameObject> WaitingThings = new Queue<GameObject>();

		
		
		#region ==== CTOR,  Setup ====------------------

		public Pool(GameObject prefab, int size, Transform parent = null, bool buildAtStart = true)
		{
			Prefab = prefab;
			MinSize = size;
			_parent = parent;

			if (buildAtStart)
				BuildAsync();
		}

		public async void BuildAsync() => await BuildAwaitable();
		public async UniTask BuildAwaitable()
		{
			Stopwatch sw = new Stopwatch();
			for (int i = 0; i < MinSize; i++)
			{
				WaitingThings.Enqueue(GameObject.Instantiate(Prefab, _parent));
				await sw.NextFrameIfSlow();
			}
			Debug.Log($"Instantiated {MinSize} pool objects in {sw.TotalSoFar()} seconds");
		}

		#endregion -----------------/CTOR, Setup ====

		
		
		#region ==== CRUD ====------------------



		public GameObject GetNewOrUsed()
		{
			while (WaitingThings.Count > 0)
			{
				var thing = WaitingThings.Dequeue();
				if (thing) //to protect against exception when pooled objects are destroyed
					return thing;
			}

			return GameObject.Instantiate(Prefab);
		}

		public void Return(GameObject thing)
		{
			thing.transform.parent = _parent;
			WaitingThings.Enqueue(thing);
		}
		

		#endregion -----------------/CRUD ====
		
		
	}
}