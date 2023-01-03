namespace Argyle.UnclesToolkit
{
    public abstract class StateMachine<T> where T : IState
    {
        /// <summary>
        /// For subclasses, create a field/property of type T to represent the current state.
        /// Let state() return that field/property.
        /// </summary>
        public abstract T CurrentState();
    
        /// <summary>
        /// Check if the current state is equal to the passed argument.
        /// If yes, return.
        /// If no, set the current state t be equal to the passed argument.
        /// </summary>
        public abstract void SetCurrentState(T state);
    
        public virtual void TransitionTo(T state)
        {
            CurrentState()?.OnExit();
            SetCurrentState(state);
            CurrentState().OnEnter();
        }
        public virtual void Tick()
        {
            CurrentState().OnTick();
        }
    }
}