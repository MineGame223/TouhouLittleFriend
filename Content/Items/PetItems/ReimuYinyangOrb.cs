using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class ReimuYinyangOrb : ModItem
    {
        public override void SetDefaults()
        {            
            Item.DefaultToVanitypet(ProjectileType<Reimu>(), BuffType<ReimuBuff>());
            Item.DefaultToVanitypetExtra(26, 26);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
