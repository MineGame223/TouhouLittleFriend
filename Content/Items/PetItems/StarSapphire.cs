using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Buffs.PetBuffs;

namespace TouhouPets.Content.Items.PetItems
{
    public class StarSapphire : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Projectiles.Pets.Star>(), BuffType<StarBuff>());
            Item.DefaultToVanitypetExtra(30, 26);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Sapphire, 1)
            .AddIngredient(ItemID.FallenStar, 1)
            .AddCondition(Language.GetOrRegister("Mods.TouhouPets.CraftCondition_StarSapphire"), () => false)
            .Register();
        }
    }
}
