using UnityEngine;
using Object = UnityEngine.Object;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utility extensions for <see cref="Component"/> and <see cref="GameObject"/> operations.
     * Includes component management, retrieval, and manipulation helpers.
     * </summary>
     */
    public static class ComponentUtil
    {
        /**
         * <summary>
         * Removes a component of a specified type from a <see cref="GameObject"/>.
         * </summary>
         * <typeparam name="T">The component type to remove.</typeparam>
         * <param name="gameObject">The game object to remove the component from.</param>
         */
        public static void RemoveComponent<T>(this GameObject gameObject)
            where T : Component
        {
            Object.Destroy(gameObject.GetComponent<T>());
        }

        /**
         * <summary>
         * Adds a component of type T to a <see cref="GameObject"/>, or returns the existing one if it already exists.
         * </summary>
         * <typeparam name="T">The component type to add or get.</typeparam>
         * <param name="gameObject">The game object to add the component to.</param>
         * <returns>The existing or newly added component.</returns>
         */
        public static T TryAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component)
                return component;
            return gameObject.AddComponent<T>();
        }

        /**
         * <summary>
         * Gets or adds a component of type T to a <see cref="GameObject"/>.
         * If the component doesn't exist, it will be added.
         * </summary>
         * <typeparam name="T">The component type to get or add.</typeparam>
         * <param name="gameObject">The game object.</param>
         * <returns>The existing or newly added component.</returns>
         */
        public static T GetOrAddComponent<T>(this GameObject gameObject)
            where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /**
         * <summary>
         * Gets a <see cref="RectTransform"/> component from a <see cref="Component"/>.
         /// </summary>
        /// <param name="component">The component to get the RectTransform from.</param>
        /// <returns>The RectTransform component if it exists; otherwise, null.</returns>
        */
        public static RectTransform GetRect(this Component component)
        {
            return component.GetComponent<RectTransform>();
        }

        /**
         * <summary>
         * Gets a <see cref="RectTransform"/> component from a <see cref="GameObject"/>.
         /// </summary>
        /// <param name="gameObject">The game object to get the RectTransform from.</param>
        /// <returns>The RectTransform component if it exists; otherwise, null.</returns>
        */
        public static RectTransform GetRect(this GameObject gameObject)
        {
            return gameObject.GetComponent<RectTransform>();
        }


    }
}


