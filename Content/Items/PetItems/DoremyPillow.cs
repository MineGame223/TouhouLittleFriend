using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class DoremyPillow : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Doremy>(), BuffType<DoremyBuff>());
            Item.DefaultToVanitypetExtra(36, 28);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 30)
            .AddIngredient(ItemID.FlinxFur, 8)
            .AddTile(TileID.Loom)
            .DisableDecraft()
            .Register();
        }
    }
}
