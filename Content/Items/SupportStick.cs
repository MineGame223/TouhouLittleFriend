using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class SupportStick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            BandPlayer bp = player.GetModPlayer<BandPlayer>();
            if (player.altFunctionUse == 2 && bp.manualStartBand)
            {
                bp.rerollMusic = false;
            }
            else
            {
                bp.manualStartBand = !bp.manualStartBand;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Glowstick, 5)
            .AddIngredient(ItemID.Sapphire, 2)
            .AddTile(TileID.WorkBenches)
            .AddCondition(Condition.InGraveyard)
            .Register();
        }
    }
}
