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
		public bool Busy { private set; get; }

		public async UniTask CheckOut()
		{
			if(Busy)
				await WaitForReady();
			Busy = true;
		}

		public void CheckIn() => Busy = false;



		public async UniTask WaitForReady()
		{
			while (Busy)
				await UniTask.Delay(10);
		}

		public async void SafeAdd(T item)
		{
			if(Busy)
				await WaitForReady();
			Add(item);
		}

		public async void SafeRemove(T item)
		{
			if(Busy)
				await WaitForReady();
			Remove(item);
		}
	}
}
