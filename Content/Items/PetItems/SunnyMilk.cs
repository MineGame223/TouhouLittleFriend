using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class SunnyMilk : ModItem
    {
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Sunny>(), BuffType<SunnyBuff>());
            Item.DefaultToVanitypetExtra(26, 28);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
