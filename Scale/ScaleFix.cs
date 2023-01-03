using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit.Scale
{
    public class ScaleFix : ArgyleComponent
    {
        [Tooltip("Drag in a ScaleFactor scriptable object asset to control scaling factor")] [field: SerializeField]
        public ScaleFactor Factor;

        [Tooltip("EG scaling up a camera transform in inverse makes the world larger.")]
        [SerializeField] private bool isInverse;
        /// <summary>
        /// EG scaling up a camera transform in inverse makes the world larger.
        /// </summary>
        public bool IsInverse => isInverse;
    
        [Tooltip("Select to cause scaling to happen continuously for testing purposes.")]
        [SerializeField] private bool isContinuous;
    
        // Start is called before the first frame update
        void Start()
        {
            ApplyScale();
        }

        private void Update()
        {
            if (isContinuous)
            {
                ApplyScale();
            }
        }

        [ContextMenu("Apply Scale")]
        void ApplyScale()
        {
            if (IsInverse)
            {
                TForm.localScale = Vector3.one / Factor.Factor;    
            }
            else
            {
                TForm.localScale = Vector3.one * Factor.Factor;
            }
        }
    }
}
