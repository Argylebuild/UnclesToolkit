using System;
using System.Collections.Generic;
using UnityEngine;

namespace Argyle.UnclesToolkit.Control
{
	[RequireComponent(typeof(LineRenderer))]
	public class PointerCast : ArgyleComponent
	{
		[Header("Options")] [SerializeField] private LayerMask _layerMask;
		[SerializeField] private float _maxDistance = 10f;
		[SerializeField] private Color _hitColor = Color.green;
		[SerializeField] private Color _missColor = Color.grey;

		#region ==== Status ====------------------

		public PointerState State { get; private set; }
		public Vector3? HitPoint { get; private set; }
		public Vector3 EndPoint => HitPoint ?? TForm.position + TForm.forward * _maxDistance;
		
		public HashSet<PointerReceiver> HoverReceivers = new HashSet<PointerReceiver>();
		public HashSet<PointerReceiver> Click1Receivers = new HashSet<PointerReceiver>();
		public HashSet<PointerReceiver> Click2Receivers = new HashSet<PointerReceiver>();
		
		public bool IsClick1 { get; private set; }
		public bool IsClick2 { get; private set; }


		public PointerReceiver _lastReceiver;
		

		#endregion -----------------/Status ====


		#region ==== Reference ====------------------

		private LineRenderer Line
		{
			get
			{
				if(!_line)
				{
					_line = GetComponent<LineRenderer>();
					
				}

				_line.positionCount = 2;
				_line.startWidth = .01f;
				_line.endWidth = .01f;
				return _line;
			}
		}
		[SerializeField] private LineRenderer _line;

		#endregion -----------------/Reference ====


		#region ==== Button Input Methods ====------------------

		public void MarkClick1Start()
		{
			foreach (var receiver in HoverReceivers)
			{
				Click1Receivers.Add(receiver);
				receiver.SignalClick1Start();
			}
		}

		public void MarkClick1Stop()
		{
			foreach (var receiver in Click1Receivers)
			{
				receiver.SignalClick1Stop();
			}
			Click1Receivers.Clear();
			IsClick1 = false;
		}

		public void MarkClick2Start()
		{
			foreach (var receiver in HoverReceivers)
			{
				Click2Receivers.Add(receiver);
				receiver.SignalClick2Start();
			}
		}

		public void MarkClick2Stop()
		{
			foreach (var receiver in Click2Receivers)
			{
				receiver.SignalClick2Stop();
			}
			Click2Receivers.Clear();
			IsClick2 = false;
		}

		private void MarkHoverStop()
		{
			foreach (var receiver in HoverReceivers)
			{
				receiver.SignalHoverStop();
			}
			HoverReceivers.Clear();
			_lastReceiver = null;
		}

		#endregion -----------------/Input Receiver ====


		#region ==== Monobehavior ====------------------

		private void OnDisable()
		{
			ClearAll();
		}

		private void Update()
		{
			Ray ray = new Ray(TForm.position, TForm.forward * _maxDistance);
			TryRaycastHit(ray);
			
			
			foreach (var receiver in HoverReceivers)
			{
				PointerHitInfo hitInfo = new PointerHitInfo(this, TForm.position, EndPoint)
				{
					IsClick1 = IsClick1,
					IsClick1Start = IsClick1 && !Click1Receivers.Contains(receiver),
					IsClick2 = IsClick2,
					IsClick2Start = IsClick2 && !Click2Receivers.Contains(receiver)
				};
				receiver.SignalHover(hitInfo);
			}
			
			UpdateRender();
		}

		#endregion -----------------/Monobehavior ====

		private void TryRaycastHit(Ray ray)
		{
			RaycastHit hitInfo;
			
			if (Physics.Raycast(ray, out hitInfo, _maxDistance, _layerMask))
			{
				Debug.Log($"Hit {hitInfo.collider.gameObject.name}");
				
				PointerReceiver receiver = hitInfo.collider.GetComponent<PointerReceiver>();
				if (receiver != null)
				{
					State = PointerState.HitReceiver;

					
					if (_lastReceiver != null && receiver != _lastReceiver)
						MarkHoverStop();
					
					
					//add to receiver list to send events. 
					HoverReceivers.Add(receiver);

					_lastReceiver = receiver;
				}
				else
				{
					State = PointerState.HitBlank;
					MarkHoverStop();
				}

				HitPoint = hitInfo.point;
			}
			else
			{
				State = PointerState.Miss;
				HitPoint = null;
				MarkHoverStop();
			}
		}

		
		
		
		private void UpdateRender()
		{
			_line.SetPositions(new[] {TForm.position, EndPoint});

			Line.startColor = Color.grey;
			switch (State)
			{
				case PointerState.HitReceiver:
					Line.endColor = _hitColor;
					break;
				case PointerState.HitBlank:
					Line.endColor = (_hitColor + _missColor) / 2f;
					break;
				case PointerState.Miss:
					Line.endColor = _missColor;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		//get a clean slate to avoid bad states
		private void ClearAll()
		{
			MarkClick1Stop();
			MarkClick2Stop();
			MarkHoverStop();

			State = PointerState.Miss;
		}
		
	}
	
	

	public enum PointerState
	{
		HitReceiver,
		HitBlank,
		Miss
	}
	
}