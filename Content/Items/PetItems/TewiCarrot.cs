using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class TewiCarrot : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Tewi>(), BuffType<TewiBuff>());
            Item.DefaultToVanitypetExtra(38, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasBuff(BuffType<EienteiBuff>()))
                player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
