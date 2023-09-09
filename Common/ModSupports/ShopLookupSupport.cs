using Terraria;
using Terraria.Localization;
using TouhouPets.Content.NPCs;

namespace TouhouPets
{
    internal static class ShopLookupSupport
    {
        public static void Setup(Mod slu)
        {
            if (slu == null)
            {
                return;
            }
            Condition inOverworld = Main.remixWorld ? 
                Condition.InUnderworldHeight : Condition.InOverworldHeight;
            RegisterMerchantInfo(slu, NPCType<YukariPortal>(), inOverworld, Condition.TimeNight, Condition.NotBloodMoon);
        }
        #region Basic Func
        private static void RegisterMerchantInfo(this Mod slu, int npcType, params Condition[] c)
        {
            slu.Call("NonPermanent", npcType, c);
        }
        private static void RegisterShopName(this Mod slu, int npcType, string shopName, LocalizedText text = null)
        {
            slu.Call("ShopName", npcType, shopName, text ?? Language.GetText(shopName));
        }
        #endregion
    }
}
