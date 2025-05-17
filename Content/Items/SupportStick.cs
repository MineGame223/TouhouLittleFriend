using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TouhouPets.Content.Items
{
    public class SupportStick : ModItem
    {
        private bool IsManualMode { get => Main.LocalPlayer.GetModPlayer<ConcertPlayer>().manualStartBand; }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = 0;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (IsManualMode)
            {
                spriteBatch.TeaNPCDraw(AltVanillaFunction.GetExtraTexture("SupportStick_Yellow")
                    , position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0);
            }
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Type, out _, out Rectangle itemFrame);
            Vector2 drawOrigin = itemFrame.Size() / 2;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);
            if (IsManualMode)
            {
                spriteBatch.TeaNPCDraw(AltVanillaFunction.GetExtraTexture("SupportStick_Yellow")
                    , drawPosition, itemFrame, alphaColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }
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
            .AddIngredient(ItemID.RainbowMoss, 10)
            .AddIngredient(ItemID.Gel, 30)
            .AddTile(TileID.WorkBenches)
            .AddCondition(Condition.InGraveyard)
            .Register();
        }
    }
}
