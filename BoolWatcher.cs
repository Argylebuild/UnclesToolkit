using UnityEngine.Events;

namespace Argyle.UnclesToolkit
{
	public class BoolWatcher
	{
		public bool LastState { get; private set; }
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

		/// <summary>
		/// Checks the current state agains the state from the last state evaluation.
		/// If changed, sets IsStateChanged to true and invokes OnStateChangedTo and OnStateChanged.
		/// If unchanged, leaves IsStateChanged alone.
		/// </summary>
		public void Evaluate()
		{
			var state = EvaluationMethod();
			
			//runs anytime state changes but leaves existing IsStateChanged = true values alone
			if(state != LastState)
			{
				IsStateChanged = true;
				
				if(OnStateChangedTo != null)
					OnStateChangedTo.Invoke(state);
				OnStateChanged.Invoke();
			}			
			LastState = state;
		}
		
		/// <summary>
		/// Check if state has changed since last check.
		/// NOTE: Does not evaluate the current state. Relies on Looping check or other evaluation trigger.
		/// For combined check use EvaluateAndCheck()
		/// </summary>
		/// <returns></returns>
		public bool Check()
		{
			if (!IsStateChanged) return false;
			
			IsStateChanged = false;
			return true;
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