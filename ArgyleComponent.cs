using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    public class ArgyleComponent : MonoBehaviour
    {
        private bool isReferenceSet = false;
        private Transform _tForm;

        protected void Awake()
        {
            WaitForPostStart();
        }

        private async void WaitForPostStart()
        {
            await UniTask.NextFrame();
            PostStart();
        }
        
        /// <summary>
        /// Runs after first frame (so after awake, onEnable, and start) for functions that rely on established setup.
        /// </summary>
        protected virtual async void PostStart()
        {
        }

        /// <summary>
        /// Pre-populated transform reference for improved performance.
        /// </summary>
        public Transform TForm
        {
            get
            {
                if(!isReferenceSet)
                    SetReferences();
                
                return _tForm;
            }
        }
        
        
        private GameObject _go;

        /// <summary>
        /// Pre-populated Gameobject reference for improved performance.
        /// </summary>
        public GameObject GO
        {
            get
            {
                if (!isReferenceSet)
                    SetReferences();

                return _go;
            }
        }


        private void SetReferences()
        {
            _tForm = transform;
            _go = gameObject;

            isReferenceSet = true;
        }
        
        /// <summary>
        /// A list of all transforms in hierarchy from this to top level.
        /// NOTE: It is recalculated each call so avoid calling in loops. 
        /// </summary>
        public List<Transform> Ancestry
        {
            get
            {
                _ancestry = new List<Transform>();

                Transform t = TForm;
                while (true)
                {
                    _ancestry.Add(t);
                    if (t.parent == null)
                        break;

                    t = t.parent;
                }

                return _ancestry;
            }
        }
        private List<Transform> _ancestry;

        /// <summary>
        /// Simple universal gameobject Toggle. 
        /// </summary>
        public void ToggleGameObject()
        {
            GO.SetActive(!GO.activeSelf);
        }

        /// <summary>
        /// Simple universal component toggle. 
        /// </summary>
        public void ToggleComponent()
        {
            enabled = !enabled;
        }
        
    }
}