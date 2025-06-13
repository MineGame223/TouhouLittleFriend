global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TouhouPets
{
    public partial class TouhouPets : Mod
    {
        private static PetChatRoom[] chatRoom = new PetChatRoom[ChatRoomSystem.MaxChatRoom];
        private static TouhouPets instance;
        public static PetChatRoom[] ChatRoom { get => chatRoom; set => chatRoom = value; }
        public static TouhouPets Instance { get => instance; set => instance = value; }

        public override void Load()
        {
            instance = this;

            InitializCrossModList();
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
            if (ModLoader.TryGetMod("Gensokyo", out result))
            {
                GensokyoSupport.Setup(result);
            }

            bool addCallForTest = true;
            if (addCallForTest)
            {
                this.SetModCall();
            }

            LoadClient();
        }
        private static void LoadClient()
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