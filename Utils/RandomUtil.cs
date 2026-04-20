using System;
using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides random number generation utilities including wrappers around Unity and System.Random.
     * Includes support for various random types like floats, integers, vectors, and weighted booleans.
     * </summary>
     */
    public static class RandomUtil
    {
        #region UnityEngine.Random wrappers

        /**
         * <summary>
         * Returns a random float between min (inclusive) and max (inclusive).
         * Wrapper around <see cref="UnityEngine.Random.Range(float, float)"/>.
         * </summary>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value.</param>
         * <returns>A random float value.</returns>
         */
        public static float RandomRange(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /**
         * <summary>
         * Returns a random int between min (inclusive) and max (inclusive).
         * Wrapper around <see cref="UnityEngine.Random.Range(int, int)"/> that includes the upper bound.
         * </summary>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value (inclusive).</param>
         * <returns>A random int value.</returns>
         */
        public static int RandomRange(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        /**
         * <summary>
         * Returns a random int between min (inclusive) and max (exclusive).
         * Wrapper around <see cref="UnityEngine.Random.Range(int, int)"/>.
         * </summary>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value (exclusive).</param>
         * <returns>A random int value.</returns>
         */
        public static int RandomRangeExclusive(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        #endregion

        #region System.Random extensions

        /**
         * <summary>
         * Returns the next random float (0-1) from a <see cref="System.Random"/>.
         * </summary>
         * <param name="random">The random generator.</param>
         * <returns>A random float between 0 and 1.</returns>
         */
        public static float NextFloat(this System.Random random)
        {
            return (float)random.NextDouble();
        }

        /**
         * <summary>
         * Returns a random int between min (inclusive) and max (inclusive) from a <see cref="System.Random"/>.
         * </summary>
         * <param name="random">The random generator.</param>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value (inclusive).</param>
         * <returns>A random int value.</returns>
         */
        public static int Range(this System.Random random, int min, int max)
        {
            return random.RangeExclusive(min, max + 1);
        }

        /**
         * <summary>
         * Returns a random float between min (inclusive) and max (exclusive) from a <see cref="System.Random"/>.
         * </summary>
         * <param name="random">The random generator.</param>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value (exclusive).</param>
         * <returns>A random float value.</returns>
         */
        public static float Range(this System.Random random, float min, float max)
        {
            return min + random.NextFloat() * (max - min);
        }

        /**
         * <summary>
         * Returns a random int between min (inclusive) and max (exclusive) from a <see cref="System.Random"/>.
         * </summary>
         * <param name="random">The random generator.</param>
         * <param name="min">The minimum value.</param>
         * <param name="max">The maximum value (exclusive).</param>
         * <returns>A random int value.</returns>
         */
        public static int RangeExclusive(this System.Random random, int min, int max)
        {
            return min + (int)(random.NextDouble() * (max - min));
        }

        /**
         * <summary>
         * Returns the next random unsigned int from a <see cref="System.Random"/>.
         * </summary>
         * <param name="random">The random generator.</param>
         * <returns>A random uint value.</returns>
         */
        public static uint NextUInt(this System.Random random)
        {
            byte[] bytes = new byte[4];
            random.NextBytes(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        #endregion

        #region New Random Functions

        /**
         * <summary>
         * Returns a random sign: either 1 or -1.
         * </summary>
         * <returns>1 or -1 with equal probability.</returns>
         */
        public static int RandomSign()
        {
            return RandomBool(0.5f) ? 1 : -1;
        }

        /**
         * <summary>
         * Returns a random boolean value with an optional weight.
         * </summary>
         * <param name="weight">The probability of returning true (0-1). Default is 0.5 for 50/50.</param>
         * <returns>true with probability equal to weight; otherwise, false.</returns>
         */
        public static bool RandomBool(float weight = 0.5f)
        {
            return RandomUFloat() < weight;
        }

        /**
         * <summary>
         * Returns a random float between -1 and 1.
         * </summary>
         * <returns>A random float in the range [-1, 1].</returns>
         */
        public static float RandomFloat()
        {
            return RandomRange(-1f, 1f);
        }

        /**
         * <summary>
         * Returns a random float between 0 and 1.
         * </summary>
         * <returns>A random float in the range [0, 1].</returns>
         */
        public static float RandomUFloat()
        {
            return RandomRange(0f, 1f);
        }

        /**
         * <summary>
         * Returns a random unit <see cref="Vector2"/> distributed uniformly within a circle.
         * The magnitude is also randomized to ensure uniform distribution across the circle.
         * </summary>
         * <returns>A random vector within a unit circle.</returns>
         */
        public static Vector2 RandomVector2Circular()
        {
            float angle = RandomUFloat() * 360;
            float distance = Mathf.Sqrt(RandomUFloat());
            return Vector2.up.Rotate(angle) * distance;
        }

        #endregion
    }
}
