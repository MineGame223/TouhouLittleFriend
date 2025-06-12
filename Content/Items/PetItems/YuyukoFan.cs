using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class YuyukoFan : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Yuyuko>(), BuffType<YuyukoBuff>());
            Item.DefaultToVanitypetExtra(32, 32);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 10)
            .AddRecipeGroup(RecipeGroupID.Wood, 5)
            .AddIngredient(ItemID.ButterflyDust, 1)
            .AddTile(TileID.Loom)
            .AddCondition(Condition.InGraveyard)
            .DisableDecraft()
            .Register();
        }
    }
}
