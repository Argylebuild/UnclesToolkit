// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

//copied from Microsoft.MixedReality.Toolkit.Examples.Demos

namespace Argyle.UnclesToolkit
{
    [AddComponentMenu("Scripts/MRTK/Examples/LaunchUrl")]
    public class LaunchUrl : MonoBehaviour
    {
        /// <summary>
        /// Launch a UWP slate app. In most cases, your experience can continue running while the
        /// launched app renders on top.
        /// </summary>
        /// <param name="url">Url of the web page or app to launch. See https://docs.microsoft.com/windows/uwp/launch-resume/launch-default-app
        /// for more information about the protocols that can be used when launching apps.</param>
        public void Launch(string url)
        {
            Debug.Log($"LaunchUrl: Launching {url}");

#if UNITY_WSA
            UnityEngine.WSA.Launcher.LaunchUri(url, false);
#else
            Application.OpenURL(url);
#endif
        }
    }
}
