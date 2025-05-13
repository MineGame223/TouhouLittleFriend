using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class HauntedTicket : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            BandPlayer bp = player.GetModPlayer<BandPlayer>();
            if (player.altFunctionUse == 2 && bp.ticketUsed)
            {
                bp.rerollMusic = false;
            }
            else
            {
                if (player.itemAnimation == 1)
                {
                    bp.ticketUsed = !bp.ticketUsed;
                }
            }
            return null;
        }
    }
}
