using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TouhouPets.Content.Items
{
    public class SupportStick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Language.GetTextValue("Mods.TouhouPets.PressShift");
            if (Main.keyState.PressingShift())
            {
                text = Language.GetTextValue("Mods.TouhouPets.CustomModeDescrip");
            }
            ModUtils.InsertTooltipLine(tooltips, text);
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
            if (player.altFunctionUse != 2)
            {
                bp.manualStartBand = !bp.manualStartBand;
            }
            if (bp.manualStartBand && (bp.musicRerolled || player.altFunctionUse == 2))
            {
                bp.musicRerolled = false;
                bp.manualRerolled = true;
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
