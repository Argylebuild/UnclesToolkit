using Argyle.UnclesToolkit.Geometry;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    /// <summary>
    /// yeah, I know. It's a silly name. For the dumb joke's sake, forgive me. 
    /// </summary>
    public class Scorpion : ArgyleComponent
    {
        public Transform _targetTransform;
        public ObjectReferenceMarker _targeReference;
        public float distanceThreshold = 0f;
        public float rotationThreshold = 0f;
        
        public Transform Target
        {
            get
            {
                if (!_targetTransform)
                    _targetTransform = _targeReference.Reference.TForm;

                return _targetTransform;
            }
        }
        
        public float _animationTime;
        public bool _useLocal;
        public bool _matchRotation = true;

        
        
        [Button]
        public async void GetOverHere()
        {
            if(Target != null)
            {
                if(_matchRotation && Quaternion.Angle(TForm.rotation, Target.rotation) > rotationThreshold)
                    TForm.rotation = Target.rotation;// should animate this later. Add Transform.AnimateRotate extension.
                
                Vector3 targetPosition = _useLocal ? TForm.parent.InverseTransformPoint(Target.position) : Target.position;
                if(Vector3.Distance(TForm.position, targetPosition) > distanceThreshold)
                    await TForm.AnimateTranslateTo(targetPosition, _animationTime, _useLocal);
                
                if(_matchRotation && Quaternion.Angle(TForm.rotation, Target.rotation) > rotationThreshold)
                    TForm.rotation = Target.rotation;// should animate this later. Add Transform.AnimateRotate extension.
            }        
            
            
        }
    }
}
