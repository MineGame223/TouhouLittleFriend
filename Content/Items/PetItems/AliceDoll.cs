using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class AliceDoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Alice>(), BuffType<AliceBuff>());
            Item.DefaultToVanitypetExtra(28, 40);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Item.shimmerTime = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.Wood, 12)
            .AddIngredient(ItemID.Silk, 7)
            .AddIngredient(ItemID.Sapphire, 2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}