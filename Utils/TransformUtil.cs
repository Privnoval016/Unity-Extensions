using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utility extensions for <see cref="Transform"/> operations including rotation, child management, and positioning.
     * </summary>
     */
    public static class TransformUtil
    {
        #region Transforms

        /**
         * <summary>
         * Sets the 2D rotation of a <see cref="Transform"/> (z-axis rotation in degrees).
         * </summary>
         * <param name="transform">The transform to rotate.</param>
         * <param name="rotation">The rotation in degrees.</param>
         */
        public static void SetRotation(this Transform transform, float rotation)
        {
            transform.eulerAngles = new Vector3(0, 0, rotation);
        }

        /**
         * <summary>
         * Sets the local 2D rotation of a <see cref="Transform"/> (z-axis rotation in degrees).
         * </summary>
         * <param name="transform">The transform to rotate.</param>
         * <param name="rotation">The local rotation in degrees.</param>
         */
        public static void SetLocalRotation(this Transform transform, float rotation)
        {
            transform.localEulerAngles = new Vector3(0, 0, rotation);
        }

        #endregion

        #region Children

        /**
         * <summary>
         * Adds a new empty child <see cref="GameObject"/> to the transform.
         * </summary>
         * <param name="transform">The parent transform.</param>
         * <returns>The newly created child game object.</returns>
         */
        public static GameObject AddChild(this Transform transform)
        {
            GameObject child = new GameObject();
            child.transform.SetParent(transform);
            child.transform.localScale = Vector3.one;
            child.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return child;
        }

        /**
         * <summary>
         * Adds a new named child <see cref="GameObject"/> to the transform.
         * </summary>
         * <param name="transform">The parent transform.</param>
         * <param name="name">The name for the child game object.</param>
         * <returns>The newly created child game object.</returns>
         */
        public static GameObject AddChild(this Transform transform, string name)
        {
            GameObject child = transform.AddChild();
            child.name = name;
            return child;
        }

        /**
         * <summary>
         * Destroys all child objects of a <see cref="Transform"/>.
         * </summary>
         * <param name="transform">The parent transform.</param>
         * <returns>The same transform for method chaining.</returns>
         */
        public static Transform DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);

            return transform;
        }

        /**
         * <summary>
         * Gets a component of a specific type from a child at the specified index.
         * </summary>
         * <typeparam name="T">The component type to retrieve.</typeparam>
         * <param name="transform">The parent transform.</param>
         * <param name="childIndex">The index of the child.</param>
         * <returns>The component on the child transform.</returns>
         */
        public static T GetChildComponent<T>(this Transform transform, int childIndex)
        {
            return transform.GetChild(childIndex).GetComponent<T>();
        }

        #endregion
    }
}

