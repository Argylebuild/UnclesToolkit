namespace Argyle.Utilities
{
    public interface IState
    {
        void OnEnter();
        void OnTick();
        void OnExit();
    }
}