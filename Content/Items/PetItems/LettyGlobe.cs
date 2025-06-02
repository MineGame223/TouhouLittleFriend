using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class LettyGlobe : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Letty>(), BuffType<LettyBuff>());
            Item.DefaultToVanitypetExtra(26, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.SnowBlock, 10)
            .AddIngredient(ItemID.BorealWood, 5)
            .AddTile(TileID.WorkBenches)
            .DisableDecraft()
            .Register();
        }
    }
}
