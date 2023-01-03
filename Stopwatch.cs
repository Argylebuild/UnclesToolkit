using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    class Stopwatch
    {
        private float startTime;
        private float lapsTotal = 0;
        private List<float> laps = new List<float>();
        public List<float> Laps => laps;

        public Stopwatch()
        {
            startTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Add a lap and return the time on that lap.
        /// </summary>
        /// <returns>The time spanned within the new lap. </returns>
        public float Lap()
        {
            float thisLap = LapSoFar();
            Laps.Add(thisLap);
            lapsTotal += thisLap;

            return thisLap;
        }

        public void Reset()
        {
            startTime = Time.realtimeSinceStartup;
            laps = new List<float>();
            lapsTotal = 0;
        }

        /// <summary>
        /// Get the total time on the clock so far.
        /// </summary>
        /// <returns>Total time, including all laps. </returns>
        public float TotalSoFar() => Time.realtimeSinceStartup - startTime;

        public float LapSoFar() => TotalSoFar() - lapsTotal;

        /// <summary>
        /// Don't let a dollar be waitin on dime! Don't be the reason the app is slowing beyond the min framerate.
        /// If this particular task is taking longer than a single fram should, holds and waits for next frame.
        /// Specifically setup for jobs with many actions that have to happen on the main thread. 
        /// </summary>
        /// <returns></returns>
        public async UniTask NextFrameIfSlow()
        {
            if (LapSoFar() > 1 / Timing.Instance.MinFramerate * .5 ) //.5 because there's other stuff to do.
            {
                Lap();
                await UniTask.NextFrame();
            }
        }
    }
}