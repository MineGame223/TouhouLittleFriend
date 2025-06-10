using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class KaguyaBranch : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Kaguya>(), BuffType<KaguyaBuff>());
            Item.DefaultToVanitypetExtra(32, 32);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasBuff(BuffType<EienteiBuff>()))
                player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.Wood, 5)
            .AddIngredient(ItemID.Amethyst, 1)
            .AddIngredient(ItemID.Topaz, 1)
            .AddIngredient(ItemID.Sapphire, 1)
            .AddIngredient(ItemID.Emerald, 1)
            .AddIngredient(ItemID.Ruby, 1)
            .AddTile(TileID.WorkBenches)
            .DisableDecraft()
            .Register();
        }
    }
}
