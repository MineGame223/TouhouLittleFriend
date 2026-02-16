using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class SanaeCoin : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Sanae>(), BuffType<SanaeBuff>());
            Item.DefaultToVanitypetExtra(32, 32);
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void UpdateInventory(Player player)
        {
            if (Item.stack == 10)
            {
                Item.stack -= 10;
                if (Item.stack <= 0)
                    Item.TurnToAir();

                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero
                , ProjectileType<SanaeCoinSpawnAnimation>(), 0, 0, Main.myPlayer);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
    public class SanaeCoinRare : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Sanae>(), BuffType<SanaeBuffRare>());
            Item.DefaultToVanitypetExtra(32, 36);
        }
        /*public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemType<SanaeCoin>(), 10)
            .AddCondition(Language.GetOrRegister("Mods.TouhouPets.CraftCondition_SanaeCoin"), () => false)
            .DisableDecraft()
            .Register();
        }*/
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
