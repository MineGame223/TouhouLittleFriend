using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class YukaSunflower : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.SacrificeCountNeeded(1);
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Yuka>(), BuffType<YukaBuff>());
            Item.DefaultToVanitypetExtra(28, 36);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Item solution = Main.LocalPlayer.ChooseAmmo(new Item(ItemID.Clentaminator2));
            if (solution == null || solution.IsAir)
                return;

            if (Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(solution.type)
                 && GetInstance<PetAbilitiesConfig>().SpecialAbility)
                tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.YukaTips"));
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
