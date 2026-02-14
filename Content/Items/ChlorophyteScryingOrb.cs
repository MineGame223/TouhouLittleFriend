using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace TouhouPets.Content.Items
{
    public class ChlorophyteScryingOrb : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.rare = ItemRarityID.Lime;
        }
        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation += new Vector2(-10 * player.direction, 8);
            player.remoteVisionForDrone = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (SpecialAbility_Yuka)
                tooltips.InsertTooltipLine(Mod.GetLocalization("Items.ChlorophyteScryingOrb.Tooltip1").Value);
            else
                tooltips.InsertTooltipLine(Mod.GetLocalization("Items.ChlorophyteScryingOrb.Tooltip2").Value);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MagicMirror, 1)
            .AddIngredient(ItemID.Lens, 2)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.IceMirror, 1)
            .AddIngredient(ItemID.Lens, 2)
            .AddIngredient(ItemID.ChlorophyteBar, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}
