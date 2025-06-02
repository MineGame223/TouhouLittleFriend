using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace TouhouPets
{
    /// <summary>
    /// 部分原版函数的替换案
    /// </summary>
    internal static class AltVanillaFunction
    {
        /// <summary>
        /// 替换原版PlaySound，允许编辑
        /// <br/>SoundStyle种类的type
        /// <br/>Vector2类型坐标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public static void PlaySound(SoundStyle type, Vector2 position, float volume = 1f, float pitch = 0f, float pitchVariance = 0f)
        {
            SoundStyle sound = type with
            {
                Volume = volume,
                Pitch = MathHelper.Clamp(pitch, -1, 1),
                PitchVariance = MathHelper.Clamp(pitchVariance, 0, 1),
            };
            SoundEngine.PlaySound(sound, position);
        }
        /// <summary>
        /// 替换原版GetTexture
        /// </summary>
        /// <param name="path"></param>
        /// <returns>TouhouPets / Assets / Textures / 剩余路径</returns>
        public static Texture2D GetTexture(string path)
        {
            return Request<Texture2D>($"{AssetLoader.TexturePath}/{path}").Value;
        }
        /// <summary>
        /// 快速获取Extra材质
        /// </summary>
        /// <param name="path"></param>
        /// <returns>TeaNPC/Assets/Textures/Extra/剩余路径名</returns>
        public static Texture2D GetExtraTexture(string path)
        {
            return GetTexture($"Extra/{path}");
        }
        /// <summary>
        /// 快速获取Glow材质
        /// </summary>
        /// <param name="path"></param>
        /// <returns>TeaNPC/Assets/Textures/Glow/剩余路径名</returns>
        public static Texture2D GetGlowTexture(string path)
        {
            return GetTexture($"Glow/{path}");
        }
        public static Texture2D ProjectileTexture(int id)
        {
            return TextureAssets.Projectile[id].Value;
        }
        public static Texture2D NPCTexture(int id)
        {
            return TextureAssets.Npc[id].Value;
        }
        public static Texture2D ItemTexture(int id)
        {
            return TextureAssets.Item[id].Value;
        }
        public static Texture2D TileTexture(int id)
        {
            return TextureAssets.Tile[id].Value;
        }
        public static Texture2D ExtraTexture(int id)
        {
            return TextureAssets.Extra[id].Value;
        }
        public static Texture2D BuffTexture(int id)
        {
            return TextureAssets.Buff[id].Value;
        }
        public static Texture2D ChainTexture(int id)
        {
            return TextureAssets.Chains[id].Value;
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>float类型的scale
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="effects"></param>
        /// <param name="layerDepth"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>Vector2类型的scale
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="effects"></param>
        /// <param name="layerDepth"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>允许进行材质拉伸
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="effects"></param>
        /// <param name="layerDepth"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>3参数类型
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color)
        {
            spriteBatch.Draw(texture, position, color);
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>3参数类型
        /// <br/>允许进行材质拉伸
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="color"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(texture, destinationRectangle, color);
        }
        /// <summary>
        /// 替换原版spriteBatch.Draw
        /// <br/>4参数类型
        /// <br/>允许进行材质拉伸
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        public static void TeaNPCDraw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }
    }
}
