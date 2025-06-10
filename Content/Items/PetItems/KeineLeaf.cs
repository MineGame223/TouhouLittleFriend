using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class KeineLeaf : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Keine>(), BuffType<KeineBuff>());
            Item.DefaultToVanitypetExtra(24, 30);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Book, 1)
            .AddIngredient(ItemID.AncientCloth, 2)
            .AddTile(TileID.Bookcases)
            .DisableDecraft()
            .Register();
        }
    }
}
