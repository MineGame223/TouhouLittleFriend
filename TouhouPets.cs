global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
global using static TouhouPets.ModUtils;
using Terraria;
using Terraria.ID;

namespace TouhouPets
{
    public partial class TouhouPets : Mod
    {
        private static TouhouPets instance;
        public static TouhouPets Instance { get => instance; set => instance = value; }

        public override void Load()
        {
            instance = this;

            InitializeChatSetting();
            InitializeCrossModList();
        }
        public override void Unload()
        {
            instance = null;

            NullifyChatSetting();
            NullifyCrossModList();
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