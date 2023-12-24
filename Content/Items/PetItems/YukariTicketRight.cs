using Terraria;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class YukariTicketRight : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Yukari>(), BuffType<LightYukariBuff>());
            Item.DefaultToVanitypetExtra(20, 20, ItemRarityID.Yellow, Item.sellPrice(0, 25, 0, 0));
            Item.UseSound = null;
        }
    }
}
