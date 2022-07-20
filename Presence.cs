using UnityEngine;

namespace Argyle.Utilities
{
    public class Presence : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Toggle()
        {
            Toggle(gameObject);
        }
        public void Toggle(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }

    }
}
