using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using TouhouPets.Content.Items;
using TouhouPets.Content.Items.PetItems;

namespace TouhouPets
{
    internal class PetGlobalLoot : GlobalNPC
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            int commonDropRate = 3;
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    npcLoot.Add(new NotDownedKingSlime(), new DownedKingSlime(), commonDropRate
                        , ItemType<DaiyouseiBomb>(), ItemType<LilyOneUp>());
                    break;

                case NPCID.EyeofCthulhu:
                    npcLoot.Add(new NotDownedEoC(), new DownedEoC(), ItemType<KogasaUmbrella>(), commonDropRate);
                    break;

                case NPCID.EaterofWorldsHead:
                    Add_ScarletSister(npcLoot, commonDropRate);
                    break;

                case NPCID.BrainofCthulhu:
                    npcLoot.Add(new NotDownedEvilBoss(), new DownedEvilBoss(), commonDropRate,
                        ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>());
                    break;

                case NPCID.QueenBee:
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
                    npcLoot.Add(new NotDownedQueenSlime(), new DownedQueenSlime(), ItemType<UselessBook>(), commonDropRate);
                    break;

                case NPCID.TheDestroyer:
                    break;

                case NPCID.Retinazer:
                    npcLoot.Add(new KomeijiSister_NotDownedTwins(), new KomeijiSister_DownedTwins(), commonDropRate,
                        ItemType<SatoriSlippers>(), ItemType<KoishiTelephone>());
                    break;
                case NPCID.Spazmatism:
                    npcLoot.Add(new KomeijiSister_NotDownedTwins(), new KomeijiSister_DownedTwins(), commonDropRate,
                        ItemType<SatoriSlippers>(), ItemType<KoishiTelephone>());
                    break;

                case NPCID.SkeletronPrime:
                    npcLoot.Add(new NotDownedPrime(), new DownedPrime(), ItemType<NitoriCucumber>(), commonDropRate);
                    break;

                case NPCID.Plantera:
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
        private static void Add_ScarletSister(NPCLoot loot, int dropRate)
        {
            IItemDropRule rule1 = loot.Add(new LeadingConditionRule(new ScarletSister_NotDownedEoW()));
            IItemDropRule rule2 = loot.Add(new LeadingConditionRule(new ScarletSister_DownedEoW()));
            rule1.OnSuccess(ItemDropRule.Common(ItemType<RemiliaRedTea>()));
            rule1.OnSuccess(ItemDropRule.Common(ItemType<FlandrePudding>()));
            rule2.OnSuccess(ItemDropRule.OneFromOptions(dropRate, ItemType<RemiliaRedTea>(), ItemType<FlandrePudding>()));
        }
    }
}
