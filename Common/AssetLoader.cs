using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TouhouPets
{
    public static class AssetLoader
    {
        public static string TexturePath => $"{nameof(TouhouPets)}/Assets/Textures";
        public static Asset<Texture2D> GlowAura => Request<Texture2D>($"{TexturePath}/Aura");
        public static Asset<Texture2D> GlowSpark => Request<Texture2D>($"{TexturePath}/Spark");
    }
}
