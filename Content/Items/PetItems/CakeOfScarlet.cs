using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Items.PetItems
{
    public class CakeOfScarlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(0, BuffType<ScarletBuff>());
            Item.DefaultToVanitypetExtra(26, 32);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemType<RemiliaRedTea>(), 1)
            .AddIngredient(ItemType<FlandrePudding>(), 1)
            .Register();
        }
    }
}