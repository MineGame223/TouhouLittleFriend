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

        /// <summary>
        /// 处理注册评价的方法
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="modName">模组名</param>
        /// <param name="list">注册对象列表</param>
        /// <param name="dictionaryID">起始索引字典枚举</param>
        private static void HandleRegisterComment(this Marisa marisa, string modName, DictionaryID dictionaryID, object list)
        {
            //记录对应字典的起始索引值
            int lastIndex = marisa.ChatDictionary.Count;
            startIndex[(int)dictionaryID] = lastIndex + 1;

            //根据对应列表长度赋予其索引值
            if (list is List<int> vanillaList)
            {
                if (vanillaList.Count > 0)
                {
                    for (int i = 0; i < vanillaList.Count; i++)
                    {
                        string text = Language.GetTextValue($"{Path}.{modName}_{i + 1}");
                        marisa.ChatDictionary.TryAdd(startIndex[(int)dictionaryID] + i, text);
                    }
                }
            }

            if (list is List<string> modList)
            {
                if (modList.Count > 0)
                {
                    for (int i = 0; i < modList.Count; i++)
                    {
                        string text = Language.GetTextValue($"{Path}.{modName}_{i + 1}");
                        marisa.ChatDictionary.TryAdd(startIndex[(int)dictionaryID] + i, text);
                    }
                }
            }
        }
        
        /// <summary>
        /// 注册评价
        /// </summary>
        /// <param name="marisa"></param>
        public static void RegisterComment(this Marisa marisa)
        {
            marisa.HandleRegisterComment("Vanilla", DictionaryID.Vanilla, bossIDList_Vanilla);
            marisa.HandleRegisterComment("Coralite", DictionaryID.Coralite, bossIDList_Coralite);
            marisa.HandleRegisterComment("Coralite", DictionaryID.Coralite, bossIDList_Coralite);
            marisa.HandleRegisterComment("Thorium", DictionaryID.Thorium, bossIDList_Thorium);
            marisa.HandleRegisterComment("HJ", DictionaryID.HomewardJourney, bossIDList_HJ);
            //由于被动添加的跨模组评价不采用索引值，因此无需注册流程
        }
        
        /// <summary>
        /// 跨模组Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="bossType"></param>
        public static void BossChat_CrossMod(this Projectile marisa, int bossType)
        {
            //若列表不存在内容，则不执行后续
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
        
        /// <summary>
        /// 原版Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="bossType"></param>
        public static void BossChat_Vanilla(this Projectile marisa, int bossType)
        {
            //以防万一（？）
            if (bossIDList_Vanilla.Count <= 0)
                return;

            int index = startIndex[(int)DictionaryID.Vanilla];

            //若被检测的种类不包含在该列表中，则不执行后续
            if (!bossIDList_Vanilla.Contains(bossType))
                return;

            //魔焰眼和激光眼的评价是一样的
            if (bossType == NPCID.Spazmatism)
                bossType = NPCID.Retinazer;

            marisa.SetChat(index + bossIDList_Vanilla.IndexOf(bossType));
        }
        
        /// <summary>
        /// 处理主动添加的跨模组Boss评价的方法
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="modName">模组名</param>
        /// <param name="boss">Boss的实例</param>
        /// <param name="bossType">Boss名称</param>
        /// <param name="list">对应列表</param>
        /// <param name="dictionaryID">对应起始索引字典枚举</param>
        private static void HandleModBossChatFromList(this Projectile marisa, string modName,
            NPC boss, string bossType, List<string> list, DictionaryID dictionaryID)
        {
            //以防万一（？）
            if (list.Count <= 0)
                return;

            int index = startIndex[(int)dictionaryID];

            //若被检测的种类不包含在该列表中，则不执行后续
            if (!list.Contains(bossType))
                return;

            //发现存在对应模组与生物时，进行评价
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
        
        /// <summary>
        /// 珊瑚石Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
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
        
        /// <summary>
        /// 瑟银Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
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
        
        /// <summary>
        /// 旅人归途Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
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
        
        /// <summary>
        /// 幻想乡Boss评价
        /// </summary>
        /// <param name="marisa"></param>
        /// <param name="boss"></param>
        public static void BossChat_Gensokyo(this Projectile marisa, NPC boss)
        {
            //string modName = "Gensokyo";
        }
    }
}
