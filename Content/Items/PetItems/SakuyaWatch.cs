using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class SakuyaWatch : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Sakuya>(), BuffType<SakuyaBuff>());
            Item.DefaultToVanitypetExtra(30, 32);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SilverWatch, 1)
            .AddTile(TileID.CrystalBall)
            .DisableDecraft()
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.TungstenWatch, 1)
            .AddTile(TileID.CrystalBall)
            .DisableDecraft()
            .Register();
        }
    }
}
