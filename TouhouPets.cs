global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using Terraria;
using Terraria.ID;

namespace TouhouPets
{
    public class TouhouPets : Mod
    {
        private static TouhouPets instance;
        public static PetChatRoom[] ChatRoom = new PetChatRoom[ChatRoomSystem.MaxChatRoom];
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
            LoadVanillaTextures();
        }
        private static void LoadVanillaTextures()
        {
            if (Main.dedServ)
                return;

            Main.instance.LoadItem(ItemID.TragicUmbrella);
            Main.instance.LoadItem(ItemID.Umbrella);
            Main.instance.LoadFlameRing();
            Main.instance.LoadProjectile(ProjectileID.CultistRitual);
        }
    }
}