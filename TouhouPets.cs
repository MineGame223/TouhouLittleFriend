global using Terraria.ModLoader;
global using static Terraria.ModLoader.ModContent;

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

            //��Ҫ���б���г�ʼ��
            for (int i = 0; i < (int)TouhouPetID.Count; i++)
            {
                CrossModChatText[i] = [];
                CrossModChatCondition[i] = [];
                CrossModChatWeight[i] = [];
            }
        }
        public override void Unload()
        {
            instance = null;

            //��֪����ɶӰ�죬��д��
            CrossModChatText = null;
            CrossModChatCondition = null;
            CrossModChatWeight = null;
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