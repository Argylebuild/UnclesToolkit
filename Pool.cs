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
	public class Pool : ArgyleComponent
	{
		#region ==== Configuration ====------------------

		public GameObject Prefab;
		public int MinSize;
		private Transform _parent;
		public List<GameObject> Preload = new List<GameObject>();

		#endregion -----------------/Configuration ====

		public Queue<GameObject> WaitingThings = new Queue<GameObject>();
		
		
		
		#region ==== CTOR,  Setup ====------------------


		public async void BuildAsync() => await BuildAwaitable();
		public async UniTask BuildAwaitable()
		{
			Stopwatch sw = new Stopwatch();
			for (int i = 0; i < MinSize; i++)
			{
				if(i < Preload.Count)
					WaitingThings.Enqueue(Preload[i]);
				else
				{
					WaitingThings.Enqueue(GameObject.Instantiate(Prefab, _parent));
					await sw.NextFrameIfSlow();
				}			
			}

			Preload = new List<GameObject>();
			//Debug.Log($"Instantiated {MinSize} pool objects in {sw.TotalSoFar()} seconds");
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