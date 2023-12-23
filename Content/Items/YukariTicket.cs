using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class YukariTicket : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return TouhouPets.devMode;
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.IronBroadsword);
            Item.damage = 0;
            Item.width = 30;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = 0;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.MyTooltipLine("");
        }
        public override bool? UseItem(Player player)
        {
            player.GetModPlayer<TouhouPetPlayer>().purchaseValueCount = 0;
            player.GetModPlayer<TouhouPetPlayer>().totalPurchaseValueCount = 0;
            return null;
        }
    }
    public class DevGlobaItem : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return TouhouPets.devMode;
        }
        public override bool InstancePerEntity => true;
    }
}
