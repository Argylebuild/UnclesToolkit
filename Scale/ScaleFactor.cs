using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Argyle
{
    [CreateAssetMenu(fileName = "scale", menuName = "Argyle/ScaleFactor", order = 1)]
    public class ScaleFactor : ArgyleScriptableObject
    {
        [Tooltip("How much to scale in multiplier. 1 means no scaling applied.")]
        [SerializeField] private float factor;
        /// <summary>
        /// How much to scale in multiplier. 1 means no scaling applied.
        /// </summary>
        public float Factor => factor;
        
    }
}