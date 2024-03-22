using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Items.PetItems
{
    public class StarSapphire : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Projectiles.Pets.Star>(), BuffType<StarBuff>());
            Item.DefaultToVanitypetExtra(30, 26);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
