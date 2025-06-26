using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using TouhouPets.Content.Items;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets
{
    internal class PetGlobalLoot : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!PetCanDropFromBoss)
                return;

            AddHomewardJourneyLoot(npc, npcLoot);
            AddThoriumLoot(npc, npcLoot);
            AddCalamityLoot(npc, npcLoot);
            AddCoraliteLoot(npc, npcLoot);

            CommonLoot(npc, npcLoot);
        }
        #region 联动掉落
        private static void AddCoraliteLoot(NPC npc, NPCLoot npcLoot)
        {
            string name = "Coralite";

            if (HasModAndFindNPC(name, npc, "NightmarePlantera"))
                npcLoot.Add(ItemDropRule.Common(ItemType<DoremyPillow>()));
        }
        private static void AddHomewardJourneyLoot(NPC npc, NPCLoot npcLoot)
        {
            string name = "ContinentOfJourney";

            if (HasModAndFindNPC(name, npc, "TheOverwatcher"))
                npcLoot.Add(ItemDropRule.Common(ItemType<SakuyaWatch>()));
        }
        private static void AddThoriumLoot(NPC npc, NPCLoot npcLoot)
        {
            string name = "ThoriumMod";

            if (HasModAndFindNPC(name, npc, "Viscount"))
                npcLoot.Add(ItemDropRule.OneFromOptions(1, ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>()));

            if (HasModAndFindNPC(name, npc, "BoreanStrider")
                || HasModAndFindNPC(name, npc, "BoreanStriderPopped"))
                npcLoot.Add(ItemDropRule.OneFromOptions(1, ItemType<LettyGlobe>()));
        }
        private static void AddCalamityLoot(NPC npc, NPCLoot npcLoot)
        {
            string name = "CalamityMod";

            if (HasModAndFindNPC(name, npc, "OarfishHead"))
                npcLoot.Add(ItemDropRule.Common(ItemType<IkuOarfish>(), 100));
        }
        #endregion
        private static void CommonLoot(NPC npc, NPCLoot npcLoot)
        {
            int enemiesDropRate = 100;
            if (npc.type == NPCID.WyvernHead)
            {
                npcLoot.Add(ItemType<MeirinPanda>(), enemiesDropRate - 15);
            }
            if (npc.type == NPCID.PirateCaptain)
            {
                npcLoot.Add(ItemType<MurasaBailer>(), enemiesDropRate / 4);
            }
            if (npc.type == NPCID.AngryNimbus)
            {
                npcLoot.Add(ItemType<RaikoDrum>(), enemiesDropRate);
            }
            if (npc.type == NPCID.BloodNautilus)
            {
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate / 4, ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>()));
            }
            if (npc.type == NPCID.Harpy)
            {
                npcLoot.Add(ItemType<MystiaFeather>(), enemiesDropRate);
            }
            if (npc.type == NPCID.Werewolf)
            {
                npcLoot.Add(ItemType<LunaMoon>(), enemiesDropRate);
            }
            if (npc.type == NPCID.Pixie)
            {
                npcLoot.Add(ItemType<SunnyMilk>(), enemiesDropRate);
            }
            if (npc.type == NPCID.WindyBalloon || npc.type == NPCID.Dandelion)
            {
                npcLoot.Add(ItemType<AyaCamera>(), enemiesDropRate);
            }
            if (npc.type == NPCID.RedDevil)
            {
                npcLoot.Add(ItemType<ShinkiHeart>(), enemiesDropRate);
            }
            if (npc.type == NPCID.Pumpking)
            {
                npcLoot.Add(ItemType<KokoroMask>(), enemiesDropRate / 4);
            }
            if (npc.type == NPCID.CorruptBunny || npc.type == NPCID.CrimsonBunny)
            {
                npcLoot.Add(ItemType<TewiCarrot>(), enemiesDropRate);
            }
            if (npc.type == NPCID.Ghost || npc.type == NPCID.DungeonSpirit)
            {
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate - 15, ItemType<PoltergeistAlbum>()));
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate - 15, ItemType<SupportStick>()));
            }

            int bossDropRate = 20;
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(new NotDownedKingSlime(), new DownedKingSlime(), bossDropRate
                        , ItemType<DaiyouseiBomb>(), ItemType<LilyOneUp>(), ItemType<KoakumaPower>());
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.Add(new NotDownedEoC(), new DownedEoC(), ItemType<KogasaUmbrella>(), bossDropRate);
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(new NotDownedQueenBee(), new DownedQueenBee(), ItemType<WriggleInAJar>(), bossDropRate);
                    break;

                case NPCID.SkeletronHead:
                    npcLoot.Add(new NotDownedSkeletron(), new DownedSkeletron(), ItemType<HinaDoll>(), bossDropRate);
                    break;

                case NPCID.Deerclops:
                    npcLoot.Add(new NotDownedDeerclops(), new DownedDeerclops(), ItemType<CirnoIceShard>(), bossDropRate);
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(new NotDownedWoF(), new DownedWoF(), bossDropRate,
                        ItemType<UtsuhoEye>(), ItemType<RinSkull>());
                    break;

                case NPCID.QueenSlimeBoss:
                    npcLoot.Add(new NotDownedQueenSlime(), new DownedQueenSlime(), ItemType<PatchouliMoon>(), bossDropRate);
                    break;

                case NPCID.TheDestroyer:
                    npcLoot.Add(new NotDownedDestroyer(), new DownedDestroyer(), ItemType<MomoyoPickaxe>(), bossDropRate);
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npcLoot.Add(new KomeijiSister_NotDownedTwins(), new KomeijiSister_DownedTwins(), bossDropRate,
                        ItemType<SatoriSlippers>(), ItemType<KoishiTelephone>());
                    break;

                case NPCID.SkeletronPrime:
                    npcLoot.Add(new NotDownedPrime(), new DownedPrime(), ItemType<NitoriCucumber>(), bossDropRate);
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(new NotDownedPlantera(), new DownedPlantera(), ItemType<YukaSunflower>(), bossDropRate);
                    break;

                case NPCID.Golem:
                    npcLoot.Add(new NotDownedGolem(), new DownedGolem(), ItemType<SekibankiBow>(), bossDropRate);
                    break;

                case NPCID.HallowBoss:
                    npcLoot.Add(new NotDownedEoL(), new DownedEoL(), ItemType<TenshiKeyStone>(), bossDropRate);
                    break;

                case NPCID.DukeFishron:
                    npcLoot.Add(new NotDownedFishron(), new DownedFishron(), ItemType<IkuOarfish>(), bossDropRate);
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(new NotDownedCultist(), new DownedCultist(), bossDropRate,
                        ItemType<ReimuYinyangOrb>(), ItemType<MarisaHakkero>(), ItemType<SanaeCoin>());
                    break;

                case NPCID.MoonLordCore:
                    npcLoot.Add(new NotDownedMoonLord(), new DownedMoonLord(), bossDropRate,
                        ItemType<HecatiaPlanet>(), ItemType<JunkoMooncake>());
                    break;

                case NPCID.DD2Betsy:
                    break;

                default:
                    break;
            }
        }
    }
}
