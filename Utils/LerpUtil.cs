using System;
using UnityEngine;

namespace Extensions.Utils
{
    /// <summary>
    /// Utility for managing time-based lerp animations that interpolate values over a fixed duration.
    /// Unlike dt-based damping, these use absolute time for consistent results regardless of frame rate.
    /// </summary>
    public static class LerpUtil
    {
        /// <summary>
        /// Tracks a lerp value between start and target over a duration using absolute time.
        /// </summary>
        [Serializable]
        public struct TimedLerp<T>
        {
            public float startTime;
            public T startValue;
            public T targetValue;
            public float duration;

            public TimedLerp(float duration, T startValue, T targetValue)
            {
                this.startTime = Time.time;
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.duration = duration;
            }

            /// <summary>
            /// Gets the interpolation progress (0 to 1) based on elapsed time.
            /// </summary>
            public float GetProgress()
            {
                float elapsed = Time.time - startTime;
                return Mathf.Clamp01(elapsed / duration);
            }

            /// <summary>
            /// Returns true if the lerp is complete.
            /// </summary>
            public bool IsComplete()
            {
                return GetProgress() >= 1f;
            }
        }

        /// <summary>
        /// Interpolates between two float values based on elapsed time.
        /// </summary>
        public static float LerpFloat(this TimedLerp<float> lerp)
        {
            return Mathf.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /// <summary>
        /// Interpolates between two int values based on elapsed time.
        /// </summary>
        public static int LerpInt(this TimedLerp<int> lerp)
        {
            return Mathf.RoundToInt(Mathf.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress()));
        }

        /// <summary>
        /// Interpolates between two Color values based on elapsed time.
        /// </summary>
        public static Color LerpColor(this TimedLerp<Color> lerp)
        {
            return Color.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /// <summary>
        /// Interpolates between two Vector2 values based on elapsed time.
        /// </summary>
        public static Vector2 LerpVector2(this TimedLerp<Vector2> lerp)
        {
            return Vector2.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /// <summary>
        /// Interpolates between two Vector3 values based on elapsed time.
        /// </summary>
        public static Vector3 LerpVector3(this TimedLerp<Vector3> lerp)
        {
            return Vector3.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /// <summary>
        /// Interpolates between two Vector4 values based on elapsed time.
        /// </summary>
        public static Vector4 LerpVector4(this TimedLerp<Vector4> lerp)
        {
            return Vector4.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /// <summary>
        /// Interpolates between two Quaternion values based on elapsed time.
        /// </summary>
        public static Quaternion LerpQuaternion(this TimedLerp<Quaternion> lerp)
        {
            return Quaternion.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }
    }
}

