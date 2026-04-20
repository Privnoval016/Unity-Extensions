using UnityEngine;

namespace Extensions.Utils
{
    /**
     * <summary>
     * Provides utility extensions for texture operations including conversion, manipulation, and effects.
     /// Useful for runtime texture processing and rendering.
     /// </summary>
     */
    public static class TextureUtil
    {
        /**
         * <summary>
         * Converts a <see cref="RenderTexture"/> to a <see cref="Texture2D"/>.
         /// Creates a readable copy of the render texture's contents.
         /// </summary>
        /// <param name="rTexture">The render texture to convert.</param>
        /// <returns>A new Texture2D with the render texture's contents.</returns>
        */
        public static Texture2D ToTexture2D(this RenderTexture rTexture)
        {
            Texture2D tex = new Texture2D(rTexture.width, rTexture.height, TextureFormat.ARGB32, false);
            RenderTexture oldActiveTexture = RenderTexture.active;
            RenderTexture.active = rTexture;
            tex.ReadPixels(new Rect(0, 0, rTexture.width, rTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = oldActiveTexture;
            return tex;
        }

        /**
         * <summary>
         * Fixes premultiplied alpha in a texture by unmultiplying the RGB channels.
         /// Used when alpha was pre-multiplied into color but you need separate color values.
         /// </summary>
        /// <param name="texture">The texture to fix.</param>
        /// <returns>The same texture with corrected alpha.</returns>
        */
        public static Texture2D FixPremultipliedAlpha(this Texture2D texture)
        {
            Color32[] data = texture.GetPixels32();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].a == 0 || data[i].a == byte.MaxValue)
                    continue;

                data[i].r = (data[i].r / (data[i].a / 255f)).ClampToByte();
                data[i].g = (data[i].g / (data[i].a / 255f)).ClampToByte();
                data[i].b = (data[i].b / (data[i].a / 255f)).ClampToByte();
            }

            texture.SetPixels32(data);
            texture.Apply();
            return texture;
        }

        /**
         * <summary>
         * Applies a fade effect to the edges of a texture, with optional corner rounding.
         /// Creates a smooth transition from opaque to transparent at the edges.
         /// </summary>
        /// <param name="texture">The texture to apply fading to.</param>
        /// <param name="fading">The distance over which the fade occurs (in pixels).</param>
        /// <param name="corner">The corner rounding distance (higher = more rounded corners).</param>
        /// <returns>The same texture with faded edges.</returns>
        */
        public static Texture2D FadeEdges(this Texture2D texture, float fading, float corner)
        {
            Color32[] data = texture.GetPixels32();
            Vector2 halfTextureSize = new Vector2(texture.width / 2f, texture.height / 2f);
            for (int i = 0; i < data.Length; i++)
            {
                int x = i % texture.width;
                int y = i / texture.width;

                float xDist = -(Mathf.Abs(x - halfTextureSize.x) - halfTextureSize.x);
                float yDist = -(Mathf.Abs(y - halfTextureSize.y) - halfTextureSize.y);
                float total = xDist + yDist;
                float distance = Mathf.Min(xDist, yDist);

                float fade = distance / fading;

                if (total / MathUtil.SQRT_2 < corner + fading)
                {
                    fade = Mathf.Min(fade, (total / MathUtil.SQRT_2 - corner) / fading);
                }

                fade = Mathf.Clamp01(fade);
                if (fade != 1f)
                    data[i].a = (data[i].a * EaseUtil.Ease(fade, 2, true)).ClampToByte();
            }

            texture.SetPixels32(data);
            texture.Apply();
            return texture;
        }
    }
}


