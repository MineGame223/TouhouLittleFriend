using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class SizuhaBrush : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Sizuha>(), BuffType<SizuhaBuff>());
            Item.DefaultToVanitypetExtra(30, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Paintbrush, 1)
            .AddIngredient(ItemID.RedPaint, 1)
            .AddIngredient(ItemID.OrangePaint, 1)
            .AddIngredient(ItemID.YellowPaint, 1)
            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
