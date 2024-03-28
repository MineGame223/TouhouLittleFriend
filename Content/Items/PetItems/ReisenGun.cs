using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class ReisenGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Reisen>(), BuffType<ReisenBuff>());
            Item.DefaultToVanitypetExtra(34, 36);
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, -10);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Megaphone, 1)
            .AddIngredient(ItemID.IllegalGunParts, 1)
            .AddTile(TileID.Anvils)
            .DisableDecraft()
            .Register();
        }
    }
}
