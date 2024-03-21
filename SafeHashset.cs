using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class SafeHashset<T> : HashSet<T>
	{
		private HashSet<T> _bufferAdd = new HashSet<T>();
		private HashSet<T> _bufferRemove = new HashSet<T>();
        
		private bool _busy;

		public async UniTask CheckOut()
		{
			while (_busy)
			{
				await UniTask.Yield();
			}
			_busy = true;
		}

		public UniTask CheckIn()
		{
			ProcessBuffers();
			_busy = false;
			return UniTask.CompletedTask;
		}

		private void ProcessBuffers()
		{
			foreach (var item in _bufferAdd)
			{
				Add(item);
			}
			_bufferAdd.Clear();

			foreach (var item in _bufferRemove)
			{
				Remove(item);
			}
			_bufferRemove.Clear();
		}

		public void SafeAdd(T item)
		{
			if (_busy)
			{
				_bufferAdd.Add(item);
			}
			else
			{
				Add(item);
			}
		}

		public void SafeRemove(T item)
		{
			if (_busy)
			{
				_bufferRemove.Add(item);
			}
			else
			{
				Remove(item);
			}
		}
	}
}