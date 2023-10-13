using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    public class ArgyleComponent : MonoBehaviour
    {
        private bool _isReferenceSet = false;
        private Transform _tForm;
        

        protected void OnDestroy()
        {
            CancelObject();
        }
        

        /// <summary>
        /// Pre-populated transform reference for improved performance.
        /// </summary>
        public Transform TForm
        {
            get
            {
                if(!_isReferenceSet)
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
                if (!_isReferenceSet)
                    SetReferences();

                return _go;
            }
        }


        private void SetReferences()
        {
            _tForm = transform;
            _go = gameObject;

            _isReferenceSet = true;
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

        
        
        
        #region ==== Cancellation ====-----------------

        private HashSet<CancellationTokenSource> CancelObjectSources = new HashSet<CancellationTokenSource>();
        
        

        /// <summary>
        /// Cancel async functions for this particular object (instance).
        /// Can access cancellation bia CancelObjectToken or CancelAny.
        /// </summary>
        public void CancelObject()
        {
            foreach (var source in CancelObjectSources)
            {
                source.Cancel();
            }
        }

        protected CancellationToken AddCancelToObject()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancelObjectSources.Add(source);

            return source.Token;
        }

        protected void ReleaseCancel(CancellationToken token)
        {
            foreach (var source in CancelObjectSources)
            {
                if (source.Token == token)
                {
                    CancelObjectSources.Remove(source);
                    return;
                }
            }
        }


        #endregion ------------------/Cancellation ====
    }
}