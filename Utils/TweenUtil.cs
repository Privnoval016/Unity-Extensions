//#define PRIMETWEEN

#if PRIMETWEEN

using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides tweening and movement utility extensions using the PrimeTween animation library.
     * This class is only compiled when PRIMETWEEN is defined. Includes methods for tweening rigidbodies and transforms.
     * </summary>
     */
    public static class TweenUtil
    {
        #region Tweening

        /**
         * <summary>
         * Tweens a <see cref="Rigidbody"/> to move a specified distance in a given direction.
         * </summary>
         * <param name="rb">The rigidbody to tween.</param>
         * <param name="direction">The direction to move (will be normalized).</param>
         * <param name="distance">The distance to traverse.</param>
         * <param name="time">The duration of the tween in seconds.</param>
         * <param name="ease">The easing function to apply to the animation.</param>
         */
        public static void TweenDistance(this Rigidbody rb, Vector3 direction, float distance, float time,
            Ease ease = Ease.Default)
        {
            direction.Normalize();
            rb.linearVelocity = Vector3.zero;
            Tween.RigidbodyMovePosition(rb, rb.position + direction * distance, time, ease);
        }

        /**
         * <summary>
         * Tweens a <see cref="Transform"/> to move a specified distance in a given direction.
         * </summary>
         * <param name="t">The transform to tween.</param>
         * <param name="direction">The direction to move (will be normalized).</param>
         * <param name="distance">The distance to traverse.</param>
         * <param name="time">The duration of the tween in seconds.</param>
         * <param name="ease">The easing function to apply to the animation.</param>
         */
        public static void TweenDistance(this Transform t, Vector3 direction, float distance, float time,
            Ease ease = Ease.Default)
        {
            direction.Normalize();
            Tween.Position(t, t.position + direction * distance, time, ease);
        }
        
        /**
         * <summary>
         * Finds the position of a <see cref="Transform"/> after dropping it to the ground using a raycast.
         * </summary>
         * <param name="t">The transform to raycast from.</param>
         * <param name="groundMask">The layer mask for ground detection. Default is everything.</param>
         * <param name="distance">The maximum raycast distance in units.</param>
         * <returns>The ground position if hit; otherwise, the transform's current position.</returns>
         */
        public static Vector3 GetGroundedPosition(this Transform t, LayerMask groundMask = default,
            float distance = 100f)
        {
            RaycastHit hit;
            if (Physics.SphereCast(t.position, 0.1f, Vector3.down, out hit, distance, groundMask))
            {
                return hit.point;
            }

            return t.position;
        }

        #endregion
    }
}

#endif