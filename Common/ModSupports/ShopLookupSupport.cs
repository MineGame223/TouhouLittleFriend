using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using TouhouPets.Content.NPCs;

namespace TouhouPets
{
    internal static class ShopLookupSupport
    {
        public static void Setup(Mod slu)
        {
            string prefix = $"Mods.{nameof(TouhouPets)}.";
            if (slu == null)
            {
                return;
            }

            Condition inOverworld = Main.remixWorld ?
                Condition.InUnderworldHeight : Condition.InOverworldHeight;

            RegisterMerchantInfo(slu, NPCType<YukariPortal>(), inOverworld, Condition.TimeNight, Condition.NotBloodMoon);

            Dictionary<string, LocalizedText> shopLocals = new()
            {
                { "Shop", Language.GetText(prefix + "PetShop") },
                { "Shop2", Language.GetText(prefix + "LightPetShop") },
            };
            RegisterShopName(slu, NPCType<YukariPortal>(), shopLocals);
        }
        #region Basic Func
        private static void RegisterMerchantInfo(this Mod slu, int npcType, params Condition[] c)
        {
            slu.Call("NonPermanent", npcType, c);
        }
        private static void RegisterShopName(this Mod slu, int npcType, Dictionary<string, LocalizedText> shopLocals)
        {
            slu.Call(0, npcType, shopLocals);
        }
        #endregion
    }
}
