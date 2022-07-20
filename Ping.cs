using EasyButtons;
using UnityEngine;

namespace Argyle.Utilities
{
	public class Ping : ArgyleComponent
	{
		[Button]
		public void Invoke(string caller = "")
		{
			Debug.Log($"{caller} Ping invoked.");
		}
	}
}
