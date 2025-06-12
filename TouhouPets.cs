global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;
using System;
using Terraria;
using Terraria.ID;

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

            //需要对列表进行初始化
            for (int i = 0; i < (int)TouhouPetID.Count; i++)
            {
                CrossModDialogList[i] = [];
            }
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

            Func<bool> condi_1 = delegate () { return Main.LocalPlayer.ZoneBeach; };
            Func<bool> condi_2 = delegate () { return !Main.dayTime; };
            Func<bool> condi_3 = delegate () { return !Main.dayTime && Main.LocalPlayer.ZoneSkyHeight; };
            for (int i = 1; i <= 61; i++)
            {
                Call("PetDialog", i, $"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在海边出现", condi_1, 1);
                Call("PetDialog", i, $"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在夜晚出现", condi_2, 1);
                Call("PetDialog", i, $"这句话是由 {nameof(TouhouPets)} 给所有宠物添加的，只会在夜晚的太空出现", condi_2, 1);
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