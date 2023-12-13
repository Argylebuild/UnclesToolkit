using System.Collections.Generic;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// Since Unity's UnityEvents are decidedly non-ordered, this let's us explicitly call out in which order the methods are called.
	/// Each UnityEvent in the list can have one or more methods attached to it. The methods of one UnityEvent will be called before the methods of the next UnityEvent.
	/// Remember that there is no particular order WITHIN the unity event.  
	/// </summary>
	public class OrderedEvents : ArgyleComponent
	{
		public List<UnityEvent> Events = new List<UnityEvent>();
		
		public void InvokeAll()
		{
			foreach (UnityEvent unityEvent in Events)
			{
				unityEvent.Invoke();
			}
		}
	}
}