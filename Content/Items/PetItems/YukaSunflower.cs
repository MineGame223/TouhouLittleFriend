using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Projectiles.Pets;

namespace TouhouPets.Content.Items.PetItems
{
    public class YukaSunflower : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ProjectileType<Yuka>(), BuffType<YukaBuff>());
            Item.DefaultToVanitypetExtra(28, 36);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuka)
                return;

            Item solution = Main.LocalPlayer.ChooseAmmo(new Item(ItemID.Clentaminator2));
            if (solution == null || solution.IsAir)
            {
                tooltips.InsertTooltipLine(Mod.GetLocalization("YukaTips2").Value);
                return;
            }

            if (Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(solution.type))
                tooltips.InsertTooltipLine(Mod.GetLocalization("YukaTips").Value);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}
