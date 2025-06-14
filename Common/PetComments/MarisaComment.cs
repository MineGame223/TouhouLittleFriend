using Terraria.ID;
using Terraria;
using TouhouPets.Content.Projectiles.Pets;
using static TouhouPets.ModUtils;
using static TouhouPets.TouhouPets;
using Terraria.Localization;
using System.Collections.Generic;

namespace TouhouPets
{
    public static partial class MarisaComment
    {
        private enum DictionaryID : int
        {
            Vanilla,
            Coralite,
            Thorium,
            HomewardJourney,
            Gensokyo,
            Mods,
            Count
        }
        private const string Path = $"Mods.{nameof(TouhouPets)}.Chat_Marisa";

        private static List<int> bossIDList_Vanilla = [
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.EaterofWorldsHead,
            NPCID.BrainofCthulhu,
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.Deerclops,
            NPCID.WallofFlesh,
            NPCID.QueenSlimeBoss,
            NPCID.Retinazer,
            NPCID.Spazmatism,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.DukeFishron,
            NPCID.HallowBoss,
            NPCID.CultistBoss,
            NPCID.MoonLordCore,
            NPCID.DD2Betsy,
            ];

        private static List<string> bossIDList_Coralite = [
            "Rediancie",
            "BabyIceDragon",
            "SlimeEmperor",
            "ShadowBall",
            "Bloodiancie",
            "ThunderveinDragon",
            "ZacurrentDragon",
            "NightmarePlantera"
            ];

        private static List<string> bossIDList_Thorium = [
            "TheGrandThunderBird",
            "QueenJellyfish",
            "Viscount",
            "GraniteEnergyStorm",
            "BuriedChampion",
            "StarScouter",
            "BoreanStrider",
            "FallenBeholder",
            "Lich",
            "ForgottenOne",
            "SlagFury",
            "Aquaius",
            "Omnicide",
            "DreamEater"
            ];

        private static List<string> bossIDList_HJ = [
            "MarquisMoonsquid",
            "PriestessRod",
            "TheMotherbrain",
            "Diver",
            "WallofShadow",
            "SlimeGod",
            "TheOverwatcher",
            "TheLifebringerHead",
            "TheMaterealizer",
            "ScarabBelief",
            "WorldsEndEverlastingFallingWhale",
            "TheSon"
            ];

        private static int[] startIndex = new int[(int)DictionaryID.Count];

        private static void HandleRegisterModComment(this Marisa marisa, string modName, List<string> list, DictionaryID dictionaryID)
        {
            int lastIndex = marisa.ChatDictionary.Count;
            startIndex[(int)dictionaryID] = lastIndex + 1;

            for (int i = 0; i < list.Count; i++)
            {
                string text = Language.GetTextValue($"{Path}.{modName}_{i + 1}");
                marisa.ChatDictionary.TryAdd(startIndex[(int)dictionaryID] + i, text);
            }
        }
        public static void RegisterComment(this Marisa marisa)
        {
            int lastIndex = marisa.ChatDictionary.Count;
            startIndex[(int)DictionaryID.Vanilla] = lastIndex + 1;

            for (int i = 0; i < bossIDList_Vanilla.Count; i++)
            {
                string text = Language.GetTextValue($"{Path}.Vanilla_{i + 1}");
                marisa.ChatDictionary.TryAdd(startIndex[(int)DictionaryID.Vanilla] + i, text);
            }

            marisa.HandleRegisterModComment("Coralite", bossIDList_Coralite, DictionaryID.Coralite);
            marisa.HandleRegisterModComment("Thorium", bossIDList_Thorium, DictionaryID.Thorium);
            marisa.HandleRegisterModComment("HJ", bossIDList_HJ, DictionaryID.HomewardJourney);
        }
        public static void BossChat_CrossMod(this Projectile marisa, int bossType)
        {
            if (CrossModBossComment == null || CrossModBossComment.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < CrossModBossComment.Count; i++)
            {
                if (bossType == CrossModBossComment[i].ObjectType)
                {
                    marisa.SetChat(CrossModBossComment[i].CommentText.Value);
                    break;
                }
            }
        }
        public static void BossChat_Vanilla(this Projectile marisa, int bossType)
        {
            //以防万一（？）
            if (bossIDList_Vanilla.Count <= 0)
                return;

            int index = startIndex[(int)DictionaryID.Vanilla];

            if (!bossIDList_Vanilla.Contains(bossType))
                return;

            //魔焰眼和激光眼的评价是一样的
            if (bossType == NPCID.Spazmatism)
                bossType = NPCID.Retinazer;

            marisa.SetChat(index + bossIDList_Vanilla.IndexOf(bossType));
        }
        private static void HandleModBossChatFromList(this Projectile marisa, string modName,
            NPC boss, string bossType, List<string> list, DictionaryID dictionaryID)
        {
            //以防万一（？）
            if (list.Count <= 0)
                return;

            int index = startIndex[(int)dictionaryID];

            if (!list.Contains(bossType))
                return;

            if (HasModAndFindNPC(modName, boss, bossType))
            {
                string actualType = bossType;

                //三灾的评价是一样的
                if (dictionaryID == DictionaryID.Thorium)
                {
                    if (bossType == "Aquaius" || bossType == "Omnicide")
                        actualType = "SlagFury";
                }

                marisa.SetChat(index + list.IndexOf(actualType));
            }
        }
        public static void BossChat_Coralite(this Projectile marisa, NPC boss)
        {
            string modName = "Coralite";
            List<string> list = bossIDList_Coralite;
            DictionaryID dictionary = DictionaryID.Coralite;

            foreach (string name in list)
            {
                marisa.HandleModBossChatFromList(modName, boss, name, list, dictionary);
            }
        }
        public static void BossChat_Thorium(this Projectile marisa, NPC boss)
        {
            string modName = "ThoriumMod";
            List<string> list = bossIDList_Thorium;
            DictionaryID dictionary = DictionaryID.Thorium;

            foreach (string name in list)
            {
                marisa.HandleModBossChatFromList(modName, boss, name, list, dictionary);
            }
        }
        public static void BossChat_HomewardHourney(this Projectile marisa, NPC boss)
        {
            string modName = "ContinentOfJourney";
            List<string> list = bossIDList_HJ;
            DictionaryID dictionary = DictionaryID.HomewardJourney;

            foreach (string name in list)
            {
                marisa.HandleModBossChatFromList(modName, boss, name, list, dictionary);
            }
        }
        public static void BossChat_Gensokyo(this Projectile marisa, NPC boss)
        {
            //string modName = "Gensokyo";
        }
    }
}
