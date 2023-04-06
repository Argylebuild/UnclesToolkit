using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	public class BoolWatcher
	{
		public bool PreviousState { get; private set; }
		private bool IsStateChanged { get; set; }

		public delegate bool Evaluation();
		public UnityEvent<bool> OnStateChangedTo = new UnityEvent<bool>();
		public UnityEvent OnStateChanged = new UnityEvent();
		
		
		/// <summary>
		/// Executed the evaluation method to determine the current state and whether it has changed since last check.
		/// </summary>
		public Evaluation EvaluationMethod;

		public BoolWatcher(Evaluation evaluationMethod, Looper looper = null)
		{
			EvaluationMethod = evaluationMethod;
			looper?.Add(Evaluate).StartLoop();
		}

		public void Evaluate()
		{
			var state = EvaluationMethod();
			
			IsStateChanged = state != PreviousState;
			PreviousState = state;

			if (IsStateChanged)
			{
				OnStateChangedTo.Invoke(state);
				OnStateChanged.Invoke();
			}
		}
		
		/// <summary>
		/// Check if state has changed since last check.
		/// NOTE: Does not evaluate the current state. Relies on Looping check or other evaluation trigger.
		/// For combined check use EvaluateAndCheck()
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			if (IsStateChanged)
			{
				PreviousState = !PreviousState;
				IsStateChanged = false;
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Evaluate current state and check if it has changed since last check.
		/// </summary>
		/// <returns></returns>
		public bool EvaluateAndCheck()
		{
			Evaluate();
			return Check();
		}
	}
}