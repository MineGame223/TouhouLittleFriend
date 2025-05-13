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
        private static void AddHomewardJourneyLoot(NPC npc, NPCLoot npcLoot)
        {
            bool hasHJMod = ModLoader.TryGetMod("ContinentOfJourney", out Mod result);
            if (!hasHJMod)
                return;

            bool isTimeGod = result.TryFind("TheOverwatcher", out ModNPC n) && npc.type == n.Type;
            if (isTimeGod)
                npcLoot.Add(ItemDropRule.Common(ItemType<SakuyaWatch>()));
        }
        private static void AddThoriumLoot(NPC npc, NPCLoot npcLoot)
        {
            bool hasThoMod = ModLoader.TryGetMod("ThoriumMod", out Mod result);
            if (!hasThoMod)
                return;

            bool isViscount = result.TryFind("Viscount", out ModNPC n) && npc.type == n.Type;
            if (isViscount)
                npcLoot.Add(ItemDropRule.OneFromOptions(1, ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>()));

            bool isStrier = (result.TryFind("BoreanStrider", out n) && npc.type == n.Type
                || result.TryFind("BoreanStriderPopped", out n) && npc.type == n.Type);
            if (isStrier)
                npcLoot.Add(ItemDropRule.OneFromOptions(1, ItemType<LettyGlobe>()));
        }
        private static void AddCalamityLoot(NPC npc, NPCLoot npcLoot)
        {
            bool hasCalMod = ModLoader.TryGetMod("CalamityMod", out Mod result);
            if (!hasCalMod)
                return;

            bool isOarfish = result.TryFind("OarfishHead", out ModNPC n) && npc.type == n.Type;
            if (isOarfish)
                npcLoot.Add(ItemDropRule.Common(ItemType<IkuOarfish>(), 20));
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!GetInstance<PetObtainConfig>().PetCanDropFromBoss)
                return;

            AddHomewardJourneyLoot(npc, npcLoot);
            AddThoriumLoot(npc, npcLoot);
            AddCalamityLoot(npc, npcLoot);

            CommonLoot(npc, npcLoot);
        }
        private static void CommonLoot(NPC npc, NPCLoot npcLoot)
        {
            int enemiesDropRate = 20;
            if (npc.type == NPCID.PirateCaptain)
            {
                npcLoot.Add(ItemType<MurasaBailer>(), enemiesDropRate - 10);
            }
            if (npc.type == NPCID.AngryNimbus)
            {
                npcLoot.Add(ItemType<RaikoDrum>(), enemiesDropRate);
            }
            if (npc.type == NPCID.BloodNautilus)
            {
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate - 15, ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>()));
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
                npcLoot.Add(ItemType<AyaCamera>(), enemiesDropRate - 5);
            }
            if (npc.type == NPCID.RedDevil)
            {
                npcLoot.Add(ItemType<ShinkiHeart>(), enemiesDropRate - 5);
            }
            if (npc.type == NPCID.Pumpking)
            {
                npcLoot.Add(ItemType<KokoroMask>(), enemiesDropRate - 5);
            }
            if (npc.type == NPCID.CorruptBunny || npc.type == NPCID.CrimsonBunny)
            {
                npcLoot.Add(ItemType<TewiCarrot>(), enemiesDropRate - 3);
            }
            if (npc.type == NPCID.Ghost || npc.type == NPCID.DungeonSpirit)
            {
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate - 5, ItemType<PoltergeistAlbum>()));
                npcLoot.Add(ItemDropRule.OneFromOptions(enemiesDropRate - 2, ItemType<SupportStick>()));
            }

            int commonDropRate = 3;
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(new NotDownedKingSlime(), new DownedKingSlime(), commonDropRate
                        , ItemType<DaiyouseiBomb>(), ItemType<LilyOneUp>(), ItemType<KoakumaPower>());
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.Add(new NotDownedEoC(), new DownedEoC(), ItemType<KogasaUmbrella>(), commonDropRate);
                    break;

                case NPCID.QueenBee:
                    npcLoot.Add(new NotDownedQueenBee(), new DownedQueenBee(), ItemType<WriggleInAJar>(), commonDropRate);
                    break;

                case NPCID.SkeletronHead:
                    npcLoot.Add(new NotDownedSkeletron(), new DownedSkeletron(), ItemType<HinaDoll>(), commonDropRate);
                    break;

                case NPCID.Deerclops:
                    npcLoot.Add(new NotDownedDeerclops(), new DownedDeerclops(), ItemType<CirnoIceShard>(), commonDropRate);
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(new NotDownedWoF(), new DownedWoF(), commonDropRate,
                        ItemType<UtsuhoEye>(), ItemType<RinSkull>());
                    break;

                case NPCID.QueenSlimeBoss:
                    npcLoot.Add(new NotDownedQueenSlime(), new DownedQueenSlime(), ItemType<PatchouliMoon>(), commonDropRate);
                    break;

                case NPCID.TheDestroyer:
                    npcLoot.Add(new NotDownedDestroyer(), new DownedDestroyer(), ItemType<MomoyoPickaxe>(), commonDropRate);
                    break;

                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                    npcLoot.Add(new KomeijiSister_NotDownedTwins(), new KomeijiSister_DownedTwins(), commonDropRate,
                        ItemType<SatoriSlippers>(), ItemType<KoishiTelephone>());
                    break;

                case NPCID.SkeletronPrime:
                    npcLoot.Add(new NotDownedPrime(), new DownedPrime(), ItemType<NitoriCucumber>(), commonDropRate);
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(new NotDownedPrime(), new DownedPrime(), ItemType<YukaSunflower>(), commonDropRate);
                    break;

                case NPCID.Golem:
                    npcLoot.Add(new NotDownedGolem(), new DownedGolem(), ItemType<SekibankiBow>(), commonDropRate);
                    break;

                case NPCID.HallowBoss:
                    npcLoot.Add(new NotDownedEoL(), new DownedEoL(), ItemType<TenshiKeyStone>(), commonDropRate);
                    break;

                case NPCID.DukeFishron:
                    npcLoot.Add(new NotDownedFishron(), new DownedFishron(), ItemType<IkuOarfish>(), commonDropRate);
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(new NotDownedCultist(), new DownedCultist(), commonDropRate,
                        ItemType<ReimuYinyangOrb>(), ItemType<MarisaHakkero>(), ItemType<SanaeCoin>());
                    break;

                case NPCID.MoonLordCore:
                    npcLoot.Add(new NotDownedMoonLord(), new DownedMoonLord(), commonDropRate,
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
