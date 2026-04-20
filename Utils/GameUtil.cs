using System.Collections.Generic;
using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides game-specific utility extensions for vectors and game objects.
     * Includes vector transformations relative to bases, radial calculations, and transform finding.
     * </summary>
     */
    public static class GameUtil
    {
        /**
         * <summary>
         * Converts the target vector to a vector relative to the basis vector (treated as the forward vector).
         * </summary>
         * <param name="target">The target vector to convert.</param>
         * <param name="basis">The basis vector that defines the new forward direction.</param>
         * <returns>The target vector relative to the basis.</returns>
         */
        public static Vector3 GetRelativeVector3(this Vector3 target, Vector3 basis)
        {
            return Quaternion.FromToRotation(Vector3.forward, basis) * target;
        }

        /**
         * <summary>
         * Converts the target vector to a vector relative to the basis vector (treated as the up vector).
         * </summary>
         * <param name="target">The target vector to convert.</param>
         * <param name="basis">The basis vector that defines the new up direction.</param>
         * <returns>The target vector relative to the basis.</returns>
         */
        public static Vector2 GetRelativeVector2(this Vector2 target, Vector2 basis)
        {
            return Quaternion.FromToRotation(Vector2.up, basis) * target;
        }

        #region Vector Transformations

        /**
         * <summary>
         * Finds a radial vector at a specific distance and angle from a given direction.
         * Given a vector u, returns a vector that is radius units away from the origin and rotated by angle degrees.
         * </summary>
         * <param name="u">The base direction vector.</param>
         * <param name="radius">The distance from the origin.</param>
         * <param name="angle">The rotation angle in degrees.</param>
         * <returns>A radial vector with the specified radius and rotation.</returns>
         */
        public static Vector3 FindRadialVector3(this Vector3 u, float radius, float angle)
        {
            u.Normalize();
            Vector3 axisOfRotation = Vector3.Cross(Vector3.forward, u);
            return Quaternion.AngleAxis(angle, axisOfRotation) * u * radius;
        }

        /**
         * <summary>
         * Finds the closest <see cref="Transform"/> in an array to the specified position.
         * </summary>
         * <param name="transforms">The array of transforms to search.</param>
         * <param name="v">The reference position.</param>
         * <returns>The transform closest to the specified position, or null if the array is empty.</returns>
         */
        public static Transform GetClosestTransform(this Transform[] transforms, Vector3 v)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;
            foreach (Transform tr in transforms)
            {
                float distance = Vector3.Distance(v, tr.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = tr;
                }
            }

            return closest;
        }

        #endregion

        /**
         * <summary>
         * Makes a <see cref="GameObject"/> look in a specified direction by adjusting its rotation.
         * </summary>
         * <param name="go">The game object to rotate.</param>
         * <param name="direction">The direction to look towards.</param>
         */
        public static void LookInDirection(this GameObject go, Vector3 direction)
        {
            go.transform.LookAt(direction + go.transform.position);
        }

        /**
         * <summary>
         * Attempts to get a component of type T from the specified game object or its children.
         * </summary>
         * <typeparam name="T">The component type to search for.</typeparam>
         * <param name="go">The game object to search.</param>
         * <param name="component">When this method returns, contains the component if found; otherwise, null.</param>
         * <returns>true if the component was found; otherwise, false.</returns>
         */
        public static bool TryGetComponentInChildren<T>(this GameObject go, out T component) where T : Component
        {
            component = go.GetComponentInChildren<T>();
            return component != null;
        }

        /**
         * <summary>
         * Replaces the contents of a list with new items from an enumerable collection.
         * </summary>
         * <typeparam name="T">The type of items in the list.</typeparam>
         * <param name="list">The list to refresh.</param>
         * <param name="items">The new items to populate the list with.</param>
         */
        public static void RefreshWith<T>(this List<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }
    }
}