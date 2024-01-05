using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class RaikoDrum : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Raiko>(), BuffType<RaikoBuff>());
            Item.DefaultToVanitypetExtra(28, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
