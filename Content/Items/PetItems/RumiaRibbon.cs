using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class RumiaRibbon : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Rumia>(), BuffType<RumiaBuff>());
            Item.DefaultToVanitypetExtra(22, 22);
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
            .AddIngredient(ItemID.SoulofNight, 2)
            .AddTile(TileID.WorkBenches)
            .DisableDecraft()
            .Register();
        }
    }
}
