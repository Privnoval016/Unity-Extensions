using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor.U2D;
#endif

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides general-purpose utility extensions for Unity functionality.
     * Includes threading, array manipulation, enum helpers, and editor-only sprite atlas utilities.
     /// </summary>
     */
    public static class UnityUtil
    {
        /**
         * <summary>
         * Starts a new background thread with the specified action.
         * </summary>
         * <param name="action">The action to run on the thread.</param>
         * <returns>The created thread.</returns>
         */
        public static Thread StartThread(Action action)
        {
            ThreadStart threadStart = new ThreadStart(action);
            Thread thread = new Thread(threadStart);
            thread.Start();
            return thread;
        }

        /**
         * <summary>
         * Converts a <see cref="NBool"/> value to a boolean.
         * True returns true, False returns false, and Both returns true.
         * </summary>
         * <param name="nBool">The nullable bool value.</param>
         * <returns>The boolean representation.</returns>
         */
        public static bool IsTrue(this NBool nBool)
        {
            switch (nBool)
            {
                case NBool.True:
                    return true;
                case NBool.False:
                    return false;
                case NBool.Both:
                    return true;
                default:
                    return false;
            }
        }

        /**
         * <summary>
         * Logs a message to the console using Debug.Log.
         * </summary>
         * <param name="message">The message to log.</param>
         */
        public static void Print(object message)
        {
            Debug.Log(message);
        }

        /**
         * <summary>
         * Converts an array to a string representation with comma-separated values.
         * </summary>
         * <typeparam name="T">The type of elements in the array.</typeparam>
         * <param name="array">The array to convert.</param>
         * <returns>A string representation of the array.</returns>
         */
        public static string ArrayAsString<T>(this T[] array)
        {
            return string.Join(", ", array.Select(x => x.ToString()).ToArray());
        }

        /**
         * <summary>
         * Adds an item to an array, returning a new array with increased size.
         * </summary>
         * <typeparam name="T">The type of elements in the array.</typeparam>
         * <param name="array">The original array.</param>
         * <param name="item">The item to add.</param>
         * <returns>A new array containing all original elements plus the new item.</returns>
         */
        public static T[] Add<T>(this T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        /**
         * <summary>
         * Removes the first occurrence of an item from an array, returning a new array.
         * </summary>
         * <typeparam name="T">The type of elements in the array.</typeparam>
         * <param name="array">The original array.</param>
         * <param name="item">The item to remove.</param>
         * <returns>A new array with the first occurrence of the item removed.</returns>
         */
        public static T[] Remove<T>(this T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Remove(item);
            return list.ToArray();
        }

        /**
         * <summary>
         * Teleports a <see cref="CharacterController"/> to a new position by temporarily disabling and re-enabling it.
         * </summary>
         * <param name="characterController">The character controller to teleport.</param>
         * <param name="newPosition">The new position to teleport to.</param>
         */
        public static void Teleport(this CharacterController characterController, Vector3 newPosition)
        {
            characterController.enabled = false;
            characterController.transform.position = newPosition;
            characterController.enabled = true;
        }

        /**
         * <summary>
         * Removes all delegate listeners from a delegate object.
         * Useful for cleaning up event subscriptions.
         * </summary>
         * <param name="delegateObject">The delegate to clear.</param>
         */
        public static void RemoveAllDelegateListeners(this Delegate delegateObject)
        {
            if (delegateObject == null)
                return;

            foreach (Delegate d in delegateObject.GetInvocationList())
            {
                delegateObject = Delegate.Remove(delegateObject, d);
            }
        }

        #region Line Renderers

        /**
         * <summary>
         * Sets all color keys in a <see cref="LineRenderer"/>'s gradient to a specified color.
         /// </summary>
         * <param name="lineRenderer">The line renderer to modify.</param>
         * <param name="color">The color to set.</param>
         */
        public static void SetAllColorKeys(this LineRenderer lineRenderer, Color color)
        {
            Gradient colorGradient = lineRenderer.colorGradient;
            GradientColorKey[] colorKeys = colorGradient.colorKeys;
            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i].color = color;
            }

            colorGradient.colorKeys = colorKeys;
            lineRenderer.colorGradient = colorGradient;
        }

        /**
         * <summary>
         * Sets all alpha keys in a <see cref="LineRenderer"/>'s gradient to a specified alpha value.
         /// </summary>
         * <param name="lineRenderer">The line renderer to modify.</param>
         * <param name="alpha">The alpha value to set (0-1).</param>
         */
        public static void SetAllAlphaKeys(this LineRenderer lineRenderer, float alpha)
        {
            Gradient colorGradient = lineRenderer.colorGradient;
            GradientAlphaKey[] alphaKeys = colorGradient.alphaKeys;
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i].alpha = alpha;
            }

            colorGradient.alphaKeys = alphaKeys;
            lineRenderer.colorGradient = colorGradient;
        }

        #endregion

        #region Sprite Atlases

#if UNITY_EDITOR
        /**
         * <summary>
         * Clears all packable objects from a <see cref="SpriteAtlas"/>.
         /// Only available in the Unity Editor.
         /// </summary>
         * <param name="atlas">The sprite atlas to clear.</param>
         */
        public static void ClearPackables(this SpriteAtlas atlas)
        {
            atlas.Remove(atlas.GetPackables());
        }

        /**
         * <summary>
         * Sets the packable objects for a <see cref="SpriteAtlas"/>, replacing any existing ones.
         /// Only available in the Unity Editor.
         /// </summary>
         * <param name="atlas">The sprite atlas to modify.</param>
         * <param name="objects">The objects to add to the atlas.</param>
         */
        public static void SetPackables(this SpriteAtlas atlas, Object[] objects)
        {
            atlas.Remove(atlas.GetPackables());
            atlas.Add(objects);
        }

        /**
         * <summary>
         * Sets the texture compression settings for a <see cref="SpriteAtlas"/>.
         /// Only available in the Unity Editor.
         /// </summary>
         * <param name="atlas">The sprite atlas to modify.</param>
         * <param name="compression">The compression mode to apply.</param>
         */
        public static void SetImporterCompression(
            this SpriteAtlas atlas,
            TextureImporterCompression compression
        )
        {
            TextureImporterPlatformSettings platformSettings = atlas.GetPlatformSettings(
                "DefaultTexturePlatform"
            );
            platformSettings.textureCompression = compression;
            atlas.SetPlatformSettings(platformSettings);
        }
#endif

        #endregion
    }
    
    /**
     * <summary>
     * A tri-state boolean enum that can be True, False, or Both.
     * Useful for optional boolean parameters or tri-state logic.
     * </summary>
     */
    public enum NBool
    {
        False,
        True,
        Both
    }
}