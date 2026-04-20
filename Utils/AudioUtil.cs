using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utility extensions for <see cref="AudioSource"/> operations including randomization and playback control.
     /// </summary>
     */
    public static class AudioUtil
    {
        /**
         * <summary>
         * Randomizes the playback position of an audio clip within an <see cref="AudioSource"/>.
         /// Useful for adding variety to repeated sound effects.
         /// </summary>
        /// <param name="source">The audio source to randomize.</param>
        */
        public static void RandomizeTime(this AudioSource source)
        {
            source.time = source.clip.length * RandomUtil.RandomUFloat();
        }

        /**
         * <summary>
         * Plays an audio clip from a random position.
         /// Combines randomization and playback in one call.
         /// </summary>
        /// <param name="source">The audio source to play.</param>
        */
        public static void PlayRandomly(this AudioSource source)
        {
            source.RandomizeTime();
            source.Play();
        }
    }
}

