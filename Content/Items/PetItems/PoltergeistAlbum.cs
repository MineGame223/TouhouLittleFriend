using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class PoltergeistAlbum : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Lunasa>(), BuffType<PoltergeistBuff>());
            Item.DefaultToVanitypetExtra(30, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemType<LunasaViolin>(), 1)
            .AddIngredient(ItemType<MerlinTrumpet>(), 1)
            .AddIngredient(ItemType<LyricaKeyboard>(), 1)
            .Register();
        }
    }
}
