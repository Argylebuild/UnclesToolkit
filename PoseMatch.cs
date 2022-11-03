using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    public class PoseMatch : MonoBehaviour
    {
        public Transform MatchTarget
        {
            get
            {
                if (matchTarget == null)
                {
                    if(targetReference != null)
                        matchTarget = ObjectReference.GetReference(targetReference).TForm;
            
                    if(matchTarget == null)
                        matchTarget = Reference.MainCameraTransform;
                }

                return matchTarget;
            }
        }
        [Tooltip("The object to match this object to. ")]
        public Transform matchTarget;

        public ObjectReferenceMarker targetReference;
    
        [Tooltip("If true, copy the local position, ignoring global." +
                 "If false, copy the global position, ignoring local. Except scale. :(")]
        public bool useLocal = true;

        public bool useLocalTarget = true;
        
        public bool runOnUpdate = true;

        [Tooltip("Move to match the position of the target. ")]
        public bool matchPosition = true;
        [Tooltip("Rotate to match the rotation of the target.")]
        public bool matchRotation = true;
        [Tooltip("NOTE: Does not work if useLocal is false.")]
        public bool matchScale = false;
    
    
    
        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnEnable()
        {
            
        }

        private void Update()
        {
            if(runOnUpdate)
                Match();
        }

        // Update is called once per frame
        public void Match()
        {
            if (useLocal)
            {
                if(matchPosition)
                    transform.localPosition = useLocalTarget ? MatchTarget.localPosition : MatchTarget.position;
                if(matchRotation)
                    transform.localRotation = useLocalTarget ? MatchTarget.localRotation : MatchTarget.rotation;
                if(matchScale)
                    transform.localScale = MatchTarget.localScale;
            }
            else
            {
                if(matchPosition)
                    transform.position = useLocalTarget ? MatchTarget.localPosition : MatchTarget.position;
                if(matchRotation)
                    transform.rotation = useLocalTarget ? MatchTarget.localRotation : MatchTarget.rotation;
            }
        }
    }
}
