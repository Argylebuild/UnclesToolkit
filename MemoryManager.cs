using System;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	public class MemoryManager : Manager<MemoryManager>
	{
		public float MemoryLimitGB = 4;
		public static long MemoryLimit => (int) (Instance.MemoryLimitGB * 1024 * 1024 * 1024);

		public static long MemoryUsed => GC.GetTotalMemory(false);
		
		public static float MemoryLimitUsed => MemoryUsed / (float) MemoryLimit;
		
		
		
		public UnityEvent OnMemoryLimitReached = new UnityEvent();
		
		private void Update()
		{
			if (MemoryUsed > MemoryLimit)
			{
				OnMemoryLimitReached.Invoke();
			}
		}
	}
}