using Argyle.Events;
using Argyle.UnclesToolkit;

namespace Argyle.Utilities.UI
{
    public class ActivationWatcher : ArgyleComponent
    {

        public VoidEventChannelSO ThisCreated;
        public VoidEventChannelSO ThisDestroyed;
        public VoidEventChannelSO ThisEnabled;
        public VoidEventChannelSO ThisDisabled;


        // Start is called before the first frame update
        void Start()
        {
            if(ThisCreated != null)
            ThisCreated.RaiseEvent();
        }

        private void OnDestroy()
        {
            if(ThisDestroyed != null)
            ThisDestroyed.RaiseEvent();
        }

        private void OnEnable()
        {
            if(ThisEnabled )
            ThisEnabled.RaiseEvent();
        }

        private void OnDisable()
        {
            if(ThisDisabled)
            ThisDisabled.RaiseEvent();
        }
    }
}