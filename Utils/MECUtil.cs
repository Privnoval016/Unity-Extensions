//#define MEC

#if MEC

using System;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides coroutine and physics utility extensions for the Modular Coroutine (MEC) system.
     * This class is only compiled when MEC is defined. Includes coroutine management and rigidbody physics helpers.
     * </summary>
     */
    public static class MECUtil
    {
        #region Coroutines

        /**
         * <summary>
         * Creates a coroutine that waits for a specified delay before invoking an action.
         * </summary>
         * <param name="delay">The delay in seconds.</param>
         * <param name="action">The action to invoke after the delay.</param>
         * <returns>A coroutine enumerator.</returns>
         */
        public static IEnumerator<float> RunAfterDelay(float delay, Action action)
        {
            yield return Timing.WaitForSeconds(delay);
            action();
        }

        /**
         * <summary>
         * Runs a coroutine with MEC timing and automatic cancellation when the game object is destroyed.
         * </summary>
         * <param name="m">The <see cref="MonoBehaviour"/> owner of the coroutine.</param>
         * <param name="coroutine">The coroutine to run.</param>
         * <param name="tag">Optional tag for identifying and managing the coroutine.</param>
         * <param name="segment">The timing segment to run in (e.g., RealtimeUpdate, Update, etc.).</param>
         * <returns>A handle to the running coroutine.</returns>
         */
        public static CoroutineHandle RunSegmentCoroutine(this MonoBehaviour m, IEnumerator<float> coroutine,
            string tag = null,
            Segment segment = Segment.RealtimeUpdate)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return Timing.RunCoroutine(coroutine.CancelWith(m), segment, m.GetInstanceID());
            }

            return Timing.RunCoroutine(coroutine.CancelWith(m), segment, m.GetInstanceID(), tag);
        }

        /**
         * <summary>
         * Kills all coroutines running on a <see cref="MonoBehaviour"/>, optionally filtered by tag.
         * </summary>
         * <param name="m">The <see cref="MonoBehaviour"/> whose coroutines to kill.</param>
         * <param name="tag">Optional tag to only kill coroutines with that tag.</param>
         */
        public static void KillObjectCoroutines(this MonoBehaviour m, string tag = null)
        {
            if (string.IsNullOrEmpty(tag))
            {
                Timing.KillCoroutines(m.GetInstanceID());
                return;
            }

            Timing.KillCoroutines(m.GetInstanceID(), tag);
        }

        #endregion
        
        #region Rigidbody Physics

        /**
         * <summary>
         * Creates a coroutine that moves a rigidbody a specified distance in a given time using impulse force.
         * The rigidbody will have its velocity reset to zero at the end.
         * </summary>
         * <param name="rb">The rigidbody to move.</param>
         * <param name="direction">The direction to move (will be normalized).</param>
         * <param name="distance">The distance to traverse.</param>
         * <param name="time">The time in seconds to complete the movement.</param>
         * <param name="condition">Optional additional condition to terminate the movement early.</param>
         * <returns>A coroutine enumerator.</returns>
         */
        public static IEnumerator<float> TraverseDistanceInTime(this Rigidbody rb, Vector3 direction, float distance,
            float time, Func<bool> condition = null)
        {
            if (time <= 0 || distance <= 0)
            {
                yield break;
            }

            rb.linearVelocity = Vector3.zero;

            direction.Normalize();
            Vector3 initialPosition = rb.position;
            float startTime = Time.time;

            float impulse = rb.mass * (distance / time - rb.linearVelocity.magnitude);
            Debug.Log(
                $"Impulse: {impulse}, Mass: {rb.mass}, Distance: {distance}, Time: {time}, Direction: {direction}");
            rb.AddForce(direction * impulse, ForceMode.Impulse);

            yield return Timing.WaitUntilTrue(() => Vector3.Distance(rb.position, initialPosition) >= distance ||
                                                    Time.time - startTime >= time ||
                                                    (condition != null && condition()));

            rb.linearVelocity = Vector3.zero;
        }

        /**
         * <summary>
         * Creates a coroutine that moves a rigidbody with a constant velocity in a given direction until a condition is met.
         * The rigidbody will have its velocity reset to zero when the loop condition becomes false.
         * </summary>
         * <param name="rb">The rigidbody to move.</param>
         * <param name="direction">The direction to move (will be normalized).</param>
         * <param name="magnitude">The velocity magnitude to apply.</param>
         * <param name="loopCondition">The condition to continue moving (returns true to continue, false to stop).</param>
         * <returns>A coroutine enumerator.</returns>
         */
        public static IEnumerator<float> TraverseWithVelocity(this Rigidbody rb, Vector3 direction, float magnitude,
            Func<bool> loopCondition)
        {
            direction.Normalize();

            while (loopCondition())
            {
                rb.linearVelocity = direction * magnitude;
                yield return Timing.WaitForOneFrame;
            }

            rb.linearVelocity = Vector3.zero;
        }

        #endregion
    }
}

#endif