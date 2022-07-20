using System;
using UnityEngine;

namespace Argyle.Utilities
{
    public class Awareness : ArgyleComponent
    {
        
        public enum Clarity
        {
            occluded, inconclusive, clear
        }
        
        public static Clarity IsClearShot(Vector3 targetPosition, out RaycastHit hit, LayerMask meatspaceLayer, Vector3 source)
        {
            Clarity clarity;
            
            Vector3 direction = (targetPosition - source) * 2;
            Ray ray = new Ray(source, direction);

            Debug.DrawRay(source, direction);
            if (Physics.Raycast(ray, out hit, 20, meatspaceLayer))
            {
                float targetDistance = Vector3.Distance(source, targetPosition);
                float hitDistance = Vector3.Distance(source, hit.point);


                if (hitDistance >= targetDistance)
                    clarity = Clarity.clear;
                else
                    clarity = Clarity.occluded;
            }
            else
                clarity = Clarity.inconclusive;


            return clarity;
        }

        public static Clarity IsClearShot(Vector3 targetPosition, LayerMask meatspaceLayer, Vector3 source) =>
            IsClearShot(targetPosition, out RaycastHit hit, meatspaceLayer,  source);

        public static float IsClearShotFloat(Vector3 targetPosition, LayerMask meatspaceLayer, Vector3 source)
        {
            Clarity clarity = IsClearShot(targetPosition, meatspaceLayer, source);

            switch (clarity)
            {
                case Clarity.occluded:
                    return 0;
                case Clarity.inconclusive:
                    return 1;
                case Clarity.clear:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        
    }
}
