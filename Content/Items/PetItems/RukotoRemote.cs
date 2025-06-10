using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class RukotoRemote : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Rukoto>(), BuffType<RukotoBuff>());
            Item.DefaultToVanitypetExtra(20, 36);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddIngredient(ItemID.Wire, 12)
            .AddIngredient(ItemID.LihzahrdPowerCell, 1)
            .AddTile(TileID.MythrilAnvil)
            .DisableDecraft()
            .Register();
        }
    }
}
