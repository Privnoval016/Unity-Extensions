using System;
using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utilities for managing time-based lerp animations that interpolate values over a fixed duration.
     * Unlike delta-time-based damping, these use absolute time for consistent results regardless of frame rate.
     * </summary>
     */
    public static class LerpUtil
    {
        /**
         * <summary>
         * Tracks the state of a timed interpolation between a start and target value.
         * Uses absolute time to measure progress, making it frame-rate independent.
         * </summary>
         * <typeparam name="T">The type of value being interpolated.</typeparam>
         */
        [Serializable]
        public struct TimedLerp<T>
        {
            /**
             * <summary>
             * The time when the lerp was created.
             * </summary>
             */
            public float startTime;
            
            /**
             * <summary>
             * The value at the start of the lerp.
             * </summary>
             */
            public T startValue;
            
            /**
             * <summary>
             * The target value to lerp towards.
             * </summary>
             */
            public T targetValue;
            
            /**
             * <summary>
             * The total duration of the lerp in seconds.
             * </summary>
             */
            public float duration;

            /**
             * <summary>
             * Initializes a new <see cref="TimedLerp{T}"/> that interpolates from start to target over a duration.
             * </summary>
             * <param name="duration">The duration of the lerp in seconds.</param>
             * <param name="startValue">The initial value.</param>
             * <param name="targetValue">The target value to interpolate towards.</param>
             */
            public TimedLerp(float duration, T startValue, T targetValue)
            {
                this.startTime = Time.time;
                this.startValue = startValue;
                this.targetValue = targetValue;
                this.duration = duration;
            }

            /**
             * <summary>
             * Gets the interpolation progress as a normalized value between 0 and 1.
             * </summary>
             * <returns>The progress (0 = start, 1 = complete).</returns>
             */
            public float GetProgress()
            {
                float elapsed = Time.time - startTime;
                return Mathf.Clamp01(elapsed / duration);
            }

            /**
             * <summary>
             * Determines whether the lerp animation has completed.
             * </summary>
             * <returns>true if the lerp has reached or exceeded its target duration; otherwise, false.</returns>
             */
            public bool IsComplete()
            {
                return GetProgress() >= 1f;
            }
        }

        /**
         * <summary>
         * Interpolates between two float values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated float value.</returns>
         */
        public static float LerpFloat(this TimedLerp<float> lerp)
        {
            return Mathf.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /**
         * <summary>
         * Interpolates between two int values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated int value (rounded to nearest integer).</returns>
         */
        public static int LerpInt(this TimedLerp<int> lerp)
        {
            return Mathf.RoundToInt(Mathf.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress()));
        }

        /**
         * <summary>
         * Interpolates between two <see cref="Color"/> values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated color value.</returns>
         */
        public static Color LerpColor(this TimedLerp<Color> lerp)
        {
            return Color.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /**
         * <summary>
         * Interpolates between two <see cref="Vector2"/> values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated vector value.</returns>
         */
        public static Vector2 LerpVector2(this TimedLerp<Vector2> lerp)
        {
            return Vector2.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /**
         * <summary>
         * Interpolates between two <see cref="Vector3"/> values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated vector value.</returns>
         */
        public static Vector3 LerpVector3(this TimedLerp<Vector3> lerp)
        {
            return Vector3.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /**
         * <summary>
         * Interpolates between two <see cref="Vector4"/> values based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated vector value.</returns>
         */
        public static Vector4 LerpVector4(this TimedLerp<Vector4> lerp)
        {
            return Vector4.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }

        /**
         * <summary>
         * Interpolates between two <see cref="Quaternion"/> values using spherical interpolation based on elapsed time.
         * </summary>
         * <param name="lerp">The timed lerp structure containing interpolation parameters.</param>
         * <returns>The interpolated quaternion value.</returns>
         */
        public static Quaternion LerpQuaternion(this TimedLerp<Quaternion> lerp)
        {
            return Quaternion.Lerp(lerp.startValue, lerp.targetValue, lerp.GetProgress());
        }
    }
}

