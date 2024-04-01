using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouPets
{
    public struct DrawPetConfig
    {
        public DrawPetConfig(int rowCount)
        {
            TextureRow = rowCount;
        }
        /// <summary>
        /// 贴图应当划分的总列数
        /// </summary>
        public int TextureRow;
        /// <summary>
        /// 替换用贴图
        /// </summary>
        public Texture2D AltTexture = null;
        /// <summary>
        /// 是否应当启用EntitySpriteDraw，此方法会使宠物受到染料影响
        /// </summary>
        public bool ShouldUseEntitySpriteDraw = false;
        /// <summary>
        /// 基于原始尺寸的贴图尺寸（相乘）
        /// </summary>
        public float Scale = 1f;
        /// <summary>
        /// 基于原始位置的绘制位置偏移（相加）
        /// </summary>
        public Vector2 PositionOffset = Vector2.Zero;
    }
}
