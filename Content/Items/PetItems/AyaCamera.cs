using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class AyaCamera : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Aya>(), BuffType<AyaBuff>());
            Item.DefaultToVanitypetExtra(30, 24);
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Item.shimmerTime = 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BlackLens, 1)
            .AddRecipeGroup(RecipeGroupID.IronBar, 5)
            .AddIngredient(ItemID.Wire, 7)
            .AddIngredient(ItemID.SoulofFlight, 3)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
