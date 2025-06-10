using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class SatoriSlippers : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<KoishiTelephone>();
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Satori>(), BuffType<SatoriBuff>());
            Item.DefaultToVanitypetExtra(30, 24);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasBuff(BuffType<KomeijiBuff>()))
                player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
