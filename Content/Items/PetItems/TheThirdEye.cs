using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class TheThirdEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Satori>(), BuffType<KomeijiBuff>());
            Item.DefaultToVanitypetExtra(32, 34);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemType<SatoriSlippers>(), 1)
            .AddIngredient(ItemType<KoishiTelephone>(), 1)
            .AddIngredient(ItemType<RinSkull>(), 1)
            .AddIngredient(ItemType<UtsuhoEye>(), 1)
            .Register();
        }
    }
}
