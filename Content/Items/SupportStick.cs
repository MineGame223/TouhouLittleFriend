using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class SupportStick : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<CustomSupportStick>();
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
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
            ConcertPlayer bp = player.GetModPlayer<ConcertPlayer>();
            if (player.altFunctionUse == 2)
            {
                if (bp.manualStartBand)
                {
                    bp.musicRerolled = false;
                }
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
