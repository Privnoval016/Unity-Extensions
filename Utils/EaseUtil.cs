using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utilities for easing and damping animations, enabling smooth transitions between values.
     * Includes damping functions for various types and easing curves for animation control.
     * </summary>
     */
    public static class EaseUtil
    {
        #region Damping

        /**
         * <summary>
         * Dampens a float value towards a target using an exponential decay function.
         * </summary>
         * <param name="a">The current value.</param>
         * <param name="b">The target value.</param>
         * <param name="damping">The damping coefficient (higher = faster convergence).</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened value.</returns>
         */
        public static float Damp(float a, float b, float damping, float dt)
        {
            return Mathf.Lerp(a, b, BaseDamp(damping, dt));
        }

        /**
         * <summary>
         * Dampens an angle towards a target angle using spherical interpolation.
         * </summary>
         * <param name="a">The current angle in degrees.</param>
         * <param name="b">The target angle in degrees.</param>
         * <param name="damping">The damping coefficient.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened angle in degrees.</returns>
         */
        public static float DampAngle(float a, float b, float damping, float dt)
        {
            return Mathf.LerpAngle(a, b, BaseDamp(damping, dt));
        }

        /**
         * <summary>
         * Dampens a <see cref="Quaternion"/> rotation towards a target rotation.
         * </summary>
         * <param name="a">The current quaternion.</param>
         * <param name="b">The target quaternion.</param>
         * <param name="damping">The damping coefficient.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened quaternion.</returns>
         */
        public static Quaternion DampQuaternion(Quaternion a, Quaternion b, float damping, float dt)
        {
            return Quaternion.Slerp(a, b, BaseDamp(damping, dt));
        }

        /**
         * <summary>
         * Dampens a <see cref="Vector2"/> towards a target vector using uniform damping.
         * </summary>
         * <param name="a">The current vector.</param>
         * <param name="b">The target vector.</param>
         * <param name="damping">The damping coefficient applied to both axes.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened vector.</returns>
         */
        public static Vector2 DampVector2(Vector2 a, Vector2 b, float damping, float dt)
        {
            return Vector2.Lerp(a, b, BaseDamp(damping, dt));
        }

        /**
         * <summary>
         * Dampens a <see cref="Vector2"/> towards a target vector using per-axis damping coefficients.
         * </summary>
         * <param name="a">The current vector.</param>
         * <param name="b">The target vector.</param>
         * <param name="damping">A vector containing per-axis damping coefficients.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened vector.</returns>
         */
        public static Vector2 DampVector2(Vector2 a, Vector2 b, Vector2 damping, float dt)
        {
            return new Vector2(Damp(a.x, b.x, damping.x, dt), Damp(a.y, b.y, damping.y, dt));
        }

        /**
         * <summary>
         * Dampens a <see cref="Vector3"/> towards a target vector using uniform damping.
         * </summary>
         * <param name="a">The current vector.</param>
         * <param name="b">The target vector.</param>
         * <param name="damping">The damping coefficient applied to all axes.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened vector.</returns>
         */
        public static Vector3 DampVector3(Vector3 a, Vector3 b, float damping, float dt)
        {
            return Vector3.Lerp(a, b, BaseDamp(damping, dt));
        }

        /**
         * <summary>
         * Dampens a <see cref="Vector3"/> towards a target vector using per-axis damping coefficients.
         * </summary>
         * <param name="a">The current vector.</param>
         * <param name="b">The target vector.</param>
         * <param name="damping">A vector containing per-axis damping coefficients.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>The dampened vector.</returns>
         */
        public static Vector3 DampVector3(Vector3 a, Vector3 b, Vector3 damping, float dt)
        {
            return new Vector3(
                Damp(a.x, b.x, damping.x, dt),
                Damp(a.y, b.y, damping.y, dt),
                Damp(a.z, b.z, damping.z, dt)
            );
        }

        /**
         * <summary>
         * Calculates the base damping factor using exponential decay.
         * </summary>
         * <param name="damping">The damping coefficient.</param>
         * <param name="dt">The delta time in seconds.</param>
         * <returns>A lerp factor between 0 and 1.</returns>
         */
        static float BaseDamp(float damping, float dt)
        {
            return 1 - Mathf.Exp(-damping * dt);
        }

        #endregion

        #region Easing

        /**
         * <summary>
         * Applies an easing function to a normalized value (0-1).
         * </summary>
         * <param name="x">The normalized input value.</param>
         * <param name="easing">The easing power (higher = more curve).</param>
         * <param name="reverse">If true, applies the reverse easing curve.</param>
         * <returns>The eased value.</returns>
         */
        public static float Ease(float x, float easing, bool reverse = false)
        {
            float sign = Mathf.Sign(x);
            float abs = Mathf.Abs(x);
            if (reverse)
                return (1 - Mathf.Pow(1 - abs, easing)) * sign;
            else
                return Mathf.Pow(abs, easing) * sign;
        }

        /**
         * <summary>
         * Applies a smooth ease-in-out curve to a normalized value.
         * </summary>
         * <param name="x">The normalized input value (0-1).</param>
         * <param name="power">The power of the easing curve (default 3 for smooth cubic).</param>
         * <returns>The eased value.</returns>
         */
        public static float EaseInOut(float x, float power = 3)
        {
            if (x < 0.5f)
                return Mathf.Pow(2, power - 1) * Mathf.Pow(x, power);
            else
                return 1 - Mathf.Pow(-2 * x + 2, power) / 2;
        }

        /**
         * <summary>
         * Applies a "back" easing function that slightly overshoots the target before settling.
         * </summary>
         * <param name="x">The normalized input value (0-1).</param>
         * <param name="overshoot">The amount of overshoot (default 1.7f).</param>
         * <param name="reverse">If true, applies the reverse easing curve.</param>
         * <returns>The eased value.</returns>
         */
        public static float EaseBack(float x, float overshoot = 1.7f, bool reverse = false)
        {
            float x2 = (reverse ? 1 - x : x) - 1;
            float result = x2 * x2 * ((x2 + 1) * overshoot + x2) + 1;

            return reverse ? 1 - result : result;
        }

        /**
         * <summary>
         * Applies a bell curve easing function that peaks in the middle (useful for punch/emphasis effects).
         * Values &lt;1 produce no ease-in/out, ~2 is when ease-in/out becomes clearly visible.
         * </summary>
         * <param name="x">The normalized input value (0-1).</param>
         * <param name="power">The power of the curve (higher = sharper peak).</param>
         * <returns>The eased value.</returns>
         */
        public static float BellCurveEase(float x, float power = 3)
        {
            return Mathf.Pow(-4 * x * (x - 1), power);
        }

        #endregion
    }
}