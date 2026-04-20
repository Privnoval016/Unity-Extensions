using UnityEngine;
using UnityEngine.VFX;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides safe wrapper methods for setting properties on <see cref="VisualEffect"/> graphs.
     * These methods check for property existence before setting values, preventing errors when a property doesn't exist.
     * </summary>
     */
    public static class VFXUtil
    {
        #region Safe Setters (String)

        /**
         * <summary>
         * Safely sets a <see cref="Vector4"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector4(this VisualEffect vfx, string name, Vector4 value)
        {
            if (!vfx.HasVector4(name)) return false;
            vfx.SetVector4(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a float property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetFloat(this VisualEffect vfx, string name, float value)
        {
            if (!vfx.HasFloat(name)) return false;
            vfx.SetFloat(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets an int property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetInt(this VisualEffect vfx, string name, int value)
        {
            if (!vfx.HasInt(name)) return false;
            vfx.SetInt(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a bool property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetBool(this VisualEffect vfx, string name, bool value)
        {
            if (!vfx.HasBool(name)) return false;
            vfx.SetBool(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Texture"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetTexture(this VisualEffect vfx, string name, Texture value)
        {
            if (!vfx.HasTexture(name)) return false;
            vfx.SetTexture(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Mesh"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetMesh(this VisualEffect vfx, string name, Mesh value)
        {
            if (!vfx.HasMesh(name)) return false;
            vfx.SetMesh(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Vector3"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector3(this VisualEffect vfx, string name, Vector3 value)
        {
            if (!vfx.HasVector3(name)) return false;
            vfx.SetVector3(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Vector2"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector2(this VisualEffect vfx, string name, Vector2 value)
        {
            if (!vfx.HasVector2(name)) return false;
            vfx.SetVector2(name, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Gradient"/> property on a <see cref="VisualEffect"/> by property name.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="name">The property name.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetGradient(this VisualEffect vfx, string name, Gradient value)
        {
            if (!vfx.HasGradient(name)) return false;
            vfx.SetGradient(name, value);
            return true;
        }

        #endregion

        #region Safe Setters (ID)

        /**
         * <summary>
         * Safely sets a <see cref="Vector4"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector4(this VisualEffect vfx, int id, Vector4 value)
        {
            if (!vfx.HasVector4(id)) return false;
            vfx.SetVector4(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a float property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetFloat(this VisualEffect vfx, int id, float value)
        {
            if (!vfx.HasFloat(id)) return false;
            vfx.SetFloat(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets an int property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetInt(this VisualEffect vfx, int id, int value)
        {
            if (!vfx.HasInt(id)) return false;
            vfx.SetInt(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a bool property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetBool(this VisualEffect vfx, int id, bool value)
        {
            if (!vfx.HasBool(id)) return false;
            vfx.SetBool(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Texture"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetTexture(this VisualEffect vfx, int id, Texture value)
        {
            if (!vfx.HasTexture(id)) return false;
            vfx.SetTexture(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Mesh"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetMesh(this VisualEffect vfx, int id, Mesh value)
        {
            if (!vfx.HasMesh(id)) return false;
            vfx.SetMesh(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Vector3"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector3(this VisualEffect vfx, int id, Vector3 value)
        {
            if (!vfx.HasVector3(id)) return false;
            vfx.SetVector3(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Vector2"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetVector2(this VisualEffect vfx, int id, Vector2 value)
        {
            if (!vfx.HasVector2(id)) return false;
            vfx.SetVector2(id, value);
            return true;
        }

        /**
         * <summary>
         * Safely sets a <see cref="Gradient"/> property on a <see cref="VisualEffect"/> by property ID.
         * </summary>
         * <param name="vfx">The visual effect to modify.</param>
         * <param name="id">The property ID.</param>
         * <param name="value">The value to set.</param>
         * <returns>true if the property exists and was set; otherwise, false.</returns>
         */
        public static bool SafeSetGradient(this VisualEffect vfx, int id, Gradient value)
        {
            if (!vfx.HasGradient(id)) return false;
            vfx.SetGradient(id, value);
            return true;
        }

        #endregion
    }
}

