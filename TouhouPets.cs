global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
global using static TouhouPets.ModUtils;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TouhouPets
{
    public partial class TouhouPets : Mod
    {
        private static TouhouPets instance;
        public static TouhouPets Instance { get => instance; set => instance = value; }

        private static bool forceCompatibilityMode;
        public static bool ForceCompatibilityMode { get => forceCompatibilityMode; set => forceCompatibilityMode = value; }

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
            AutoSetCompatibilityMode();
            if (ModLoader.TryGetMod("ShopLookup", out Mod result))
            {
                ShopLookupSupport.Setup(result);
            }
            if (ModLoader.TryGetMod("Gensokyo", out result))
            {
                GensokyoSupport.Setup(result);
            }

            bool addCallForTest = false;
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
        private static List<string> banList = [
            "CalamityMod",
            "CatalystMod",
            "FargowiltasSouls",
            ];
        private static void AutoSetCompatibilityMode()
        {
            ForceCompatibilityMode = false;
            foreach (string name in banList)
            {
                if (ModLoader.TryGetMod(name, out _))
                {
                    ForceCompatibilityMode = true;
                }
            }
        }
    }
}