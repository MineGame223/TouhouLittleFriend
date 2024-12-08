using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using TouhouPets.Content.Buffs.PetBuffs;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets
{
    public class TouhouPetGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemType<KokoroMask>())
            {
                tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.KokoroTips"));
            }
            if (item.type == ItemType<RemiliaRedTea>() || item.type == ItemType<FlandrePudding>())
            {
                tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.VampireTips"));
            }
            if (item.type == ItemType<MurasaBailer>())
            {
                if (Main.remixWorld && GetInstance<PetAbilitiesConfig>().SpecialAbility_Murasa)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.MurasaTips"));
            }
            if (item.type == ItemType<HinaDoll>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Hina)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.HinaTips"));
            }
            if (item.type == ItemType<TenshiKeyStone>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Tenshin)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.TenshinTips"));
            }
            if (item.type == ItemType<SatoriSlippers>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Satori)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.SatoriTips"));
            }
            if (item.type == ItemType<WriggleInAJar>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Wriggle)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.WriggleTips"));
            }
            if (item.type == ItemType<YuyukoFan>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Yuyuko)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.YuyukoTips"));
            }
            if (item.type == ItemType<KaguyaBranch>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_MokuAndKaguya)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.KaguyaTips"));
            }
            if (item.type == ItemType<MokuMatch>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_MokuAndKaguya)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.MokuTips"));
            }
            if (item.type == ItemType<StarSapphire>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Star)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.StarTips"));
            }
            if (item.type == ItemType<MinorikoSweetPotato>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Minoriko)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.MinorikoTips"));
            }
            if (item.type == ItemType<TewiCarrot>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Tewi)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.TewiTips"));
            }
            if (item.type == ItemType<LettyGlobe>())
            {
                if (GetInstance<PetAbilitiesConfig>().SpecialAbility_Letty)
                    tooltips.MyTooltipLine(Language.GetTextValue("Mods.TouhouPets.LettyTips"));
            }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff<MinorikoBuff>() && GetInstance<PetAbilitiesConfig>().SpecialAbility_Minoriko)
            {
                if (ItemID.Sets.IsFood[item.type] && item.buffType > 0)
                {
                    int defaultTime = item.buffTime;
                    item.buffTime += (int)(defaultTime * 0.03f);
                }
            }
            return base.CanUseItem(item, player);
        }
        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (item.type == ItemID.Sapphire && !item.shimmered)
            {
                foreach (Item i in Main.ActiveItems)
                {
                    if (i.type == ItemID.FallenStar && i.getRect().Intersects(item.getRect()) && !i.shimmered)
                    {
                        i.stack--;
                        item.stack--;
                        if (i.stack <= 0)
                        {
                            i.TurnToAir();
                        }
                        if (item.stack <= 0)
                        {
                            item.TurnToAir();
                        }

                        ParticleOrchestraSettings settings;
                        for (int z = 0; z < 8; z++)
                        {
                            settings = new ParticleOrchestraSettings
                            {
                                PositionInWorld = item.Center + new Vector2(0, Main.rand.Next(0, 50)).RotatedByRandom(MathHelper.TwoPi),
                                MovementVector = Vector2.Zero,
                            };
                            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StardustPunch, settings);
                        }
                        settings = new ParticleOrchestraSettings
                        {
                            PositionInWorld = item.Center,
                            MovementVector = Vector2.Zero,
                        };
                        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerTownNPC, settings);

                        Item.NewItem(item.GetSource_FromThis(), item.getRect(), ItemType<StarSapphire>());
                        break;
                    }
                }
            }
        }
    }
}
