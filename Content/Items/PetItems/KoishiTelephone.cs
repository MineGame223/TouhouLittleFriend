using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class KoishiTelephone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
            Terraria.ID.ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<SatoriSlippers>();
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Koishi>(), BuffType<KoishiBuff>());
            Item.DefaultToVanitypetExtra(26, 34);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasBuff(BuffType<KomeijiBuff>()))
                player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
