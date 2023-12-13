using UnityEngine;
using UnityEngine.Events;

namespace Argyle.UnclesToolkit.Control
{
	public class PointerReceiver : ArgyleComponent
	{

		#region ==== Events ====------------------

		[Header("Hover")]
		public UnityEvent OnHoverStart;
		public UnityEvent OnHoverHold;
		public UnityEvent OnHoverStop;
		
		[Header("Click 1")]
		public UnityEvent OnClick1Start;
		public UnityEvent OnClick1Hold;
		public UnityEvent OnClick1Stop;
		
		[Header("Click2")]
		public UnityEvent OnClick2Start;
		public UnityEvent OnClick2Hold;
		public UnityEvent OnClick2Stop;

		private PointerHitInfo _lastHit;

		#endregion -----------------/Events ====

		#region ==== State====------------------

		public bool IsHovering { get; private set; }
		public bool IsClick1 { get; private set; }
		public bool IsClick2 { get; private set; }

		#endregion -----------------/State ====
		
		
		
		public void SignalHover(PointerHitInfo hitInfo)
		{
			_lastHit = hitInfo;
			
			if(IsHovering)
				OnHoverHold.Invoke();
			else
			{
				Debug.Log("Hover Start");
				OnHoverStart.Invoke();
			}
			
			IsHovering = true;
		}

		public void SignalHoverStop()
		{
			Debug.Log("Hover Stop");
			IsHovering = false;
			OnHoverStop.Invoke();
		}

		public void SignalClick1Start()
		{
			Debug.Log("Click 1 start");
			IsClick1 = true;
			OnClick1Start.Invoke();
		}

		public void SignalClick1Stop()
		{
			Debug.Log("Click 1 Stop");
			IsClick1 = false;
			OnClick1Stop.Invoke();
		}
		
		public void SignalClick2Start()
		{
			Debug.Log("Click 2 Start");
			IsClick2 = true;
			OnClick1Start.Invoke();
		}
		
		public void SignalClick2Stop()
		{
			Debug.Log("Click 2 Stop");
			IsClick2 = false;
			OnClick1Stop.Invoke();
		}
		
		
	}


	public class PointerHitInfo
	{
		public PointerCast Caster;
		
		public Vector3 StartPoint;
		public Vector3 HitPoint;

		public bool IsClick1;
		public bool IsClick1Start;

		public bool IsClick2;
		public bool IsClick2Start;
		
		public PointerHitInfo( PointerCast caster, Vector3 startPoint, Vector3 hitPoint)
		{
			Caster = caster;
			StartPoint = startPoint;
			HitPoint = hitPoint;
		}

	}
}