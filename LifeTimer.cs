using System;
using Cysharp.Threading.Tasks;

namespace Argyle.UnclesToolkit
{
	public class LifeTimer : ArgyleComponent
	{
		public float lifespan = 1;
		public bool countOnStart = true;
		
		private void Start()
		{
			if(countOnStart)
				Count();
		}

		public async void Count()
		{
			await UniTask.Delay((int) lifespan * 1000);
			Destroy(GO);
		}
	}
}