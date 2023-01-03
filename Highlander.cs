using System.Collections.Generic;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    /// <summary>
    /// There can be only one. 
    /// </summary>
    public class Highlander : ArgyleComponent
    {
        public bool destroyFull = false;
        public bool destroyComponent = false;
        
        [SerializeField] private string id;
        public string Id
        {
            get
            {
                if (id == null)
                    id = GO.name;

                return id;
            }
        }

        
        /// <summary>
        /// When the revolution comes, these components will be the first to be destroyed.
        /// Use to prevent dependencies from throwing errors when components are destroyed. 
        /// </summary>
        public List<Component> _firstAgainstTheWall = new List<Component>();

        /// <summary>
        /// He save the children but...
        /// The poorly named list of children to preserve. Pass them in the purge. 
        /// </summary>
        public List<Transform> _notTheBritishChildren = new List<Transform>();
        /// <summary>
        /// He save the components but...
        /// The poorly named list of components to preserve. Pass them in the purge. 
        /// </summary>
        public List<Component> _notTheBritishComponents = new List<Component>();

        
        private void Awake()
        {
            var highlanders = FindObjectsOfType<Highlander>();

            foreach (var hl in highlanders)
            {
                if (hl.Id == Id && hl != this) //i'm the imposter!
                    ThereCanBeOnlyOne();
            }
        }

        /// <summary>
        /// Destroy or purge the extra stuff. 
        /// </summary>
        private void ThereCanBeOnlyOne()
        {
            if(destroyFull)
                Destroy(GO);
            else if(destroyComponent)
                Destroy(this);
            else
            {
                var children = GetComponentsInChildren<Transform>();
                var components = GetComponents(typeof(MonoBehaviour));

                foreach (var child in children)
                    if(!_notTheBritishChildren.Contains(child) && child != TForm)
                        Destroy(child.gameObject);

                foreach (var component in _firstAgainstTheWall)
                    Destroy(component);
                foreach (var component in components)
                    if(!_notTheBritishComponents.Contains(component))
                        Destroy(component);
                
            }
        }
    }
}
