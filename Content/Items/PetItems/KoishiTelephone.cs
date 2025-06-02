using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class KoishiTelephone : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<SatoriSlippers>();
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Koishi>(), BuffType<KoishiBuff>());
            Item.DefaultToVanitypetExtra(26, 34);
        }
        public override bool CanUseItem(Player player)
        {
            foreach (Projectile koishi in Main.ActiveProjectiles)
            {
                if (koishi.owner == Main.myPlayer && koishi.type == Item.shoot)
                {
                    if (koishi.ai[1] >= 2 && koishi.ai[1] <= 3)
                    {
                        if (Main.myPlayer == player.whoAmI)
                            Main.NewText(Language.GetTextValue("Mods.TouhouPets.KoishiTelephone1"));
                    }
                    else if (koishi.ai[1] >= 4 && koishi.ai[1] <= 7)
                    {
                        if (Main.myPlayer == player.whoAmI)
                            Main.NewText(Language.GetTextValue("Mods.TouhouPets.KoishiTelephone2"));
                        return false;
                    }
                }
            }
            return base.CanUseItem(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.HasBuff(BuffType<KomeijiBuff>()))
                player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
