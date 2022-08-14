using System.Runtime.InteropServices;
using Argyle.Utilities;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
    /// <summary>
    /// Simple helper to jump objects around to predetermined locations. 
    /// </summary>
    public class Jumper : ArgyleComponent
    {
        public Vector3 PrimaryDestination = new Vector3();
        public bool useLocal;


        #region ==== Setup ====-----------------

        
        [Button]
        public void SetDestinationByPosition()
        {
            if (useLocal)
                PrimaryDestination = TForm.localPosition;
            else
                PrimaryDestination = TForm.position;
        }

        #endregion -----------------/Setup ====


        #region ==== Move ====---------------------
        
        [Button]
        public void MoveToX(bool inverse = false)
        {
            float x = inverse ? -PrimaryDestination.x : PrimaryDestination.x;
            if (useLocal)
                TForm.localPosition = new Vector3(x, TForm.localPosition.y, TForm.localPosition.z);
            else
                TForm.position = new Vector3(x, TForm.position.y, TForm.position.z);
        }
        [Button]
        public void MoveToY(bool inverse = false)
        {
            float y = inverse ? -PrimaryDestination.y : PrimaryDestination.y;
            if (useLocal)
                TForm.localPosition = new Vector3(TForm.localPosition.x, y, TForm.localPosition.z);
            else
                TForm.position = new Vector3(TForm.position.x, y, TForm.position.z);
        }
        [Button]
        public void MoveToZ(bool inverse = false)
        {
            float z = inverse ? -PrimaryDestination.z : PrimaryDestination.z;
            if (useLocal)
                TForm.localPosition = new Vector3(TForm.localPosition.x, TForm.localPosition.y, z);
            else
                TForm.position = new Vector3(TForm.position.x, TForm.position.y, z);
        }
        [Button]
        public void MoveToPosition(bool inverse = false)
        {
            MoveToX(inverse);
            MoveToY(inverse);
            MoveToZ(inverse);
        }


        #endregion -----------------/Move ====


    }
}
