using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// This hashset can be used in foreach loops without risk of exception because the set changes mid loop.
	/// loop checks out before starting, then checks in when finished. Any change to the loop must happen following WaitForReady.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SafeHashset<T> : HashSet<T>
	{
		private HashSet<T> _bufferAdd = new HashSet<T>();
		private HashSet<T> _bufferRemove = new HashSet<T>();
		
		public bool Busy { private set; get; }

		public async UniTask CheckOut()
		{
			if(Busy)
				await WaitForReady();
			Busy = true;
		}

		public async void CheckIn()
		{
			await UniTask.RunOnThreadPool(() =>
			{
				foreach (var item in _bufferAdd)
					Add(item);
				_bufferAdd.Clear();

				foreach (var item in _bufferRemove)
					Remove(item);
				_bufferRemove.Clear(); 
			});
			
			Busy = false;
		}


		public async UniTask WaitForReady()
		{
			while (Busy)
				await UniTask.Yield();
		}

		public void SafeAdd(T item)
		{
			if(Busy)
				_bufferAdd.Add(item);
			else
				Add(item);
		}

		public void SafeRemove(T item)
		{
			if(Busy)
				_bufferRemove.Add(item);
			else
				Remove(item);
		}
	}
}
