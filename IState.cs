namespace Argyle.UnclesToolkit
{
    public interface IState
    {
        void OnEnter();
        void OnTick();
        void OnExit();
    }
}