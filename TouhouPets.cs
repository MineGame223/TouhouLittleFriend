global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;

namespace TouhouPets
{
    public class TouhouPets : Mod
    {
        private static TouhouPets instance;
        public static bool devMode = false;
        public static TouhouPets Instance { get => instance; set => instance = value; }
        public override void Load()
        {
            instance = this;
        }
        public override void Unload()
        {
            instance = null;
        }
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("ShopLookup", out Mod result))
            {
                ShopLookupSupport.Setup(result);
            }
        }
    }
}