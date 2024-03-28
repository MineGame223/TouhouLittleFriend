using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouPets
{
    public struct DrawPetConfig
    {
        public DrawPetConfig(int rowCount = 1)
        {
            TextureRow = rowCount;
        }
        public int TextureRow;
        public Texture2D AltTexture = null;
        public bool ShouldUseEntitySpriteDraw = false;
        public float Scale = 1f;
        public Vector2 PositionOffset = Vector2.Zero;
    }
}
