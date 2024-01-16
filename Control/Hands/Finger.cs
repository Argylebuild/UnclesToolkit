using UnityEngine;

namespace Argyle.UnclesToolkit.Control
{
	[SerializeField]
	public class Finger : ArgyleComponent
	{
		[Header("Finger Parts")]
		public Transform Knuckle;

		public Transform Tip;

		[Header("Options")]
		[SerializeField] private readonly float _outAngleThreshold;

		[SerializeField] private readonly float _inDistanceThreshold;
		
		

		[Header("Populated By Script")]
		[SerializeField] private Transform _outCompareStart;
		[SerializeField] private Transform _outCompareEnd;
		[SerializeField] private Transform _inCompare;
		
		public Vector3 Direction => Vector3.Normalize(Tip.position - Knuckle.position);
		
		/// <summary>
		/// CTOR
		/// </summary>
		/// <param name="outAngleThreshold">How close the direction needs to match to count as out</param>
		/// <param name="outCompareStart">A transform that is used to compare the direction of this finger to. The start of the comparison vector.</param>
		/// <param name="outCompareEnd">A transform that is used to compare the direction of this finger to. The end of the comparison vector.</param>
		/// <param name="inDistanceThreshold"></param>
		/// <param name="inCompare"></param>
		public void Initialize(Transform outCompareStart, Transform outCompareEnd, Transform inCompare)
		{
			_outCompareStart = outCompareStart;
			_outCompareEnd = outCompareEnd;
			_inCompare = inCompare;
		}
		


		public bool IsOut => Vector3.Angle(Direction, _outCompareEnd.position - _outCompareStart.position) < _outAngleThreshold;
		public bool IsIn => Vector3.Distance(Tip.position, _inCompare.position) < _inDistanceThreshold;
		
	}
}